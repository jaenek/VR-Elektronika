using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public static SelectionController instance;
    public GameObject placeholderOrigin;
    private Vector3 defaultPlaceholderSize;

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

    public void PlaceItem() //Place item and hide placeholder
    {
        var item = transform.GetChild(1);
        item.GetChild(0).tag = "Placed";
        item.parent = null;
        ToggleState(false);
    }

    public void DiscardPlacing() //Destroy selected object and hide placeholder
    {
        Destroy(transform.GetChild(1).gameObject);
        ToggleState(false);
    }
}
