using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public float speed = 2f;
    public float acceleration = -0.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(speed < 0)
            speed = 0;
        else
            speed += acceleration * Time.deltaTime;
    }
}
