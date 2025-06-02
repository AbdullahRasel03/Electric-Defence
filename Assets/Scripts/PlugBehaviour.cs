using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugBehaviour : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private float fixedYPosition; // Store the initial Y position

    void Start()
    {
        fixedYPosition = transform.position.y; // Store the initial Y position
    }

    void Update()
    {
        if (isDragging)
        {
            // Get the mouse position in world space
            Vector3 mousePosition = GetMouseWorldPosition();
            // Apply the offset and keep the Y position fixed
            transform.position = new Vector3(mousePosition.x - offset.x, fixedYPosition, mousePosition.z - offset.z);
        }
    }

    void OnMouseDown()
    {
        // Calculate the offset between the object's position and the mouse position
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = mousePosition - transform.position;
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position on the screen and convert it to world space
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}