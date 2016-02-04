using UnityEngine;
using System.Collections;

public class VehicleStat  {

	const float TWO_PI = 6.2831853f;
	public Vector2 currentLocation, direction;
	public bool turnInitiate = false, isQuad = false;
	public int turnPlan, lane, name, index, turnCounter = 0, type;
	public float speed,  halfLength, halfWidth, halfDiag;



	public VehicleStat (Vector2 locIn, int turnPlanIn, int laneIn, 
						int nameIn, float speedIn, 
		float halfLengthIn, float halfWidthIn, int indexIn, Vector2 dirIn, 
		int typeIn, bool isQuadIn, float halfDiagIn)
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
		type = typeIn;
		isQuad = isQuadIn;
		halfDiag = halfDiagIn;


	}

	public int GetName()
	{
		return name;
	}


	public void UpdateAndAdvance (float deltaTime)
	{
		

		currentLocation += direction* speed*deltaTime;
		if (turnPlan == 1) 
		{
			if (!turnInitiate ) if (-33f <currentLocation.x && currentLocation.x < 33f
				&& -33f < currentLocation.y && currentLocation.y < 33f)
				turnInitiate = true;
			if (turnInitiate && turnCounter <= 90) 
			{
				if (turnCounter>0) direction = RotateCCW(direction, TWO_PI / 360f);
			
				turnCounter++;
			}
		} 
		else if (turnPlan == -1) 
		{
			if (!turnInitiate) if (-39f < currentLocation.x && currentLocation.x < 39f
				&& -39f < currentLocation.y && currentLocation.y < 39f)
				turnInitiate = true;
			if (turnInitiate && turnCounter <= 90) 
			{
				if (turnCounter>0) direction = RotateCW(direction, TWO_PI / 180f);

				turnCounter+=2;
			}
		}
	}

	//for fast updates of quads
	public void UpdateAndAdvance (float deltaTime, int multiplier)
	{
		
		for (int i=0;i<multiplier && !GameController.futureCollisionDetected;i++)
		{
			
			currentLocation += direction* speed*deltaTime*multiplier;

			if (turnPlan == 1) 
			{
				if (!turnInitiate ) if (-33f <currentLocation.x && currentLocation.x < 33f
					&& -33f < currentLocation.y && currentLocation.y < 33f)
					turnInitiate = true;
				if (turnInitiate && turnCounter <= 90) 
				{
					if (turnCounter>0) direction = RotateCCW(direction, TWO_PI / (360f/multiplier));

					turnCounter+=multiplier;
				}
			} 
			else if (turnPlan == -1) 
			{
				if (!turnInitiate) if (-39f < currentLocation.x && currentLocation.x < 39f
					&& -39f < currentLocation.y && currentLocation.y < 39f)
					turnInitiate = true;
				if (turnInitiate && turnCounter <= 90) 
				{
					if (turnCounter>0) direction = RotateCW(direction, TWO_PI / (180f/multiplier));

					turnCounter+=(2*multiplier);
				}
			}

			if (!GameController.doneWithCheck) if (-290f > currentLocation.x || currentLocation.x > 290f
			    || -290f > currentLocation.y || currentLocation.y > 290f) 
				GameController.doneWithCheck = true;

			//now check collision
			if (name == GameController.nameBeingChecked && !GameController.futureCollisionDetected) 
				for (int j=0;j<GameController.numCloseV;j++)
				{
					
					if (name!= GameController.futureVehicles[j].name)
					{
						
						if (GameController.QuickCollisionDetection (currentLocation, GameController.futureVehicles [j].currentLocation,
								    halfDiag, GameController.futureVehicles [j].halfDiag))

							if (GameController.FullCollisionDetection (currentLocation, GameController.futureVehicles [j].currentLocation, 
									    halfWidth, GameController.futureVehicles [j].halfWidth,
									    halfLength, GameController.futureVehicles [j].halfLength,
									    direction, GameController.futureVehicles [j].direction))
							{
								Debug.Log ("Collision");
								GameController.futureCollisionDetected = true;
								GameController.doneWithCheck = true;
							}
							
					}
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
