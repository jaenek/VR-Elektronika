using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitController : MonoBehaviour
{
    public static CircuitController instance;

    void Awake()
    {
        instance = this;
    }

    private class Node
    {
        public List<GameObject> elements = new List<GameObject>();

        public Node(ref GameObject obj)
        {
            elements.Add(obj);
        }
    }
    
    private static List<Node> Nodes = new List<Node>();

    private void AddNode(ref GameObject element)
    {
        Nodes.Add(new Node(ref element));
    }

    private Node LastNode()
    {
        return Nodes[Nodes.Count-1];
    }

    private void AddElementToLastNode(ref GameObject element)
    {
        LastNode().elements.Add(element);
    }

    public void AddConnection(ref GameObject element)
    {
        if (Nodes.Count > 0)
        {
            if (LastNode().elements.Contains(element))
            {
                AddNode(ref element);
            }
            else {
                AddElementToLastNode(ref element);
            }
        }
        else
        {
            AddNode(ref element);
        }
    }

    public void DiscardConnection()
    {
        if (LastNode().elements.Count == 1) {
            Nodes.Remove(LastNode());
        }
    }
}