using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClass : MonoBehaviour
{
    public Sprite itemImage;
    public string itemName;
    public Vector3Int size;
    public Vector3 originTransform = Vector3.down;
	public List<ConnectionClass> connections;

	public ConnectionClass GetClosestConnection()
	{
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 4))
        {
			var closest = connections[0];
			var closestDistance = Vector3.Distance(connections[0].transform.position, hit.point);
			foreach (var conn in connections.GetRange(1, connections.Count-1))
			{
				var dist = Vector3.Distance(conn.transform.position, hit.point);
				if (dist < closestDistance)
				{
					closest = conn;
				}
				closestDistance = dist;
			}

			return closest;
        }
		else
		{
			foreach (var conn in connections)
			{
				if (!conn.connected)
				{
					return conn;
				}
			}
		}

		return null;
	}
}
