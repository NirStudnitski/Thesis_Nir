using UnityEngine;
using System.Collections;

public class VehicleStat {

	const float TWO_PI = 6.2831853f;
	public Vector2 currentLocation, direction;
	//public Quaternion direction;
	public bool turnInitiate = false;
	public int turnPlan, lane, name, index, turnCounter = 0;
	public float speed,  halfLength, halfWidth;


	public VehicleStat (Vector2 locIn, int turnPlanIn, int laneIn, 
						int nameIn, float speedIn, 
		float halfLengthIn, float halfWidthIn, int indexIn, Vector2 dirIn)
	{
		currentLocation = locIn;
		turnPlan= turnPlanIn;
		lane = laneIn;
		name = nameIn;
		speed = speedIn;
		halfLength = halfLengthIn;
		halfWidth = halfWidthIn;
		index = indexIn;
		direction = dirIn;
	}

	public int GetName()
	{
		return name;
	}


	public void UpdateAndAdvance (float deltaTime)
	{
		

		currentLocation += direction * speed/deltaTime;
		if (turnPlan == 1) 
		{
			if (!turnInitiate ) if (-33f <currentLocation.x && currentLocation.x < 33f
				&& -33f < currentLocation.y && currentLocation.y < 33f)
				turnInitiate = true;
			if (turnInitiate && turnCounter <= 90) 
			{
				direction = RotateCW(direction, TWO_PI / 720f);
			
				turnCounter++;
			}
		} 
		else if (turnPlan == -1) 
		{
			if (!turnInitiate) if (-39f < currentLocation.x && currentLocation.x < 39f
				&& -39f < currentLocation.y && currentLocation.y < 39f)
				turnInitiate = true;
			if (turnInitiate && turnCounter <= 90) {
				direction = RotateCCW(direction, TWO_PI / 360f);

				turnCounter+=2;
			}
		}
	}

	private Vector2 RotateCCW (Vector2 vIn, float angle)
	{
		return new Vector2 (vIn.x * Mathf.Cos (angle) - vIn.y * Mathf.Sin (angle),
			vIn.x * Mathf.Sin (angle) + vIn.y * Mathf.Cos (angle));
	}

	private Vector2 RotateCW (Vector2 vIn, float angle)
	{
		angle = TWO_PI - angle;
		return new Vector2 (vIn.x * Mathf.Cos (angle) - vIn.y * Mathf.Sin (angle),
			vIn.x * Mathf.Sin (angle) + vIn.y * Mathf.Cos (angle));
	}
}
