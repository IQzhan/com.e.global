using System;
using System.Collections;
using System.Collections.Generic;

namespace E
{
    public class BehaviourCollection : IEnumerable<GlobalBehaviour>, IEnumerable
    {
        public BehaviourCollection()
        {
            linkFactory = new LinkFactory<GlobalBehaviour>();
            typeLinks = new SortedList<int, LinkFactory<GlobalBehaviour>.LinkAddress>();
        }

        private readonly LinkFactory<GlobalBehaviour> linkFactory;

        private readonly SortedList<int, LinkFactory<GlobalBehaviour>.LinkAddress> typeLinks;

        private void GetTypeLink(in Type type, out LinkFactory<GlobalBehaviour>.LinkAddress typelink)
        {
            int hashCode = type.GetHashCode();
            if (!typeLinks.TryGetValue(hashCode, out typelink))
            {
                typelink = linkFactory.RequireNewLink();
                typeLinks.Add(hashCode, typelink);
            }
        }

        private void SetTypeLink(in Type type, in LinkFactory<GlobalBehaviour>.LinkAddress typelink)
        {
            int hashCode = type.GetHashCode();
            typeLinks[hashCode] = typelink;
        }

        public void Add(in GlobalBehaviour value)
        {
            Type type = value.GetType();
            GetTypeLink(type, out LinkFactory<GlobalBehaviour>.LinkAddress typelink);
            value.ID = linkFactory.Add(ref typelink, value);
            SetTypeLink(type, typelink);
        }

        public void Remove(in GlobalBehaviour value)
        {
            Type type = value.GetType();
            GetTypeLink(type, out LinkFactory<GlobalBehaviour>.LinkAddress typelink);
            linkFactory.Remove(ref typelink, value.ID);
            SetTypeLink(type, typelink);
        }

        public GlobalBehaviour Get(in Type type)
        {
            GetTypeLink(type, out LinkFactory<GlobalBehaviour>.LinkAddress typelink);
            return linkFactory.Get(typelink.first);
        }

        public GlobalBehaviour[] Gets(in Type type)
        {
            GetTypeLink(type, out LinkFactory<GlobalBehaviour>.LinkAddress typelink);
            return linkFactory.Gets(typelink.first);
        }

        public void Clear()
        {
            linkFactory.Clear();
            typeLinks.Clear();
        }

        private GlobalBehaviour GetFirst()
        {
            return linkFactory.GetFirst();
        }

        private GlobalBehaviour GetNext(in int address)
        {
            return linkFactory.GetNext(address);
        }

        #region Enumerator
        public struct Enumerator : IEnumerator<GlobalBehaviour>, IEnumerator, IDisposable
        {
            public Enumerator(in BehaviourCollection collection)
            {
                this.collection = collection;
                current = null;
            }

            private BehaviourCollection collection;

            private GlobalBehaviour current;

            public GlobalBehaviour Current { get => current; }

            object IEnumerator.Current => Current;

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (current == null)
                {
                    current = collection.GetFirst();
                }
                else
                {
                    current = collection.GetNext(current.ID);
                }
                return current != null;
            }

            public void Dispose()
            {

            }
        }

        public IEnumerator<GlobalBehaviour> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion
    }

    internal class LinkFactory<T>
    {
        private struct Node
        {
            public static Node Default { get => new Node() { value = default, prev = -1, next = -1, addressPrev = -1, addressNext = -1 }; }

            public T value;

            internal int prev;

            internal int next;

            internal int addressPrev;

            internal int addressNext;
        }

        public struct LinkAddress
        {
            public static LinkAddress Default { get => new LinkAddress() { first = -1, last = -1 }; }

            internal int first;

            internal int last;
        }

        private readonly List<Node> m_Block;

        private readonly Stack<int> m_Uunused;

        private LinkAddress m_FirstAddress;

        public LinkFactory()
        {
            m_Block = new List<Node>();
            m_Uunused = new Stack<int>();
            m_FirstAddress = LinkAddress.Default;
        }

        public void Clear()
        {
            m_Block.Clear();
            m_Uunused.Clear();
            m_FirstAddress = LinkAddress.Default;
        }

        public LinkAddress RequireNewLink()
        {
            return new LinkAddress() { first = -1, last = -1 };
        }

        public T[] Gets(in int address)
        {
            List<T> result = new List<T>();
            int currAddress = address;
            while (currAddress != -1)
            {
                Node currNode = m_Block[currAddress];
                if (currNode.value != null)
                {
                    result.Add(currNode.value);
                }
                currAddress = currNode.next;
            }
            return result.ToArray();
        }

        public T GetFirst()
        {
            if (m_FirstAddress.first != -1)
            {
                return m_Block[m_FirstAddress.first].value;
            }
            return default;
        }

        public T GetNext(in int address)
        {
            int index = m_Block[address].next;
            if (index != -1)
            {
                return m_Block[index].value;
            }
            return default;
        }

        public T Get(in int address)
        {
            return m_Block[address].value;
        }

        public int Add(ref LinkAddress linkAddress, in T value)
        {
            int address = RequireAddress();
            Node node = Node.Default;
            node.value = value;
            AddNormalLinkNode(ref linkAddress, address, ref node);
            AddSingleLinkNode(ref linkAddress, address, ref node);
            m_Block[address] = node;
            return address;
        }

        public void Remove(ref LinkAddress linkAddress, in int address)
        {
            Node currNode = m_Block[address];
            RemoveNormalLinkNode(ref linkAddress, currNode);
            RemoveSingleLinkNode(ref linkAddress, currNode);
            ReleaseAddress(address);
        }

        private int RequireAddress()
        {
            if (m_Uunused.Count == 0)
            {
                m_Block.Add(Node.Default);
                return m_Block.Count - 1;
            }
            return m_Uunused.Pop();
        }

        private void ReleaseAddress(in int address)
        {
            m_Block[address] = Node.Default;
            m_Uunused.Push(address);
        }

        private void AddNormalLinkNode(ref LinkAddress linkAddress, in int address, ref Node node)
        {
            if (linkAddress.first == -1 && linkAddress.last == -1)
            {
                linkAddress.first = address;
                linkAddress.last = address;
            }
            else
            {
                int prevAddress = linkAddress.last;
                Node prev = m_Block[prevAddress];
                prev.next = address;
                m_Block[prevAddress] = prev;
                node.prev = prevAddress;
                linkAddress.last = address;
            }
        }

        private void AddSingleLinkNode(ref LinkAddress linkAddress, in int address, ref Node node)
        {
            if (m_FirstAddress.first == -1 && m_FirstAddress.last == -1)
            {
                m_FirstAddress.first = address;
                m_FirstAddress.last = address;
            }
            else
            {
                int prevAddress = m_FirstAddress.last;
                Node prev = m_Block[prevAddress];
                prev.addressNext = address;
                m_Block[prevAddress] = prev;
                node.addressPrev = prevAddress;
                m_FirstAddress.last = address;
            }
        }

        private void RemoveNormalLinkNode(ref LinkAddress linkAddress, in Node currNode)
        {
            int prevAddress = currNode.prev;
            int nextAddress = currNode.next;
            bool hasPrev = prevAddress != -1;
            bool hasNext = nextAddress != -1;
            if (hasPrev && hasNext)
            {
                //curr in mid
                Node prev = m_Block[prevAddress];
                Node next = m_Block[nextAddress];
                prev.next = nextAddress;
                next.prev = prevAddress;
                m_Block[prevAddress] = prev;
                m_Block[nextAddress] = next;
            }
            else if (hasPrev && !hasNext)
            {
                //curr is tail
                Node prev = m_Block[prevAddress];
                prev.next = -1;
                m_Block[prevAddress] = prev;
                linkAddress.last = prevAddress;
            }
            else if (!hasPrev && hasNext)
            {
                //curr is head
                Node next = m_Block[nextAddress];
                next.prev = -1;
                m_Block[nextAddress] = next;
                linkAddress.first = nextAddress;
            }
            else
            {
                //curr is head and tail
                linkAddress.first = -1;
                linkAddress.last = -1;
            }
        }

        private void RemoveSingleLinkNode(ref LinkAddress linkAddress, in Node currNode)
        {
            int prevAddress = currNode.addressPrev;
            int nextAddress = currNode.addressNext;
            bool hasPrev = prevAddress != -1;
            bool hasNext = nextAddress != -1;
            if (hasPrev && hasNext)
            {
                //curr in mid
                Node prev = m_Block[prevAddress];
                Node next = m_Block[nextAddress];
                prev.addressNext = nextAddress;
                next.addressPrev = prevAddress;
                m_Block[prevAddress] = prev;
                m_Block[nextAddress] = next;
            }
            else if (hasPrev && !hasNext)
            {
                //curr is tail
                Node prev = m_Block[prevAddress];
                prev.addressNext = -1;
                m_Block[prevAddress] = prev;
                linkAddress.last = prevAddress;
            }
            else if (!hasPrev && hasNext)
            {
                //curr is head
                Node next = m_Block[nextAddress];
                next.addressPrev = -1;
                m_Block[nextAddress] = next;
                linkAddress.first = nextAddress;
            }
            else
            {
                //curr is head and tail
                linkAddress.first = -1;
                linkAddress.last = -1;
            }
        }
    }
}