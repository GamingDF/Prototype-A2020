using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour {
	StackFSM _fsm;
	public bool _altState = false;
	public bool _timeState = false;
	float _timer = 0;

	void Awake() {
		_fsm = new StackFSM();
		_fsm.PushState(InitialState);
	}

	void Update() {
		_fsm.UpdateState();
	}

	void InitialState() {
		Debug.Log("initial state");
		if (_altState) {
			_fsm.PushState(SecondState);
		}
		if (_timeState) {
			_fsm.PushState(TimeState);
			_timer = 0;
			_timeState = false;
		}
	}

	void SecondState() {
		Debug.Log("alternate state");
		if (!_altState) {
			_fsm.PopState();
		}
	}

	void TimeState() {
		_timer += Time.deltaTime;
		Debug.Log(_timer);
		if (_timer > 5) {
			_fsm.PopState();
		}
	}
}
