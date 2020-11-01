using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeekEnemy : MonoBehaviour {
	[SerializeField] bool _debug = false;
	[SerializeField] TextMeshProUGUI _informState = null;

	[Header("Steering Properties")]
	[SerializeField] [Min(0)] float _maxSpeed = 0;
	[SerializeField] [Min(0)] float _maxForce = 0;
	[SerializeField] [Min(0.01f)] float _mass = 1;

	[Header("Seek")]
	[SerializeField] [Min(0)] float _closeRadius = 0;
	[SerializeField] float _velocityModifier = 1;

	[Header("Pathfind")]
	[SerializeField] float _pathTimeToRefresh = 1;

	[Header("Idle")]
	[SerializeField] float _idleTime = 1;

	Vector3[] _path;
	int _pathIndex;
	bool _pathRequested = false;
	float _pathRefreshTimer = 0;

	float _idleTimer = 0;
	Health _health;


	StackFSM _fsm;
	Vector2 _velocity;


	void Awake() {
		_fsm = new StackFSM();
		_fsm.PushState(PathState);
		_informState.text = "";
		_health = GetComponent<Health>();
		_health.healthUpdate += CheckIfDead;
	}

	void Update() {
		_fsm.UpdateState();
	}

	void IdleState() {
		if (_debug) _informState.text = "idle";

		_idleTimer += Time.deltaTime;
		if (_idleTimer > _idleTime) {
			_fsm.PopState();
			_idleTimer = 0;
		}
	}

	void SeekState() {
		if (_debug) _informState.text = "seek";

		Vector2 __desiredPosition = Player.instance.position;
		Vector2 __desiredVelocity = __desiredPosition - (Vector2)transform.position;

		if (__desiredVelocity.magnitude < _closeRadius) {
			__desiredVelocity = __desiredVelocity.normalized * _maxSpeed * _velocityModifier * (__desiredVelocity.magnitude / _closeRadius);
		}
		else {
			__desiredVelocity = __desiredVelocity.normalized * _maxSpeed * _velocityModifier;
		}

		Vector2 __steering = __desiredVelocity - _velocity;
		__steering = Vector2.ClampMagnitude(__steering, _maxForce);
		__steering = __steering / _mass;

		_velocity = Vector2.ClampMagnitude(_velocity + __steering, _maxSpeed * _velocityModifier);
		transform.position = transform.position + (Vector3)_velocity * Time.deltaTime;

		Vector3 __direction = (Player.instance.position - transform.position).normalized;
		RaycastHit2D __ray = Physics2D.Raycast(transform.position + __direction * 1.2f, __direction);
		//Debug.DrawLine(transform.position, transform.position + __direction * 1.2f, Color.red);
		if (__ray.collider == null) {
			_fsm.PopState();
			_fsm.PushState(PathState);
		}
		else {
			if (__ray.transform.tag != "Player") {
				_fsm.PopState();
				_fsm.PushState(PathState);
			}
		}
	}

	void PathState() {
		if (_debug) _informState.text = "path";
		_pathRefreshTimer += Time.deltaTime;

		if (_path == null && !_pathRequested || _pathRefreshTimer > _pathTimeToRefresh) {
			Pathfind2D.PathRequestManager.instance.RequestPath(transform.position, Player.instance.position, PathRequestCallback);
			_pathRefreshTimer = 0;
			_pathRequested = true;
		}
		else {
			if (_path != null) {
				if (_pathIndex < _path.Length) {
					Vector2 __desiredPosition = _path[_pathIndex];
					Vector2 __desiredVelocity = __desiredPosition - (Vector2)transform.position;

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
					transform.position = transform.position + (Vector3)_velocity * Time.deltaTime;

					float __diff = (__desiredPosition - (Vector2)transform.position).magnitude;
					if (__diff < 0.3) {
						_pathIndex++;
					}
				}
				else {
					_path = null;
				}
			}
		}

		Vector3 __direction = (Player.instance.position - transform.position).normalized;
		RaycastHit2D __ray = Physics2D.Raycast(transform.position + __direction * 1.2f, __direction);
		//Debug.DrawLine(transform.position, transform.position + __direction * 1.2f, Color.red);
		if (__ray.collider != null) {
			if (__ray.transform.tag == "Player") {
				_fsm.PopState();
				_fsm.PushState(SeekState);
				_path = null;
				_pathRequested = false;
				_pathRefreshTimer = 0;
			}
		}
	}

	void PathRequestCallback(Vector3[] p_path, bool p_success) {
		if (p_success) {
			_path = p_path;
			_pathIndex = 0;
		}
		else {
			_path = null;
		}
		_pathRequested = false;
	}

	public void OnDrawGizmos() {
		if (!_debug) {
			return;
		}
		if (_path != null) {
			for (int i = 0; i < _path.Length; i++) {
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(_path[i], Vector3.one * 0.5f);

				if (i == 0) {
					Gizmos.DrawLine(transform.position, _path[i]);
				}
				else {
					Gizmos.DrawLine(_path[i - 1], _path[i]);
				}
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<Health>().DealDamage(20);
			_fsm.PushState(IdleState);
		}
	}

	void CheckIfDead(int p_health){
		if(p_health == 0)
			Destroy(gameObject);
	}
}
