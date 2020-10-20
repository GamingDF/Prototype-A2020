using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

namespace Pathfind2D {
	public class Pathfinding : MonoBehaviour {
		[SerializeField] bool _debugTime = false;

		PathGrid _grid;

		void Awake() {
			_grid = GetComponent<PathGrid>();
		}

		public void StartFindPath(Vector3 p_startPos, Vector3 p_targetPos) {
			StartCoroutine(FindPath(p_startPos, p_targetPos));
		}

		IEnumerator FindPath(Vector3 p_startPos, Vector3 p_targetPos) {
			Stopwatch __sw = new Stopwatch();
			if (_debugTime) __sw.Start();

			Vector3[] __waypoints = new Vector3[0];
			bool __pathSuccess = false;

			Node __startNode = _grid.NodeInWorldPosition(p_startPos);
			Node __targetNode = _grid.NodeInWorldPosition(p_targetPos);

			if (__startNode.walkable && __targetNode.walkable) {
				Heap<Node> __openSet = new Heap<Node>(_grid.MaxSize);
				HashSet<Node> __closedSet = new HashSet<Node>();
				__openSet.Add(__startNode);

				while (__openSet.Count > 0) {
					// Get node with lowest F cost.
					Node __currentNode = __openSet.RemoveFirst();
					__closedSet.Add(__currentNode);

					// Has path been found?
					if (__currentNode == __targetNode) {
						if (_debugTime) __sw.Stop();
						if (_debugTime) print("Path found in: " + __sw.ElapsedMilliseconds + "ms");
						__pathSuccess = true;
						break;
					}

					foreach (var __neighbour in _grid.GetNodeNeighbours(__currentNode)) {
						if (!__neighbour.walkable || __closedSet.Contains(__neighbour)) {
							continue;
						}

						int __newMovementCostToNeighbour = __currentNode.gCost + GetDistance(__currentNode, __neighbour) + __neighbour.movementPenalty;
						if (__newMovementCostToNeighbour < __neighbour.gCost || !__openSet.Contains(__neighbour)) {
							__neighbour.gCost = __newMovementCostToNeighbour;
							__neighbour.hCost = GetDistance(__neighbour, __targetNode);
							__neighbour.parent = __currentNode;

							if (!__openSet.Contains(__neighbour)) {
								__openSet.Add(__neighbour);
							}
							else {
								__openSet.UpdateItem(__neighbour);
							}
						}
					}
				}
			}

			yield return null;
			if (__pathSuccess) {
				__waypoints = RetracePath(__startNode, __targetNode);
			}
			PathRequestManager.instance.FinishedProcessingPath(__waypoints, __pathSuccess);
		}

		Vector3[] RetracePath(Node p_startNode, Node p_endNode) {
			List<Node> __path = new List<Node>();
			Node __currentNode = p_endNode;

			while (__currentNode != p_startNode) {
				__path.Add(__currentNode);
				__currentNode = __currentNode.parent;
			}
			Vector3[] __waypoints = SimplifyPath(__path);
			Array.Reverse(__waypoints);

			return __waypoints;
		}

		Vector3[] SimplifyPath(List<Node> p_path) {
			List<Vector3> __waypoints = new List<Vector3>();
			Vector2 __directionOld = Vector2.zero;

			for (int i = 1; i < p_path.Count; i++) {
				Vector2 __directionNew = new Vector2(p_path[i - 1].gridPos.x - p_path[i].gridPos.x, p_path[i - 1].gridPos.y - p_path[i].gridPos.y);
				if (__directionNew != __directionOld) {
					__waypoints.Add(p_path[i].worldPosition);
				}
				__directionOld = __directionNew;
			}

			return __waypoints.ToArray();
		}

		int GetDistance(Node p_nodeA, Node p_nodeB) {
			int __dstX = Mathf.Abs(p_nodeA.gridPos.x - p_nodeB.gridPos.x);
			int __dstY = Mathf.Abs(p_nodeA.gridPos.y - p_nodeB.gridPos.y);

			if (__dstX > __dstY) {
				return 14 * __dstY + 10 * (__dstX - __dstY);
			}
			return 14 * __dstX + 10 * (__dstY - __dstX);
		}
	}
}
