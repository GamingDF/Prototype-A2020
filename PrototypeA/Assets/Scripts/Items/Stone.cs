using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public float speed = 0.1f;
    public float acceleration = -0.5f;

    Rigidbody2D rb;
    float __minRate = 0.01f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector2(-rb.velocity.x, -rb.velocity.y) * speed, ForceMode2D.Impulse);
        Debug.Log("X: " + rb.velocity.x + " Y: " + rb.velocity.y);
        if (rb.velocity.x < __minRate && rb.velocity.x > -__minRate)
        {
            if (rb.velocity.y < __minRate && rb.velocity.y > -__minRate)
            {
                rb.velocity = new Vector3(0, 0, 0);
                GetComponent<Collider2D>().isTrigger = true;
            }
        }
    }
}
