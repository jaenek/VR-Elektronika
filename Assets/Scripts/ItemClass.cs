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
    public enum Type { Resistor, Power_Supply, Capacitor, Engine, Switch, Other};
    public Type itemType;
    public float resistance;
    public float voltage;
    public float capacity;
}