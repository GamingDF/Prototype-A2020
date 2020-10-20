using System.Collections.Generic;
using System;

public class StackFSM {
	Stack<Action> _stateStack;

	public StackFSM() {
		_stateStack = new Stack<Action>();
	}

	// Process the current state, must be called on Update method.
	public void UpdateState() {
		Action __currentState = GetCurrentState();

		if (__currentState != null) {
			__currentState();
		}
	}

	public Action PopState() {
		return _stateStack.Pop();
	}

	public void PushState(Action p_state) {
		if (GetCurrentState() != p_state) {
			_stateStack.Push(p_state);
		}
	}

	public Action GetCurrentState() {
		return _stateStack.Count > 0 ? _stateStack.Peek() : null;
	}
}
