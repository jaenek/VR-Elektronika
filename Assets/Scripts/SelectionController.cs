using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public static SelectionController instance;
    public GameObject placeholderOrigin;
    public Vector3 defaultPlaceholderSize;

    void Awake() {
        instance = this;
        defaultPlaceholderSize = placeholderOrigin.transform.localScale;
    }

    public void PointTo(Vector3 pos) {
        pos = new Vector3(
            Mathf.Floor(pos.x / 0.5f) * 0.5f + 0.25f,
            0.01f, // so it doesn't clip the plane
            Mathf.Floor(pos.z / 0.5f) * 0.5f + 0.25f
        );

        transform.position = pos;
    }

    public void Toggle() //Toggle visibility of placeholder
    {
        placeholderOrigin.SetActive(!placeholderOrigin.activeSelf);
    }

    public void ToggleState(bool state) //Set visibility of placeholder
    {
        placeholderOrigin.SetActive(state);
    }

    public void SetPlaceholderSize(Vector3 size) //Set placeholder size
    {
        placeholderOrigin.transform.localScale = Vector3.Scale(size,defaultPlaceholderSize);
    }

    public void RotateSelectionBy(int degrees) // Add degrees to actual rotation in y axis
    {
        transform.Rotate(new Vector3(0, 1, 0), degrees);
    }

    public void RotateSelectionTo(float x, float y, float z) //Set specific rotation angle
    {
        transform.eulerAngles = new Vector3(x,y,z);
    }

    public void PlaceItem() //Place item and hide placeholder
    {
        var item = transform.GetChild(2);
        item.GetComponent<ItemClass>().originTransform = placeholderOrigin.transform.position; //sets object origin point to actual selection plane origin
        item.GetChild(0).tag = "Placed"; //assign tag to object for verify in ColliderState
        foreach (var conn in item.GetComponent<ItemClass>().connections)
        {
            conn.element = item.GetComponent<ItemClass>();
        }
        item.parent = null; //detach object
        ToggleState(false);
    }

    public void DiscardPlacing() //Destroy selected object and hide placeholder
    {
        Destroy(transform.GetChild(2).gameObject);
        ToggleState(false);
    }
}
