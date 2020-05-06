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
        public List<ConnectionClass> connections = new List<ConnectionClass>();

        public Node(ConnectionClass conn)
        {
           	connections.Add(conn);
			conn.connected = true;
        }
    }

    private static List<Node> nodes = new List<Node>();

    private Node LastNode()
    {
        return nodes[nodes.Count-1];
    }

    public void AddConnection(ItemClass element)
    {
		var conn = element.GetFreeConnection();
		if (conn != null)
		{
			if (nodes.Count > 0)
			{
				if (LastNode().connections.Contains(conn))
				{
        			nodes.Add(new Node(conn));
				}
				else {
        			LastNode().connections.Add(conn);
					conn.connected = true;
				}
			}
			else
			{
				nodes.Add(new Node(conn));
			}
		}
    }

    public void DiscardConnection()
    {
        if (LastNode().connections.Count < 2) {
            nodes.Remove(LastNode());
        }
    }
}