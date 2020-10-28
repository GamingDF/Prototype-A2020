using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDirection : MonoBehaviour
{
    public Camera cam;
    public Rigidbody2D Player_rb;

    Vector2 __mousePos;

    void Update()
    {
        __mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        Vector2 lookDir = __mousePos - Player_rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }
}
