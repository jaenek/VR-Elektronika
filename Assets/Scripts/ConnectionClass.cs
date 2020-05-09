using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionClass : MonoBehaviour
{
	public enum ConnectionType
	{
		Plus,
		Minus,
		Pin1,
		Pin2,
		Pin3,
		Pin4
	}

	public ItemClass element;
	public ConnectionType type;
	public bool connected = false;
}

