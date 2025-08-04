using UnityEngine;

public class CursorHand : MonoBehaviour
{
    void Update()
    {
        Vector3 mousPos = GetMouseWorldPosition();
        mousPos.y = 10f;
        transform.position = mousPos;

    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        // mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        // mousePoint.y = 0;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
