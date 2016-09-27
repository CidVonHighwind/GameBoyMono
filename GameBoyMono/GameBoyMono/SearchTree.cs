using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    class SearchTree
    {
        List<Node> top = new List<Node>();

        public void AddItem(byte[] tileData, int number)
        {
            for (int i = 0; i < top.Count; i++)
            {
                if (top[i].data == tileData[0])
                {
                    top[i].AddItem(tileData, 1, number);
                    return;
                }
            }

            Node newNode = new Node(tileData[0]);
            top.Add(newNode);
            top[top.Count - 1].AddItem(tileData, 1, number);
        }

        public int Search(byte[] tileData)
        {
            for (int i = 0; i < top.Count; i++)
            {
                if (top[i].data == tileData[0])
                    return top[i].searchTree(tileData, 1);
            }

            return -1;
        }
    }

    class Node
    {
        public List<Node> nodes = new List<Node>();
        public int data;

        public Node(int _data)
        {
            data = _data;
        }

        public void AddItem(byte[] tileData, int currentState, int _data)
        {
            if (tileData.Length == currentState)
            {
                Node newNode = new Node(_data);
                nodes.Add(newNode);
            }
            else
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    // node already exists
                    if (nodes[i].data == tileData[currentState])
                    {
                        nodes[i].AddItem(tileData, currentState + 1, _data);
                        return;
                    }
                }

                // add the node if it does not exist
                Node newNode = new Node(tileData[currentState]);
                nodes.Add(newNode);
                nodes[nodes.Count - 1].AddItem(tileData, currentState + 1, _data);
            }
        }

        public int searchTree(byte[] tileData, int state)
        {
            if (tileData.Length == state)
            {
                if (nodes[0].nodes.Count == 0)
                    return nodes[0].data;
            }
            else
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].data == tileData[state])
                    {
                        return nodes[i].searchTree(tileData, state + 1);
                    }
                }
            }

            return -1;
        }
    }
}
