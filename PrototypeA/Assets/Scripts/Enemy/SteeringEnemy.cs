using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringEnemy : MonoBehaviour {
	[SerializeField] [Min(0)] float _maxSpeed = 0;
	[SerializeField] [Min(0)] float _maxForce = 0;
	[SerializeField] [Min(0.01f)] float _mass = 1;

	[Header("Seek State")]
	[SerializeField] [Min(0)] float _closeRadius = 0;

	[Header("Flee State")]
	[SerializeField] bool _flee = false;
	[SerializeField] [Min(0)] float _farRadius = 0;
	[SerializeField] [Min(0)] float _fleeDistance = 0;

	[Header("Random Seek Wander")]
	[SerializeField] bool _seekWander = false;
	[SerializeField] Vector2 _randomSeekMin = Vector2.zero;
	[SerializeField] Vector2 _randomSeekMax = Vector2.zero;
	[SerializeField] [Min(0)] float _newRandomTime = 0;

	[Header("Wander")]
	[SerializeField] bool _wander = false;
	[SerializeField] float _wanderCircleDistance = 0;
	[SerializeField] [Min(0)] float _wanderCircleRadius = 0;
	[SerializeField] [Min(0)] float _wanderAngleChangeLimit = 0;

	StackFSM _fsm;
	Vector2 _velocity;
	float _randomSeekTimer = 0;
	Vector2 _randomSeekTarget = Vector2.zero;
	float _wanderAngle = 0;
	bool _outOfVision = false;

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
		if (_wander) {
			_fsm.PushState(WanderState);
			return;
		}
		if (_seekWander) {
			_fsm.PushState(SeekWanderState);
			return;
		}
		if (_flee) {
			if (__diff < 1) {
				_fsm.PushState(FleeState);
			}
			return;
		}
		if (__diff > 1) {
			_fsm.PushState(SeekState);
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

	void SeekWanderState() {
		// Create random target position to go.
		_randomSeekTimer += Time.deltaTime;
		if (_randomSeekTimer > _newRandomTime) {
			_randomSeekTimer = 0;
			_randomSeekTarget = new Vector2(Random.Range(_randomSeekMin.x, _randomSeekMax.x), Random.Range(_randomSeekMin.y, _randomSeekMax.y));
		}

		// From here it's the same as Seek.
		Vector2 __desiredVelocity = _randomSeekTarget - (Vector2)transform.position;

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

		if (!_seekWander) {
			_fsm.PopState();
		}
	}

	void WanderState() {
		// Wander circle position.
		Vector2 __circle = _velocity.normalized * _wanderCircleDistance;

		// Displacement force.
		Vector2 __displacement = Quaternion.Euler(_wanderAngle * Vector3.forward) * Vector3.up * _wanderCircleRadius;

		// Change wander angle just a little.
		_wanderAngle += Random.Range(-_wanderAngleChangeLimit, _wanderAngleChangeLimit);

		Vector2 __wanderForce = __displacement + __circle;

		__wanderForce = Vector2.ClampMagnitude(__wanderForce, _maxForce);
		__wanderForce = __wanderForce / _mass;
		_velocity = Vector2.ClampMagnitude(_velocity + __wanderForce, _maxSpeed);
		transform.position = transform.position + (Vector3)_velocity;

		if (_outOfVision) {
			transform.position = Vector3.zero;
			_outOfVision = false;
		}

		if (!_wander) {
			_fsm.PopState();
		}
	}

	void OnBecameInvisible() {
		_outOfVision = true;
	}
}
