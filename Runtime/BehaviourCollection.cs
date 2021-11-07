using System;
using System.Collections.Generic;

namespace E
{
    internal class BehaviourCollection
    {
        private struct Node
        {
            public GlobalBehaviour value;

            public int prev;

            public int next;
        }

        private struct TypeNode
        {
            public int head;

            public int tail;
        }

        public BehaviourCollection()
        {
            link = new List<Node>();
            unusedIndexes = new Stack<int>();
            typeNodes = new SortedList<Type, TypeNode>();
        }

        private readonly List<Node> link;

        private readonly Stack<int> unusedIndexes;

        private readonly SortedList<Type, TypeNode> typeNodes;

        private void GetTypeNode(in Type type, out TypeNode typeNode)
        {
            if (!typeNodes.TryGetValue(type, out typeNode))
            {
                typeNode.head = -1;
                typeNode.tail = -1;
                typeNodes[type] = typeNode;
            }
        }

        private void SetTypeNode(in Type type, in TypeNode typeNode)
        {
            typeNodes[type] = typeNode;
        }

        public void Add(in GlobalBehaviour value)
        {
            Type type = value.GetType();
            GetTypeNode(type, out TypeNode typeNode);
            Add(ref typeNode, value);
            SetTypeNode(type, typeNode);
        }

        public void Remove(in GlobalBehaviour value)
        {
            Type type = value.GetType();
            GetTypeNode(type, out TypeNode typeNode);
            Remove(ref typeNode, value);
            SetTypeNode(type, typeNode);
        }

        public GlobalBehaviour Get(in Type type)
        {
            GetTypeNode(type, out TypeNode typeNode);
            if (typeNode.head == -1) return null;
            return link[typeNode.head].value;
        }

        public GlobalBehaviour[] Gets(in Type type)
        {
            GetTypeNode(type, out TypeNode typeNode);
            List<GlobalBehaviour> result = new List<GlobalBehaviour>();
            int currAddress = typeNode.head;
            while (currAddress != -1)
            {
                Node currNode = link[currAddress];
                if (currNode.value != null)
                {
                    result.Add(currNode.value);
                }
                currAddress = currNode.next;
            }
            return result.ToArray();
        }

        private void Add(ref TypeNode typeNode, in GlobalBehaviour value)
        {
            int address = RequireAddress();
            Node node = new Node();
            node.value = value;
            node.prev = -1;
            node.next = -1;
            if (typeNode.head == -1 && typeNode.tail == -1)
            {
                typeNode.head = address;
                typeNode.tail = address;
            }
            else
            {
                int prevAddress = typeNode.tail;
                Node prev = link[prevAddress];
                prev.next = address;
                link[prevAddress] = prev;
                node.prev = prevAddress;
                typeNode.tail = address;
            }
            link[address] = node;
            return;
        }

        private void Remove(ref TypeNode typeNode, in GlobalBehaviour value)
        {
            if (typeNode.head == -1 && typeNode.tail == -1)
            {
                return;
            }
            int currAddress = typeNode.head;
            while (currAddress != -1)
            {
                Node currNode = link[currAddress];
                if (currNode.value == value)
                {
                    int prevAddress = currNode.prev;
                    int nextAddress = currNode.next;
                    bool hasPrev = prevAddress != -1;
                    bool hasNext = nextAddress != -1;
                    if (hasPrev && hasNext)
                    {
                        //curr in mid
                        Node prev = link[prevAddress];
                        Node next = link[nextAddress];
                        prev.next = nextAddress;
                        next.prev = prevAddress;
                        link[prevAddress] = prev;
                        link[nextAddress] = next;
                    }
                    else if (hasPrev && !hasNext)
                    {
                        //curr is tail
                        Node prev = link[prevAddress];
                        prev.next = -1;
                        link[prevAddress] = prev;
                        typeNode.tail = prevAddress;
                    }
                    else if (!hasPrev && hasNext)
                    {
                        //curr is head
                        Node next = link[nextAddress];
                        next.prev = -1;
                        link[nextAddress] = next;
                        typeNode.head = nextAddress;
                    }
                    else
                    {
                        //curr is head and tail
                        typeNode.head = -1;
                        typeNode.tail = -1;
                    }
                    ReleaseAddress(currAddress);
                    return;
                }
                currAddress = currNode.next;
            }
        }

        private int RequireAddress()
        {
            if (unusedIndexes.Count == 0)
            {
                link.Add(default);
                return link.Count - 1;
            }
            return unusedIndexes.Pop();
        }

        private void ReleaseAddress(int address)
        {
            link[address] = default;
            unusedIndexes.Push(address);
        }
    }
}