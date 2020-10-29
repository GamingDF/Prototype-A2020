using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform throwPoint;
    public GameObject projectilePrefab;

    public float projectileForce = 20f;

    int __projNumber;
    List<string> __projectilList;

    void Start()
    {
        __projectilList = GameObject.Find("Player").GetComponent<Player>().projectils;
    }

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            __projNumber = __projectilList.Count;
            if (__projNumber > 0)
                Shoot();
        }
    }
    void Shoot()
    {
        __projectilList.RemoveAt(0);
        GameObject proj = Instantiate(projectilePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.AddForce(throwPoint.up * projectileForce, ForceMode2D.Impulse);
    }
}
