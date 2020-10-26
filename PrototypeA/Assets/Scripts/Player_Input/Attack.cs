using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform throwPoint;
    public GameObject projectilePrefab;

    public float projectileForce = 20f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    void Shoot()
    {
        GameObject proj = Instantiate(projectilePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.AddForce(throwPoint.up * projectileForce, ForceMode2D.Impulse);
    }
}
