using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	[Tooltip("Do not need to specify")]
	public Vector2Int gridPos;
	public bool DoorTop => _doorTop;
	public bool DoorDown => _doorDown;
	public bool DoorLeft => _doorLeft;
	public bool DoorRight => _doorRight;

	[SerializeField] bool _doorTop = false, _doorDown = false, _doorLeft = false, _doorRight = false;
}
