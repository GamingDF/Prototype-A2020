using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringEnemy : MonoBehaviour {
	[SerializeField] [Min(0)] float _maxSpeed = 0;
	[SerializeField] [Min(0)] float _maxForce = 0;
	[SerializeField] [Min(0.01f)] float _mass = 1;
	[SerializeField] [Min(0)] float _closeRadius = 0;
	[SerializeField] [Min(0)] float _farRadius = 0;
	[SerializeField] [Min(0)] float _fleeDistance = 0;
	[SerializeField] bool _flee = false;
	StackFSM _fsm;
	Vector2 _velocity;

	void Awake() {
		_fsm = new StackFSM();
		_fsm.PushState(CloseState);
	}

	void Update() {
		_fsm.UpdateState();
	}

	void CloseState() {
		Vector2 __mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float __diff = (__mousePosition - (Vector2)transform.position).magnitude;
		if (_flee) {
			if (__diff < 1) {
				_fsm.PushState(FleeState);
			}
		}
		else {
			if (__diff > 1) {
				_fsm.PushState(SeekState);
			}
		}
	}

	void SeekState() {
		Vector2 __mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 __desiredVelocity = __mousePosition - (Vector2)transform.position;

		if (__desiredVelocity.magnitude < _closeRadius) {
			__desiredVelocity = __desiredVelocity.normalized * _maxSpeed * (__desiredVelocity.magnitude / _closeRadius);
		}
		else {
			__desiredVelocity = __desiredVelocity.normalized * _maxSpeed;
		}

		Vector2 __steering = __desiredVelocity - _velocity;
		__steering = Vector2.ClampMagnitude(__steering, _maxForce);
		__steering = __steering / _mass;

		_velocity = Vector2.ClampMagnitude(_velocity + __steering, _maxSpeed);
		transform.position = transform.position + (Vector3)_velocity;

		float __diff = (__mousePosition - (Vector2)transform.position).magnitude;
		if (__diff < 0.1) {
			_fsm.PopState();
		}
	}

	void FleeState() {
		Vector2 __mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 __desiredVelocity = ((Vector2)transform.position - __mousePosition).normalized * _maxSpeed;
		float __diff = (__mousePosition - (Vector2)transform.position).magnitude;

		if (__diff > _farRadius) {
			__desiredVelocity = __desiredVelocity.normalized * _maxSpeed * (1 - ((__diff - _farRadius) / (_fleeDistance - _farRadius)));
		}
		else {
			__desiredVelocity = __desiredVelocity.normalized * _maxSpeed;
		}

		Vector2 __steering = __desiredVelocity - _velocity;
		__steering = Vector2.ClampMagnitude(__steering, _maxForce);
		__steering = __steering / _mass;

		_velocity = Vector2.ClampMagnitude(_velocity + __steering, _maxSpeed);
		transform.position = transform.position + (Vector3)_velocity;

		if (__diff > _fleeDistance) {
			_fsm.PopState();
		}
	}
}
