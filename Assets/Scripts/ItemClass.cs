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

	public ConnectionClass GetFreeConnection()
	{
		foreach (var conn in connections)
		{
			if (!conn.connected)
			{
				return conn;
			}
		}
		return null;
	}
}
