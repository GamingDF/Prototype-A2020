using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Stone : MonoBehaviour {
	public float speed = 0.1f;
	public Color triggerColor;

	Rigidbody2D rb;
	float _minRate = 0.01f;
	GameObject _originalPrefab;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		_originalPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Stone.prefab", typeof(GameObject));
		GetComponent<Collectable>().SetPrefab(_originalPrefab);
	}

	void FixedUpdate() {
		rb.AddForce(-rb.velocity * speed * rb.mass, ForceMode2D.Impulse);
		if (rb.velocity.magnitude < _minRate) {
			rb.velocity = Vector2.zero;
			GetComponent<Collider2D>().isTrigger = true;
			GetComponent<SpriteRenderer>().color = triggerColor;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Enemy") {
			rb.velocity = Vector2.zero;
			GetComponent<Collider2D>().isTrigger = true;
			GetComponent<SpriteRenderer>().color = triggerColor;
			Health __enemyHealth = other.gameObject.GetComponent<Health>();
			print(__enemyHealth.GetHealth());
			__enemyHealth.DealDamage(20);
		}
	}
}