using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour {
	[SerializeField] HealthInfo _healthInfo = null;
	int _currentHealth = 0;
	int _currentMaxHealth = 0;

	public bool IsFull { get => _currentHealth == _currentMaxHealth; }
	public Action<int> healthUpdate;

	void Awake() {
		// Setup all health info.
		_currentMaxHealth = _healthInfo.MaxHealth;
		_currentHealth = _currentMaxHealth;
	}

	void Start() {
		// Send initial value.
		if (healthUpdate != null) {
			healthUpdate(_currentHealth);
		}
	}

	public void DealDamage(int p_damage) {
		if (p_damage > 0) {
			_currentHealth -= p_damage;
			_currentHealth = _currentHealth < 0 ? 0 : _currentHealth;
			if (healthUpdate != null) {
				healthUpdate(_currentHealth);
			}
		}
	}

	public void HealDamage(int p_heal) {
		if (p_heal > 0) {
			_currentHealth += p_heal;
			_currentHealth = _currentHealth > _currentMaxHealth ? _currentMaxHealth : _currentHealth;
			if (healthUpdate != null) {
				healthUpdate(_currentHealth);
			}
		}
	}
	public float GetHealth(){
		return _currentHealth;
	}
}
