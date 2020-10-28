using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public float speed = 2f;
    public float acceleration = -0.5f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector2(-rb.velocity.x, -rb.velocity.y) * speed, ForceMode2D.Impulse);
        Debug.Log("X: " + rb.velocity.x + " Y: " + rb.velocity.y);
        if (rb.velocity.x < 0.05 && rb.velocity.x > -0.05)
        {
            if (rb.velocity.y < 0.05 && rb.velocity.y > -0.05)
            {
                Destroy(gameObject);
            }
        }
    }
}
