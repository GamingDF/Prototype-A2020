using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfind2D {
	public class PathGrid : MonoBehaviour {
		public bool debugGizmos = true;
		public int MaxSize => _gridSize.x * _gridSize.y;
		public TerrainType[] walkableRegion;
		public int obstacleProximityPenalty = 10;
		public int blurSize = 3;
		LayerMask _walkableMask;
		//Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();		// 3D code.
		Dictionary<LayerMask, int> _walkableRegionsDictionary = new Dictionary<LayerMask, int>();

		[SerializeField] LayerMask _unwalkableMask = 0;
		[SerializeField] Vector2 _gridWorldSize = Vector2.zero;
		[SerializeField] float _nodeRadius = 1;

		Node[,] _grid;
		float _nodeDiameter;
		Vector2Int _gridSize;
		Vector3 _gridCenter;

		int _penaltyMin = int.MaxValue;
		int _penaltyMax = int.MinValue;

		void Awake() {
			_nodeDiameter = _nodeRadius * 2;
			_gridSize.x = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
			_gridSize.y = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);

			foreach (var __region in walkableRegion) {
				_walkableMask.value |= __region.terrainMask.value;
				//_walkableRegionsDictionary.Add((int)Mathf.Log(__region.terrainMask.value, 2), __region.terrainPenalty);	// 3D code.
				_walkableRegionsDictionary.Add(__region.terrainMask, __region.terrainPenalty);
			}

			CreateGrid();
		}

		void CreateGrid() {
			_grid = new Node[_gridSize.x, _gridSize.y];
			Vector3 __worldBottonLeft = transform.position - Vector3.right * _gridWorldSize.x / 2 - Vector3.up * _gridWorldSize.y / 2;
			_gridCenter = transform.position;

			for (int x = 0; x < _gridSize.x; x++) {
				for (int y = 0; y < _gridSize.y; y++) {
					Vector3 __worldPoint = __worldBottonLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius) + Vector3.up * (y * _nodeDiameter + _nodeRadius);
					bool __walkable = !(Physics2D.OverlapCircle(__worldPoint, _nodeRadius, _unwalkableMask));

					int __movementPenalty = 0;

					// Using 3D code.
					/*
					Ray __ray = new Ray(__worldPoint + Vector3.forward * 50, Vector3.back);
					RaycastHit __hit;
					if (Physics.Raycast(__ray, out __hit, 100, _walkableMask)) {
						_walkableRegionsDictionary.TryGetValue(__hit.collider.gameObject.layer, out __movementPenalty);
						//Debug.Log("hit walkable");
					}
					*/

					// Using 2D code.
					Collider2D __collision = Physics2D.OverlapPoint(__worldPoint, _walkableMask);
					if (__collision != null) {
						List<Collider2D> __tmp = new List<Collider2D>();
						ContactFilter2D __filter = new ContactFilter2D();
						__filter.useTriggers = true;
						__filter.useLayerMask = true;

						foreach (var __region in _walkableRegionsDictionary.Keys) {
							__filter.layerMask = _walkableMask - __region;
							int __i = __collision.OverlapCollider(__filter, __tmp);
							if (__i != 0) {
								__movementPenalty = _walkableRegionsDictionary[__region];
								break;
							}

							/*
							if (Physics2D.OverlapCircle(__worldPoint, _nodeRadius, __region)) {
								__movementPenalty = _walkableRegionsDictionary[__region];
								Debug.Log("penalty: " + __movementPenalty);
								break;
							}
							*/
						}
					}

					if (!__walkable) {
						__movementPenalty += obstacleProximityPenalty;
					}

					_grid[x, y] = new Node(__walkable, __worldPoint, x, y, __movementPenalty);
				}
			}

			BlurPenaltyMap(blurSize);
		}

		void BlurPenaltyMap(int p_blurSize) {
			int __kernelSize = p_blurSize * 2 + 1;
			int __kernelExtents = (__kernelSize - 1) / 2;

			int[,] __penaltiesHorizontalPass = new int[_gridSize.x, _gridSize.y];
			int[,] __penaltiesVerticalPass = new int[_gridSize.x, _gridSize.y];

			for (int y = 0; y < _gridSize.y; y++) {
				for (int x = -__kernelExtents; x <= __kernelExtents; x++) {
					int __sampleX = Mathf.Clamp(x, 0, __kernelExtents);
					__penaltiesHorizontalPass[0, y] += _grid[__sampleX, y].movementPenalty;
				}

				for (int x = 1; x < _gridSize.x; x++) {
					int __removeIndex = Mathf.Clamp(x - __kernelExtents - 1, 0, _gridSize.x);
					int __addIndex = Mathf.Clamp(x + __kernelExtents, 0, _gridSize.x - 1);

					__penaltiesHorizontalPass[x, y] = __penaltiesHorizontalPass[x - 1, y] - _grid[__removeIndex, y].movementPenalty + _grid[__addIndex, y].movementPenalty;
				}
			}

			for (int x = 0; x < _gridSize.x; x++) {
				for (int y = -__kernelExtents; y <= __kernelExtents; y++) {
					int __sampleY = Mathf.Clamp(y, 0, __kernelExtents);
					__penaltiesVerticalPass[x, 0] += __penaltiesHorizontalPass[x, __sampleY];
				}

				int __blurredPenalty = Mathf.RoundToInt((float)__penaltiesVerticalPass[x, 0] / (__kernelSize * __kernelSize));
				_grid[x, 0].movementPenalty = __blurredPenalty;

				for (int y = 1; y < _gridSize.y; y++) {
					int __removeIndex = Mathf.Clamp(y - __kernelExtents - 1, 0, _gridSize.y);
					int __addIndex = Mathf.Clamp(y + __kernelExtents, 0, _gridSize.y - 1);

					__penaltiesVerticalPass[x, y] = __penaltiesVerticalPass[x, y - 1] - __penaltiesHorizontalPass[x, __removeIndex] + __penaltiesHorizontalPass[x, __addIndex];

					__blurredPenalty = Mathf.RoundToInt((float)__penaltiesVerticalPass[x, y] / (__kernelSize * __kernelSize));
					_grid[x, y].movementPenalty = __blurredPenalty;

					if (__blurredPenalty > _penaltyMax) {
						_penaltyMax = __blurredPenalty;
					}
					if (__blurredPenalty < _penaltyMin) {
						_penaltyMin = __blurredPenalty;
					}
				}
			}
		}

		public List<Node> GetNodeNeighbours(Node p_node) {
			List<Node> __neighbours = new List<Node>();

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					if (x == 0 && y == 0) {
						continue;
					}

					int __checkX = p_node.gridPos.x + x;
					int __checkY = p_node.gridPos.y + y;

					if (__checkX >= 0 && __checkX < _gridSize.x && __checkY >= 0 && __checkY < _gridSize.y) {
						__neighbours.Add(_grid[__checkX, __checkY]);
					}
				}
			}

			return __neighbours;
		}

		public Node NodeInWorldPosition(Vector3 p_worldPosition) {
			float __percentX = (p_worldPosition.x - _gridCenter.x + _gridWorldSize.x / 2) / _gridWorldSize.x;
			float __percentY = (p_worldPosition.y - _gridCenter.y + _gridWorldSize.y / 2) / _gridWorldSize.y;
			__percentX = Mathf.Clamp01(__percentX);
			__percentY = Mathf.Clamp01(__percentY);

			int __x = Mathf.RoundToInt((_gridSize.x - 1) * __percentX);
			int __y = Mathf.RoundToInt((_gridSize.y - 1) * __percentY);
			return _grid[__x, __y];
		}

		void OnDrawGizmos() {
			Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, _gridWorldSize.y, 1));
			if (debugGizmos) {

				if (_grid != null) {
					foreach (var __node in _grid) {
						Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(_penaltyMin, _penaltyMax, __node.movementPenalty));

						Gizmos.color = __node.walkable ? Gizmos.color : Color.red;
						Gizmos.DrawCube(__node.worldPosition, Vector3.one * (_nodeDiameter));
					}
				}
			}
		}

		[System.Serializable]
		public class TerrainType {
			public LayerMask terrainMask;
			public int terrainPenalty;
		}
	}
}
