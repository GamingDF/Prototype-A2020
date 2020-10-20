using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteeringBody {
	Vector2 GetVelocity();
	Vector2 SetVelocity(Vector2 p_velocity);
	float GetMaxVelocity();
	Vector2 GetPosition();
	float GetMass();
	float GetMaxSteering();
}
