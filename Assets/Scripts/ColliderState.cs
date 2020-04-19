using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderState : MonoBehaviour
{
    public bool isSomethingWithin;

    private void OnTriggerStay(Collider other) //Fires when rigitbody get into collider
    { 
        if(other.CompareTag("Placed")) //Only items with tag "Placed" can affect
        {
            isSomethingWithin = true;
        }
    }

    private void OnTriggerExit(Collider other) //Fires when rigitbody exit collider
    {
        if (other.CompareTag("Placed")) //Only items with tag "Placed" can affect
        {
            isSomethingWithin = false;
        }
    }
}
