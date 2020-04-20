using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderState : MonoBehaviour
{
    public bool isSomethingWithin;
    public bool isSelectionPlaneWithin;
    public GameObject objectIn;

    private void OnTriggerStay(Collider other) //Fires when rigitbody get into collider
    {
        if(other.CompareTag("Placed")) //Only items with tag "Placed" can affect
        {
            objectIn = other.transform.parent.gameObject;
            isSomethingWithin = true;
        }
        else if(other.CompareTag("SelectionPlane")) //If selection plane is in collider
        {
            isSelectionPlaneWithin = true;
        }
    }

    private void OnTriggerExit(Collider other) //Fires when rigitbody exit collider
    {
        objectIn = null;
        if (other.CompareTag("Placed")) //Only items with tag "Placed" can affect
        {
            isSomethingWithin = false;
        }
        else if (other.CompareTag("SelectionPlane")) //If selection plane is in collider
        {
            isSelectionPlaneWithin = false;
        }
    }
}
