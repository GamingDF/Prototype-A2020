using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon Room Pool", menuName = "ScriptableObjects/Dungeon/Room Pool")]
public class DungeonRoomPoolData : ScriptableObject {
	public GameObject InitialRoom => _initialRoom;
	public GameObject[] TopConnectedRooms => _topConnectedRooms;
	public GameObject[] DownConnectedRooms => _downConnectedRooms;
	public GameObject[] LeftConnectedRooms => _leftConnectedRooms;
	public GameObject[] RightConnectedRooms => _rightConnectedRooms;

	[SerializeField] GameObject _initialRoom = null;
	[SerializeField] GameObject[] _topConnectedRooms = null;
	[SerializeField] GameObject[] _downConnectedRooms = null;
	[SerializeField] GameObject[] _leftConnectedRooms = null;
	[SerializeField] GameObject[] _rightConnectedRooms = null;
}
