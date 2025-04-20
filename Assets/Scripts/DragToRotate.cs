using UnityEngine;
public class DragToRotate : MonoBehaviour
{
    public float speed = 200f;
    private Vector3 lastMousePos;

    void OnMouseDown()
    {
        lastMousePos = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - lastMousePos;
        // Rotate around Y-axis based on horizontal drag
        transform.Rotate(Vector3.up, -delta.x * speed * Time.deltaTime, Space.World);
        // Optional: Rotate around X-axis for vertical drag
        transform.Rotate(Vector3.right, delta.y * speed * Time.deltaTime, Space.World);
        lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.deltaPosition;
                transform.Rotate(Vector3.up, -delta.x * speed * Time.deltaTime, Space.World);
                transform.Rotate(Vector3.right, delta.y * speed * Time.deltaTime, Space.World);
            }
        }
    }

}


