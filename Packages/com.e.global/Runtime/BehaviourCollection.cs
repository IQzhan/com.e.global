using System;
using System.Collections;
using System.Collections.Generic;

namespace E
{
    internal class BehaviourCollection : IEnumerable<GlobalBehaviour>, IEnumerable
    {
        public BehaviourCollection()
        {
            linkFactory = new LinkFactory<GlobalBehaviour>();
            typeLinks = new SortedList<int, LinkFactory<GlobalBehaviour>.Address>();
        }

        private readonly LinkFactory<GlobalBehaviour> linkFactory;

        private readonly SortedList<int, LinkFactory<GlobalBehaviour>.Address> typeLinks;

        public GlobalBehaviour this[in int id]
        { get => linkFactory.Get(id); }

        public void Add(in GlobalBehaviour value)
        {
            Type type = value.GetType();
            int hashCode = type.GetHashCode();
            GetTypeLink(hashCode, out LinkFactory<GlobalBehaviour>.Address typelink);
            value.ID = linkFactory.Add(ref typelink, value);
            SetTypeLink(hashCode, typelink);
        }

        public void Remove(in GlobalBehaviour value)
        {
            Type type = value.GetType();
            int hashCode = type.GetHashCode();
            GetTypeLink(hashCode, out LinkFactory<GlobalBehaviour>.Address typelink);
            linkFactory.Remove(ref typelink, value.ID);
            SetTypeLink(hashCode, typelink);
        }

        public GlobalBehaviour Get(in Type type)
        {
            int hashCode = type.GetHashCode();
            GetTypeLink(hashCode, out LinkFactory<GlobalBehaviour>.Address typelink);
            return linkFactory.Get(typelink.first);
        }

        public GlobalBehaviour[] Gets(in Type type)
        {
            int hashCode = type.GetHashCode();
            GetTypeLink(hashCode, out LinkFactory<GlobalBehaviour>.Address typelink);
            return linkFactory.Gets(typelink.first);
        }

        public void Clear()
        {
            linkFactory.Clear();
            typeLinks.Clear();
        }

        private void GetTypeLink(in int hashCode, out LinkFactory<GlobalBehaviour>.Address typelink)
        {
            if (!typeLinks.TryGetValue(hashCode, out typelink))
            {
                typelink = linkFactory.RequireNewLink();
                typeLinks.Add(hashCode, typelink);
            }
        }

        private void SetTypeLink(in int hashCode, in LinkFactory<GlobalBehaviour>.Address typelink)
        {
            typeLinks[hashCode] = typelink;
        }

        #region Enumerator
        public struct Enumerator : IEnumerator<GlobalBehaviour>, IEnumerator, IDisposable
        {
            internal LinkFactory<GlobalBehaviour> body;

            internal GlobalBehaviour current;

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
                    current = body.GetAddressFirst();
                }
                else
                {
                    current = body.GetAddressNext(current.ID);
                }
                return current != null;
            }

            public void Dispose()
            {
                current = null;
                body = null;
            }
        }

        public IEnumerator<GlobalBehaviour> GetEnumerator()
        {
            return new Enumerator() { body = linkFactory, current = null };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator { body = linkFactory, current = null };
        }
        #endregion

        internal class LinkFactory<T>
        {
            private struct Node
            {
                public static Node Default { get => new Node() { value = default, prev = -1, next = -1, addressPrev = -1, addressNext = -1 }; }

                public T value;

                public int prev;

                public int next;

                public int addressPrev;

                public int addressNext;
            }

            internal struct Address
            {
                public static Address Default { get => new Address() { first = -1, last = -1 }; }

                public int first;

                public int last;
            }

            private interface INodeSetter
            {
                void SetPrev(ref Node node, in int addr);

                void SetNext(ref Node node, in int addr);

                int GetPrev(in Node node);

                int GetNext(in Node node);
            }

            private struct NodeSetter : INodeSetter
            {
                public int GetNext(in Node node)
                {
                    return node.next;
                }

                public int GetPrev(in Node node)
                {
                    return node.prev;
                }

                public void SetNext(ref Node node, in int addr)
                {
                    node.prev = addr;
                }

                public void SetPrev(ref Node node, in int addr)
                {
                    node.next = addr;
                }
            }

            private struct MainNodeSetter : INodeSetter
            {
                public int GetNext(in Node node)
                {
                    return node.addressNext;
                }

                public int GetPrev(in Node node)
                {
                    return node.addressPrev;
                }

                public void SetNext(ref Node node, in int addr)
                {
                    node.addressPrev = addr;
                }

                public void SetPrev(ref Node node, in int addr)
                {
                    node.addressNext = addr;
                }
            }

            private readonly List<Node> m_Block;

            private readonly Stack<int> m_Uunused;

            private Address m_Main;

            private NodeSetter m_NodeSetter;

            private MainNodeSetter m_MainNodeSetter;

            private List<T> m_HelperList;

            public LinkFactory()
            {
                m_Block = new List<Node>();
                m_Uunused = new Stack<int>();
                m_Main = Address.Default;
                m_NodeSetter = new NodeSetter();
                m_MainNodeSetter = new MainNodeSetter();
                m_HelperList = new List<T>();
            }

            public void Clear()
            {
                m_Block.Clear();
                m_Uunused.Clear();
                m_Main = Address.Default;
                m_HelperList.Clear();
            }

            public Address RequireNewLink()
            {
                return Address.Default;
            }

            public T[] Gets(in int address)
            {
                m_HelperList.Clear();
                int currAddress = address;
                while (currAddress != -1)
                {
                    Node currNode = m_Block[currAddress];
                    if (currNode.value != null)
                    {
                        m_HelperList.Add(currNode.value);
                    }
                    currAddress = currNode.next;
                }
                T[] array = m_HelperList.ToArray();
                return array;
            }

            public T GetAddressFirst()
            {
                if (m_Main.first != -1)
                {
                    return m_Block[m_Main.first].value;
                }
                return default;
            }

            public T GetAddressNext(in int address)
            {
                int index = m_Block[address].addressNext;
                if (index != -1)
                {
                    return m_Block[index].value;
                }
                return default;
            }

            public T Get(in int address)
            {
                if (address != -1)
                {
                    return m_Block[address].value;
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

            public int Add(ref Address linkAddress, in T value)
            {
                int address = RequireAddress();
                Node node = Node.Default;
                node.value = value;
                AddLinkNode(ref linkAddress, address, ref node, m_NodeSetter);
                AddLinkNode(ref m_Main, address, ref node, m_MainNodeSetter);
                m_Block[address] = node;
                return address;
            }

            public void Remove(ref Address linkAddress, in int address)
            {
                Node currNode = m_Block[address];
                RemoveLinkNode(ref linkAddress, currNode, m_NodeSetter);
                RemoveLinkNode(ref m_Main, currNode, m_MainNodeSetter);
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

            private void AddLinkNode<Setter>(ref Address linkAddress, in int address, ref Node node,
                in Setter setter) where Setter : struct, INodeSetter
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
                    setter.SetNext(ref prev, address);
                    m_Block[prevAddress] = prev;
                    setter.SetPrev(ref node, prevAddress);
                    linkAddress.last = address;
                }
            }

            private void RemoveLinkNode<Setter>(ref Address linkAddress, in Node currNode,
                in Setter setter) where Setter : struct, INodeSetter
            {
                int prevAddress = setter.GetPrev(currNode);
                int nextAddress = setter.GetNext(currNode);
                bool hasPrev = prevAddress != -1;
                bool hasNext = nextAddress != -1;
                if (hasPrev && hasNext)
                {
                    //curr in mid
                    Node prev = m_Block[prevAddress];
                    Node next = m_Block[nextAddress];
                    setter.SetNext(ref prev, nextAddress);
                    setter.SetPrev(ref next, prevAddress);
                    m_Block[prevAddress] = prev;
                    m_Block[nextAddress] = next;
                }
                else if (hasPrev && !hasNext)
                {
                    //curr is tail
                    Node prev = m_Block[prevAddress];
                    setter.SetNext(ref prev, -1);
                    m_Block[prevAddress] = prev;
                    linkAddress.last = prevAddress;
                }
                else if (!hasPrev && hasNext)
                {
                    //curr is head
                    Node next = m_Block[nextAddress];
                    setter.SetPrev(ref next, -1);
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
}