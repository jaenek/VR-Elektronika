using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitController
{
    public static CircuitController instance;

    private class Connection
    {
        public GameObject input;
        public List<GameObject> output;

        public Connection(GameObject obj)
        {
            input = obj;
        }

        public void AddOutput(GameObject obj)
        {
            output.Add(obj);
        }
    }
    
    private static List<Connection> Connections;

    public static void AddConnection(GameObject obj)
    {
        Connections.Add(new Connection(obj));
    }

    public static void DiscardConnection()
    {
        Connection last = Connections[Connections.Count-1];

        if (last.output.Count == 0) {
            Connections.Remove(last);
        }
    }
}