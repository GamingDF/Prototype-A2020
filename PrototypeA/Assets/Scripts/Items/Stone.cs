using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {
	public float speed = 0.1f;
	public float acceleration = -0.5f;
	public Color triggerColor;

	Rigidbody2D rb;
	float __minRate = 0.01f;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {
		rb.AddForce(-rb.velocity * speed * rb.mass, ForceMode2D.Impulse);
		if (rb.velocity.magnitude < __minRate) {
			rb.velocity = Vector2.zero;
			GetComponent<Collider2D>().isTrigger = true;
			GetComponent<SpriteRenderer>().color = triggerColor;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Enemy") {
			rb.velocity = new Vector3(0, 0, 0);
			GetComponent<Collider2D>().isTrigger = true;
			GetComponent<SpriteRenderer>().color = triggerColor;
			Health enemyHealth = other.gameObject.GetComponent<Health>();
			print(enemyHealth.GetHealth());
			enemyHealth.DealDamage(20);
			if (enemyHealth.GetHealth() == 0) {
				Destroy(other.gameObject);
			}
		}
	}
}
