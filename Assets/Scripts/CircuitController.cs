using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CircuitController : MonoBehaviour
{
    public static CircuitController instance;
    public Material connectionMaterial;

    void Awake()
    {
        instance = this;
    }

    private class Node
    {
        public List<ConnectionClass> connections = new List<ConnectionClass>();
        public List<LineRenderer> lines = new List<LineRenderer>();

        public Node(ConnectionClass conn)
        {
           	connections.Add(conn);
			conn.connected = true;
        }
        public LineRenderer LastLine()
        {
            return lines[lines.Count-1];
        }
    }

    private static List<Node> nodes = new List<Node>();

    private Node LastNode()
    {
        return nodes[nodes.Count-1];
    }

    private void DrawLine(Node node, Vector3 pos1, Vector3 pos2)
    {
        node.lines.Add(new GameObject("Line" + (node.lines.Count + 1)).AddComponent<LineRenderer>());
        node.LastLine().material = connectionMaterial;
        node.LastLine().positionCount = 2;
        node.LastLine().startWidth = 0.02f;
        node.LastLine().endWidth = 0.02f;
        node.LastLine().useWorldSpace = true;
        node.LastLine().numCapVertices = 5;
        node.LastLine().SetPosition(0, pos1);
        node.LastLine().SetPosition(1, pos2);
    }

    private void DrawNode(Node node)
    {
        if (node.connections.Count == 2)
        {
            DrawLine(node, node.connections[0].transform.position, node.connections[1].transform.position);
        }
        else
        {
            Vector3 middle = (node.connections[0].transform.position + node.connections[1].transform.position)/2;
            DrawLine(node, middle, node.connections[node.connections.Count-1].transform.position);
        }
    }

    public void AddConnection(ItemClass element, bool newConnection)
    {
		var conn = element.GetClosestConnection();
		if (conn != null)
		{
			if (nodes.Count > 0)
			{
				if (LastNode().connections.Contains(conn) || newConnection)
				{
        			nodes.Add(new Node(conn));
				}
				else {
        			LastNode().connections.Add(conn);
					conn.connected = true;
                    DrawNode(LastNode());
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