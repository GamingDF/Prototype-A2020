using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
	[SerializeField] DungeonFloorData _floorData = null;
	Room[,] _rooms;
	Dictionary<Vector2Int, bool> _takenPositions = new Dictionary<Vector2Int, bool>(); // [room position on grid, if room was created]
	Dictionary<Vector2Int, Enums.Directions> _positionsToFill = new Dictionary<Vector2Int, Enums.Directions>(); // [room position on grid, connected direction]
	public int _roomQuantityGoal;
	public int _roomQuantity;
	public int _roomBudget;

	void Awake() {
		GenerateDungeon();
	}

	void GenerateDungeon() {
		// Check if quantity of rooms fit in the floor size.
		int __initialRoomCost = GetRoomBudgetCost(_floorData.RoomPool.InitialRoom.GetComponent<Room>(), Enums.Directions.NONE);
		if (_floorData.RoomQuantity >= _floorData.FloorSize.x * _floorData.FloorSize.y - __initialRoomCost) {
			_roomQuantityGoal = _floorData.FloorSize.x * _floorData.FloorSize.y + __initialRoomCost - 1;
		}
		else {
			_roomQuantityGoal = _floorData.RoomQuantity + __initialRoomCost;
		}
		_roomQuantity = 0;
		_roomBudget = _roomQuantityGoal;

		CreateRooms();
	}

	void CreateRooms() {
		_rooms = new Room[_floorData.FloorSize.x * 2, _floorData.FloorSize.y * 2];

		// Spawn initial room. Initial room do not count for room quantity.
		_rooms[_floorData.FloorSize.x, _floorData.FloorSize.y] = Instantiate(_floorData.RoomPool.InitialRoom, Vector3.zero, Quaternion.identity).GetComponent<Room>();
		_rooms[_floorData.FloorSize.x, _floorData.FloorSize.y].gridPos = _floorData.FloorSize;
		_takenPositions.Add(_floorData.FloorSize, true);
		AddPositionsToFill(_rooms[_floorData.FloorSize.x, _floorData.FloorSize.y], Enums.Directions.NONE);
		_roomBudget -= GetRoomBudgetCost(_rooms[_floorData.FloorSize.x, _floorData.FloorSize.y], Enums.Directions.NONE);

		Debug.Log("positions to fill first size: " + _positionsToFill.Count);
		// Add rooms.
		int __iterations = 0;
		do {
			Dictionary<Vector2Int, Enums.Directions> __validPositions = new Dictionary<Vector2Int, Enums.Directions>(_positionsToFill);
			_positionsToFill.Clear();
			Debug.Log("validpositions size: " + __validPositions.Count);

			foreach (var __kvp in __validPositions) {
				//Debug.Log("position to fill: " + __kvp.Value);
				AddValidRoom(__kvp.Key, __kvp.Value);
			}

			__iterations++;
			if (__iterations > 1000) {
				Debug.LogWarning("iterations break!");
				break;
			}
		} while (_roomQuantity < _roomQuantityGoal);
	}

	void AddPositionsToFill(Room p_addedRoom, Enums.Directions p_connectedDirection) {
		if (p_addedRoom.DoorTop && p_connectedDirection != Enums.Directions.TOP) {
			Vector2Int __position = p_addedRoom.gridPos;
			__position.y++;
			if (!_positionsToFill.ContainsKey(__position)) {
				_positionsToFill.Add(__position, Enums.Directions.DOWN);
			}
			if (!_takenPositions.ContainsKey(__position)) {
				_takenPositions.Add(__position, false);
			}
		}
		if (p_addedRoom.DoorDown && p_connectedDirection != Enums.Directions.DOWN) {
			Vector2Int __position = p_addedRoom.gridPos;
			__position.y--;
			if (!_positionsToFill.ContainsKey(__position)) {
				_positionsToFill.Add(__position, Enums.Directions.TOP);
			}
			if (!_takenPositions.ContainsKey(__position)) {
				_takenPositions.Add(__position, false);
			}
		}
		if (p_addedRoom.DoorLeft && p_connectedDirection != Enums.Directions.LEFT) {
			Vector2Int __position = p_addedRoom.gridPos;
			__position.x--;
			if (!_positionsToFill.ContainsKey(__position)) {
				_positionsToFill.Add(__position, Enums.Directions.RIGHT);
			}
			if (!_takenPositions.ContainsKey(__position)) {
				_takenPositions.Add(__position, false);
			}
		}
		if (p_addedRoom.DoorRight && p_connectedDirection != Enums.Directions.RIGHT) {
			Vector2Int __position = p_addedRoom.gridPos;
			__position.x++;
			if (!_positionsToFill.ContainsKey(__position)) {
				_positionsToFill.Add(__position, Enums.Directions.LEFT);
			}
			if (!_takenPositions.ContainsKey(__position)) {
				_takenPositions.Add(__position, false);
			}
		}
	}

	int GetRoomBudgetCost(Room p_room, Enums.Directions p_connectedDirection) {
		int __cost = 0;
		if (p_room.DoorTop && p_connectedDirection != Enums.Directions.TOP) {
			if (_takenPositions.ContainsKey(new Vector2Int(p_room.gridPos.x, p_room.gridPos.y + 1))) {
				if (!_takenPositions[new Vector2Int(p_room.gridPos.x, p_room.gridPos.y + 1)]) {
					__cost++;
				}
				else {
					if (!_rooms[p_room.gridPos.x, p_room.gridPos.y + 1].DoorDown) {
						Debug.LogWarning("Não deveria conseguir chegar aqui");
						__cost++;
					}
				}
			}
			else {
				__cost++;
			}
		}
		if (p_room.DoorDown && p_connectedDirection != Enums.Directions.DOWN) {
			if (_takenPositions.ContainsKey(new Vector2Int(p_room.gridPos.x, p_room.gridPos.y - 1))) {
				if (!_takenPositions[new Vector2Int(p_room.gridPos.x, p_room.gridPos.y - 1)]) {
					__cost++;
				}
				else {
					if (!_rooms[p_room.gridPos.x, p_room.gridPos.y - 1].DoorTop) {
						Debug.LogWarning("Não deveria conseguir chegar aqui");
						__cost++;
					}
				}
			}
			else {
				__cost++;
			}
		}
		if (p_room.DoorLeft && p_connectedDirection != Enums.Directions.LEFT) {
			if (_takenPositions.ContainsKey(new Vector2Int(p_room.gridPos.x - 1, p_room.gridPos.y))) {
				if (!_takenPositions[new Vector2Int(p_room.gridPos.x - 1, p_room.gridPos.y)]) {
					__cost++;
				}
				else {
					if (!_rooms[p_room.gridPos.x - 1, p_room.gridPos.y].DoorRight) {
						Debug.LogWarning("Não deveria conseguir chegar aqui");
						__cost++;
					}
				}
			}
			else {
				__cost++;
			}
		}
		if (p_room.DoorRight && p_connectedDirection != Enums.Directions.RIGHT) {
			if (_takenPositions.ContainsKey(new Vector2Int(p_room.gridPos.x + 1, p_room.gridPos.y))) {
				if (!_takenPositions[new Vector2Int(p_room.gridPos.x + 1, p_room.gridPos.y)]) {
					__cost++;
				}
				else {
					if (!_rooms[p_room.gridPos.x + 1, p_room.gridPos.y].DoorLeft) {
						Debug.LogWarning("Não deveria conseguir chegar aqui");
						__cost++;
					}
				}
			}
			else {
				__cost++;
			}
		}

		return __cost;
	}

	void AddValidRoom(Vector2Int p_position, Enums.Directions p_connectedDirection) {
		// Check if room was already added.
		if (_takenPositions[p_position]) {
			return;
		}

		bool __validRoom = false;
		GameObject __room = null;
		int __iterations = 0;
		do {
			// Get random room from room pool.
			int __random;
			switch (p_connectedDirection) {
				case Enums.Directions.TOP:
					__random = Random.Range(0, _floorData.RoomPool.TopConnectedRooms.Length);
					__room = _floorData.RoomPool.TopConnectedRooms[__random];
					break;
				case Enums.Directions.DOWN:
					__random = Random.Range(0, _floorData.RoomPool.DownConnectedRooms.Length);
					__room = _floorData.RoomPool.TopConnectedRooms[__random];
					break;
				case Enums.Directions.LEFT:
					__random = Random.Range(0, _floorData.RoomPool.LeftConnectedRooms.Length);
					__room = _floorData.RoomPool.TopConnectedRooms[__random];
					break;
				case Enums.Directions.RIGHT:
					__random = Random.Range(0, _floorData.RoomPool.RightConnectedRooms.Length);
					__room = _floorData.RoomPool.TopConnectedRooms[__random];
					break;
				default:
					Debug.LogError("Room with no origin connection! Failed to add room.");
					return;
			}

			__iterations++;
			if (__iterations > 50) {
				Debug.LogWarning("add iterator break");
				break;
			}

			Debug.Log("neighbors test");
			// Avaliate the room.
			// By neighbors connections.
			Vector2Int __position = p_position;
			__position.y++; // Up.
			if (_takenPositions.ContainsKey(__position)) {
				if (_takenPositions[__position]) { // Exist neighbor.
					if (_rooms[__position.x, __position.y].DoorDown) { // Neighbor has connection?
						if (!__room.GetComponent<Room>().DoorTop) { // New room connects with neighbor?
							continue;
						}
					}
					else {
						if (__room.GetComponent<Room>().DoorTop) { // New room connects with neighbor?
							continue;
						}
					}
				}
			}
			__position = p_position;
			__position.y--; // Down.
			if (_takenPositions.ContainsKey(__position)) {
				if (_takenPositions[__position]) { // Exist neighbor.
					if (_rooms[__position.x, __position.y].DoorTop) { // Neighbor has connection?
						if (!__room.GetComponent<Room>().DoorDown) { // New room connects with neighbor?
							continue;
						}
					}
					else {
						if (__room.GetComponent<Room>().DoorDown) { // New room connects with neighbor?
							continue;
						}
					}
				}
			}
			__position = p_position;
			__position.x--; // Left.
			if (_takenPositions.ContainsKey(__position)) {
				if (_takenPositions[__position]) { // Exist neighbor.
					if (_rooms[__position.x, __position.y].DoorRight) { // Neighbor has connection?
						if (!__room.GetComponent<Room>().DoorLeft) { // New room connects with neighbor?
							continue;
						}
					}
					else {
						if (__room.GetComponent<Room>().DoorLeft) { // New room connects with neighbor?
							continue;
						}
					}
				}
			}
			__position = p_position;
			__position.x++; // Right.
			if (_takenPositions.ContainsKey(__position)) {
				if (_takenPositions[__position]) { // Exist neighbor.
					if (_rooms[__position.x, __position.y].DoorLeft) { // Neighbor has connection?
						if (!__room.GetComponent<Room>().DoorRight) { // New room connects with neighbor?
							continue;
						}
					}
					else {
						if (__room.GetComponent<Room>().DoorRight) { // New room connects with neighbor?
							continue;
						}
					}
				}
			}

			Debug.Log("cost test");
			// By cost.
			int __roomCost = GetRoomBudgetCost(__room.GetComponent<Room>(), p_connectedDirection);
			if (_roomBudget - __roomCost >= 0) {
				__validRoom = true;
			}
		} while (!__validRoom);

		// Add valid room.
		_rooms[p_position.x, p_position.y] = Instantiate(__room, new Vector3(p_position.x - _floorData.FloorSize.x, p_position.y - _floorData.FloorSize.y, 0), Quaternion.identity).GetComponent<Room>();
		_rooms[p_position.x, p_position.y].gridPos = p_position;
		_takenPositions[p_position] = true;
		AddPositionsToFill(_rooms[p_position.x, p_position.y], p_connectedDirection);
		_roomBudget -= GetRoomBudgetCost(_rooms[p_position.x, p_position.y], p_connectedDirection);
		_roomQuantity++;
	}
}
