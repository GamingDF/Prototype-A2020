using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    
    float _input_x = 0f;
    float _input_y = 0f;
    public float speed = 5.5f;
    bool _isWalking = false;
    void Awake()
    {
        _isWalking = false;
    }

    // Update is called once per frame
    void Update()
    {
        _input_x = Input.GetAxisRaw("Horizontal");
        _input_y = Input.GetAxisRaw("Vertical");
        _isWalking = (_input_x != 0 || _input_y != 0);

        if(_isWalking){
            var move = new Vector3(_input_x, _input_y, 0).normalized;
            transform.position += move * speed * Time.deltaTime;
        } 
    }
}
