using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfind2D {
	public class Node : IHeapItem<Node> {
		public bool walkable;
		public Vector3 worldPosition;
		public Vector2Int gridPos;
		public int HeapIndex { get; set; }
		public int movementPenalty;

		public int gCost;
		public int hCost;
		public int fCost => gCost + hCost;
		public Node parent;

		public Node(bool p_walkable, Vector3 p_worldPosition, int p_gridX, int p_gridY, int p_penalty) {
			walkable = p_walkable;
			worldPosition = p_worldPosition;
			gridPos.x = p_gridX;
			gridPos.y = p_gridY;
			movementPenalty = p_penalty;
		}

		public int CompareTo(Node p_node) {
			int __compare = fCost.CompareTo(p_node.fCost);
			if (__compare == 0) {
				__compare = hCost.CompareTo(p_node.hCost);
			}
			return -__compare;
		}
	}
}
