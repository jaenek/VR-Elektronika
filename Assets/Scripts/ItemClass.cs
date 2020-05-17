using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemClass : MonoBehaviour
{

    public Sprite itemImage;
    public string itemName;
	public Vector3Int size;
    [HideInInspector]
	public Vector3 originTransform = Vector3.down;
	public List<ConnectionClass> connections;
	public int high = -1;
	public int low = -1;
    public enum Type { Resistor, Power_Supply, Capacitor, Engine, Switch, Diode, Other};
    public Type itemType = Type.Other;
    public float resistance;
    public float voltage;
    public float capacity;
	public bool on = false;

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

		return GetFreeConnection();
	}

	public void SwitchOn()
	{
		switch (itemType)
		{
			case Type.Switch:			
				AnimationController animationController = GetComponent<AnimationController>();
				on = !on;
				animationController.SwitchState(on);
				if (itemName == "Button")
				{
					StartCoroutine(OffAfterDelay(2));
				}
				break;

			case Type.Diode:
				on = !on;
				transform.GetChild(0).GetChild(2).gameObject.SetActive(on);
				break;

		}
	}

	IEnumerator OffAfterDelay(float time)
	{
		yield return new WaitForSeconds(time);
		on = !on;
		GetComponent<AnimationController>().SwitchState(on);
	}
}
