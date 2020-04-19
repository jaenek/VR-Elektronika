using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelection : MonoBehaviour
{
    public GameObject item;
    public int index;
    public void OnItemSelectedChild()
    {
        item.transform.GetComponentInParent<ItemList>().OnItemSelected(index);
    }
}
