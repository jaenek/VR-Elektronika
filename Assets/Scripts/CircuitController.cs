using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MathNet.Numerics.LinearAlgebra;

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
        public int id;

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
                    LastNode().id = nodes.Count-1;
				}
				else {
        			LastNode().connections.Add(conn);
					conn.connected = true;
                    DrawNode(LastNode());
                    TrySolve();
				}
			}
			else
			{
				nodes.Add(new Node(conn));
                LastNode().id = nodes.Count-1;
			}
		}
    }

    public void DiscardConnection()
    {
        if (LastNode().connections.Count < 2) {
            nodes.Remove(LastNode());
        }
    }

    private bool Solveable()
    {
        foreach (var node in nodes)
        {
            foreach (var conn in node.connections)
            {
                if (conn.element.GetFreeConnection() != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private (List<ItemClass> components, int sources) GetCircuitComponents()
    {
        var components = new List<ItemClass>();
        var sources = 0;

        foreach (var node in nodes)
        {
            foreach (var conn in node.connections)
            {
                if (!components.Contains(conn.element))
                {
                    components.Add(conn.element);
                    if (conn.element.type == ItemClass.ItemType.Source)
                    {
                        sources++;
                    }
                }

                if (conn.type == ConnectionClass.ConnectionType.Plus)
                {
                    conn.element.high = node.id;
                }
                else if (conn.type == ConnectionClass.ConnectionType.Minus)
                {
                    conn.element.low = node.id;
                }
                else
                {
                    if (conn.element.low == -1)
                    {
                        conn.element.low = node.id;
                    }
                    else if (conn.element.high == -1)
                    {
                        conn.element.high = node.id;
                    }
                }
            }
        }

        return (components, sources);
    }

    public (Matrix<float> A, Vector<float> b) CalculateMatrices(List<ItemClass> components, int sources)
    {
        var matrixSize = nodes.Count + sources - 1;

        var A = Matrix<float>.Build.Dense(matrixSize, matrixSize);
        var b = Vector<float>.Build.Dense(matrixSize);

        var index = matrixSize - sources;

        foreach (var component in components)
        {
            Debug.Log(component.type + " high: " + component.high + " low: " + component.low);
            var high = component.high;
            var low = component.low;

            if (component.type == ItemClass.ItemType.Resistor)
            {
                var value = component.GetComponent<ResistorClass>().resistance;

                if (high != 0)
                {
                    A[high - 1, high - 1] += 1/value;
                }
                if (low != 0)
                {
                    A[low - 1, low - 1] += 1/value;
                }

                if (high != 0 && low != 0)
                {
                    A[high - 1, low - 1] -= 1/value;
                    A[low - 1, high - 1] -= 1/value;
                }
            }
            else if (component.type == ItemClass.ItemType.Source)
            {
                var value = component.GetComponent<SourceClass>().voltage;

                if (high != 0)
                {
                    A[high - 1, index] += 1;
                    A[index, high - 1] += 1;
                }
                if (low != 0)
                {
                    A[low - 1, index] -= 1;
                    A[index, low - 1] -= 1;
                }

                b[index] = value;

                index = index + 1;
            }
        }

        return (A, b);
    }

    public void TrySolve()
    {
        if (Solveable())
        {
            var (components, sources) = GetCircuitComponents();

            var (A, b) = CalculateMatrices(components, sources);
            Debug.Log("A:\n" + A);
            Debug.Log("b:\n" + b);

            Debug.Log("SOLVING");
            var x = A.Solve(b);

            Debug.Log(x);
        }
    }
}