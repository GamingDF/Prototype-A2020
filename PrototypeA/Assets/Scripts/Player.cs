using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Transform instance;

	void Awake() {
		if (instance == null) {
			instance = transform;
		}
		else {
			Debug.Log("Multiple players!");
			Destroy(gameObject);
		}
	}
}
