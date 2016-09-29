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
        Node head = new Node(-1);

        public bool AddItem(byte[] tileData, int number)
        {
            return head.AddItem(tileData, 0, number);
        }

        public int Search(byte[] tileData)
        {
            return head.searchTree(tileData, 0);
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

        public bool AddItem(byte[] tileData, int currentState, int _data)
        {
            if (tileData.Length == currentState)
            {
                Node newNode = new Node(_data);
                nodes.Add(newNode);
                return true;
            }
            else
            {
                // node already exists?
                for (int i = 0; i < nodes.Count; i++)
                    if (nodes[i].data == tileData[currentState])
                        return nodes[i].AddItem(tileData, currentState + 1, _data);
                
                // add the node if it does not exist
                Node newNode = new Node(tileData[currentState]);
                nodes.Add(newNode);
                return nodes[nodes.Count - 1].AddItem(tileData, currentState + 1, _data);
            }
        }

        public int searchTree(byte[] tileData, int state)
        {
            if (tileData.Length == state)
            {
                // return the value found
                if (nodes[0].nodes.Count == 0)
                    return nodes[0].data;
            }
            else
            {
                // search for the next value
                for (int i = 0; i < nodes.Count; i++)
                    if (nodes[i].data == tileData[state])
                        return nodes[i].searchTree(tileData, state + 1);
                    
            }

            return -1;
        }
    }
}
