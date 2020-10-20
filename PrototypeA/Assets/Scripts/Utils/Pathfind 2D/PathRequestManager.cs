using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfind2D {
	public class PathRequestManager : MonoBehaviour {
		struct PathRequest {
			public Vector3 pathStart;
			public Vector3 pathEnd;
			public Action<Vector3[], bool> callback;

			public PathRequest(Vector3 p_pathStart, Vector3 p_pathEnd, Action<Vector3[], bool> p_callback) {
				pathStart = p_pathStart;
				pathEnd = p_pathEnd;
				callback = p_callback;
			}
		}

		public static PathRequestManager instance;
		Pathfinding _pathfinding;
		Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
		PathRequest _currentRequest;
		bool _isProcessingPath = false;

		void Awake() {
			if (instance == null) {
				instance = this;
			}
			else {
				Debug.LogWarning("Duplicate instance of PathRequestManager in the scene!");
			}

			_pathfinding = GetComponent<Pathfinding>();
		}

		public void RequestPath(Vector3 p_pathStart, Vector3 p_pathEnd, Action<Vector3[], bool> p_callback) {
			PathRequest __newRequest = new PathRequest(p_pathStart, p_pathEnd, p_callback);
			instance._requestQueue.Enqueue(__newRequest);
			instance.TryProcessNext();
		}

		void TryProcessNext() {
			if (!_isProcessingPath && _requestQueue.Count > 0) {
				_currentRequest = _requestQueue.Dequeue();
				_isProcessingPath = true;
				_pathfinding.StartFindPath(_currentRequest.pathStart, _currentRequest.pathEnd);
			}
		}

		public void FinishedProcessingPath(Vector3[] p_path, bool p_success) {
			_currentRequest.callback(p_path, p_success);
			_isProcessingPath = false;
			TryProcessNext();
		}
	}
}
