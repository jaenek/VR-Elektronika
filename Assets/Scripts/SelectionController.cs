using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public static SelectionController instance;

    void Awake() {
        instance = this;
    }

    public void PointTo(Vector3 pos) {
        pos = new Vector3(
            Mathf.Floor(pos.x / 0.5f) * 0.5f + 0.25f,
            0.01f, // so it doesn't clip the plane
            Mathf.Floor(pos.z / 0.5f) * 0.5f + 0.25f
        );

        Debug.Log("pointing to (" + pos.x + ", " + pos.z + ")");

        transform.position = pos;
    }
}
