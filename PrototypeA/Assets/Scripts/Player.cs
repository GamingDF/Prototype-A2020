using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Transform instance;
	public List<string> projectils;

	void Awake() {
		if (instance == null) {
			instance = transform;
		}
		else {
			Debug.Log("Multiple players!");
			Destroy(gameObject);
		}

		projectils = new List<string>();
		projectils.Add("normalStone");
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.CompareTag("Collectable")) {
			string projectilType = collision.gameObject.GetComponent<Collectable>().projectilType;
			projectils.Add(projectilType);
			Destroy(collision.gameObject);
		}
	}
}
