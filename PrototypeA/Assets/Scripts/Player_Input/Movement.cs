using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    
    float __input_x = 0f;
    float __input_y = 0f;
    public float speed = 5.5f;
    bool __isWalking = false;
    void Awake()
    {
        __isWalking = false;
    }

    // Update is called once per frame
    void Update() {
        __input_x = Input.GetKey("d") ? (Input.GetKey("a") ? 0 : 1) : (Input.GetKey("a") ? -1 : 0);
        __input_y = Input.GetKey("w") ? (Input.GetKey("s") ? 0 : 1) : (Input.GetKey("s") ? -1 : 0);
        __isWalking = (__input_x != 0 || __input_y != 0);
    }

    void FixedUpdate()
    {
        if(__isWalking){
            var move = new Vector3(__input_x, __input_y, 0).normalized;
            transform.position += move * speed * Time.deltaTime;
        } 
    }
}
