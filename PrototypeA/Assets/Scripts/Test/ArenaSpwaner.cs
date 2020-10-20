using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpwaner : MonoBehaviour {
	[SerializeField] GameObject _spawned = null;
	[SerializeField] Transform[] _spawnPositions = null;
	[SerializeField] float _spawnTime = 0;

	float _spawnTimer = 0;

	void Start() {
		_spawnTimer = _spawnTime - 10;
	}

	void Update() {
		_spawnTimer += Time.deltaTime;
		if (_spawnTimer > _spawnTime) {
			int __index = Random.Range(0, _spawnPositions.Length);
			Instantiate(_spawned, _spawnPositions[__index].position, Quaternion.identity);
			_spawnTimer = 0;
		}
	}
}
