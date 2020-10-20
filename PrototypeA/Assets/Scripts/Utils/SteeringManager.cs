using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager {
	Vector2 _steering;
	ISteeringBody _host;

	public SteeringManager(ISteeringBody p_host) {
		_host = p_host;
		_steering = Vector2.zero;
	}

	public Vector2 Update() {
		Vector2 __velocity = _host.GetVelocity();
		Vector2 __position = _host.GetPosition();

		_steering = Vector2.ClampMagnitude(_steering, _host.GetMaxSteering());
		_steering = _steering / _host.GetMass();

		__velocity += _steering;
		__velocity = Vector2.ClampMagnitude(__velocity, _host.GetMaxVelocity());
		_host.SetVelocity(__velocity);

		__position += __velocity * Time.deltaTime;

		Reset();
		return __position;
	}

	public void Reset() {
		_steering = Vector2.zero;
	}

	public void Seek(Vector2 p_target, float p_slowRadius = 0) {
		_steering += DoSeek(p_target, p_slowRadius);
	}

	Vector2 DoSeek(Vector2 p_target, float p_slowRadius) {
		Vector2 __desired = p_target - _host.GetPosition();
		float __distance = __desired.magnitude;

		if (__distance < p_slowRadius) {
			__desired = __desired.normalized * _host.GetMaxVelocity() * (__distance / p_slowRadius);
		}
		else {
			__desired = __desired.normalized * _host.GetMaxVelocity();
		}

		return __desired - _host.GetVelocity();
	}

	public void Flee(Vector2 p_target, float p_distance, float p_slowDistanceRatio = 1) {
		_steering += DoFlee(p_target, p_distance, p_slowDistanceRatio);
	}

	Vector2 DoFlee(Vector2 p_target, float p_distance, float p_slowDistanceRatio) {
		Vector2 __desired = _host.GetPosition() - p_target;
		float __distance = __desired.magnitude;

		if (__distance > p_distance * p_slowDistanceRatio) {
			__desired = __desired.normalized * _host.GetMaxVelocity() * (1 - ((__distance - p_distance * p_slowDistanceRatio) / (p_distance - p_distance * p_slowDistanceRatio)));
		}
		else {
			__desired = __desired.normalized * _host.GetMaxVelocity();
		}

		return __desired - _host.GetVelocity();
	}
}
