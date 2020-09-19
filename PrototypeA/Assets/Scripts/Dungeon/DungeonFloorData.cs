using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon Floor Data", menuName = "ScriptableObjects/Dungeon/Floor Data")]
public class DungeonFloorData : ScriptableObject {
	public Vector2Int FloorSize => _floorSize;
	public int RoomQuantity => _roomQuantity;
	public DungeonRoomPoolData RoomPool => _roomPool;

	[SerializeField] [Min(1)] Vector2Int _floorSize = Vector2Int.one;
	[SerializeField] [Min(0)] int _roomQuantity = 0;
	[SerializeField] DungeonRoomPoolData _roomPool = null;
}
