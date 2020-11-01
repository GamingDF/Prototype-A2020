using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Transform instance;
	public List<GameObject> projectiles;
	public GameObject initialProjectile;

	void Awake() {
		if (instance == null) {
			instance = transform;
		}
		else {
			Debug.Log("Multiple players!");
			Destroy(gameObject);
		}

		projectiles = new List<GameObject>();
		projectiles.Add(initialProjectile);
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.CompareTag("Collectable")) {
			if(collision.gameObject.GetComponent<Collectable>().itemType == "projectile")
				projectiles.Add(collision.gameObject.GetComponent<Collectable>().GetPrefab());
			Destroy(collision.gameObject);
		}
	}
}
