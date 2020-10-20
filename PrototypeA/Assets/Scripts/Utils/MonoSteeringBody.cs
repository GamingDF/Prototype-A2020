using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSteeringBody : MonoBehaviour, ISteeringBody {
	public float Mass => _mass;
	public float MaxVelocity => _maxVelocity;
	public Vector2 Velocity => _velocity;
	public float MaxSteering => _maxSteering;

	[Header("Steering Body")]
	[SerializeField] [Min(0.01f)] float _mass = 1;
	[SerializeField] [Min(0)] float _maxVelocity = 0;
	[SerializeField] [Min(0)] float _maxSteering = 0;
	Vector2 _velocity = Vector2.zero;

	public Vector2 GetVelocity() {
		return _velocity;
	}

	public Vector2 SetVelocity(Vector2 p_velocity) {
		return _velocity = p_velocity;
	}

	public float GetMaxVelocity() {
		return _maxVelocity;
	}

	public Vector2 GetPosition() {
		return transform.position;
	}

	public float GetMass() {
		return _mass;
	}

	public float GetMaxSteering() {
		return _maxSteering;
	}
}
