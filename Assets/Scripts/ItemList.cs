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
    public GameObject selection;
    public GameObject player;

    void Start()
    {
        var i = 0;
        itemList.ForEach(item => //Add every items to inventory UI
        {
            var itemClass = item.GetComponent<ItemClass>(); //Get informations about item

            GameObject newItem = Instantiate(prefabItem); //Create new object
            newItem.GetComponent<ItemSelection>().index = i; //Set index to know which button is clicked
            newItem.transform.GetChild(1).GetComponent<Image>().sprite = itemClass.itemImage; //Set image for button
            newItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemClass.itemName; //Set text for button
            newItem.transform.SetParent(layout.transform,false);

            i++;
        });
    }

    public void OnItemSelected(int index) //Fires when user select an object to place
    {
        if(selection.transform.childCount > 2)
        {
            Destroy(selection.transform.GetChild(2).gameObject); //Destroy previous selection object
        }
        var selectionController = selection.GetComponent<SelectionController>();
        selectionController.ToggleState(true); //Show placeholder
        var playerController = player.GetComponent<PlayerController>();
        playerController.OpenInventory(); //Hide inventory
        GameObject selectedItem = Instantiate(itemList[index]); //Create new object in scene
        selectionController.SetPlaceholderSize(selectedItem.GetComponent<ItemClass>().size);
        selectedItem.transform.position = selection.transform.position; //Set the same postion as parent
        selectedItem.transform.rotation = selection.transform.rotation; //Set the same rotation as parent
        selectedItem.transform.SetParent(selection.transform); // Add this object to selection plane
        playerController.ChangeState(true, PlayerController.State.isPlacing); //Change state to placing item

    }
}
