using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    public List<GameObject> itemList;
    public GameObject prefabItem;
    public GameObject layout;

    void Start()
    {
        itemList.ForEach(item =>
        {
            var itemClass = item.GetComponent<ItemClass>();

            GameObject newItem = Instantiate(prefabItem);
            var newItemImage = newItem.transform.GetChild(2);
            var newItemImageComponent = newItemImage.GetComponent<Image>();
            var newItemName = newItem.transform.GetChild(3);
            var newItemTextComponent = newItemName.GetComponent<TextMeshProUGUI>();
            newItemImageComponent.sprite = itemClass.itemImage;
            newItemTextComponent.text = itemClass.itemName;
            newItem.transform.SetParent(layout.transform,false);
        });
    }
}
