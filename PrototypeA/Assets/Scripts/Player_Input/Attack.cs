using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
	public Transform throwPoint;
	

	public float projectileForce = 10f;

	int __projNumber;
	List<GameObject> __projectileList;

	void Start() {
		__projectileList = GameObject.Find("Player").GetComponent<Player>().projectiles;
	}

	void Update() {
		if (Input.GetButtonDown("Fire1")) {
			__projNumber = __projectileList.Count;
			if (__projNumber > 0)
				Shoot();
		}
	}
	void Shoot() {
		GameObject proj = Instantiate(__projectileList[0], throwPoint.position, throwPoint.rotation);
		__projectileList.RemoveAt(0);
		Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
		rb.AddForce(throwPoint.up * projectileForce * rb.mass, ForceMode2D.Impulse);
	}
}
