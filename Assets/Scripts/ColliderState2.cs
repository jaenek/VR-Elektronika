using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderState2 : MonoBehaviour
{
    public GameObject objectIn = null;
    private void OnTriggerStay(Collider other) //Fires when rigitbody get into collider
    {
        if(other.CompareTag("Placed"))
        {
            objectIn = other.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) //Fires when rigitbody exit collider
    {
        if (other.CompareTag("Placed"))
        {
            objectIn = null;
        }
    }
}
