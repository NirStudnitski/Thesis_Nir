using UnityEngine;
using System.Collections;

public class VehicleStat : MonoBehaviour {

	Vector2 spawnLocation;
	int turnPlan, lane, name;
	float speed;
	float spawnTime;
	float halfLength, halfWidth;

	public VehicleStat (Vector2 spawnIn, int turnPlanIn, int laneIn, 
						int nameIn, float speedIn, float timeIn, 
		float halfLengthIn, float halfWidthIn)
	{
		spawnLocation = spawnIn;
		turnPlan= turnPlanIn;
		lane = laneIn;
		name = nameIn;
		speed = speedIn;
		spawnTime = timeIn;
		halfLength = halfLengthIn;
		halfWidth = halfWidthIn;
	}
}
