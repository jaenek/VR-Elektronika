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

	public int id;
	public ConnectionType type;
	public float current;
	public float voltage;
	public bool connected = false;
}

