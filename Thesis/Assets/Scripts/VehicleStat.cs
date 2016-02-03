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
		int turnMultiplier = 5;
		for (int i=0;i<multiplier && !GameController.futureCollisionDetected;i++)
		{
			
			currentLocation += direction* speed*deltaTime*turnMultiplier;

			if (turnPlan == 1) 
			{
				if (!turnInitiate ) if (-33f <currentLocation.x && currentLocation.x < 33f
					&& -33f < currentLocation.y && currentLocation.y < 33f)
					turnInitiate = true;
				if (turnInitiate && turnCounter <= 90) 
				{
					if (turnCounter>0) direction = RotateCCW(direction, TWO_PI / (360f/turnMultiplier));

					turnCounter+=turnMultiplier;
				}
			} 
			else if (turnPlan == -1) 
			{
				if (!turnInitiate) if (-39f < currentLocation.x && currentLocation.x < 39f
					&& -39f < currentLocation.y && currentLocation.y < 39f)
					turnInitiate = true;
				if (turnInitiate && turnCounter <= 90) 
				{
					if (turnCounter>0) direction = RotateCW(direction, TWO_PI / (180f/turnMultiplier));

					turnCounter+=(2*turnMultiplier);
				}
			}

			if (!GameController.doneWithCheck) if (-290f > currentLocation.x || currentLocation.x > 290f
			    || -290f > currentLocation.y || currentLocation.y > 290f) 
				GameController.doneWithCheck = true;

			//now check collision
			if (name == GameController.nameBeingChecked && !GameController.futureCollisionDetected) for (int j=0;j<GameController.numCloseV;j++)
			{
				
				if (name!= GameController.futureVehicles[j].name)
				{
					
						if (QuickCollisionDetection (currentLocation, GameController.futureVehicles [j].currentLocation,
							    halfDiag, GameController.futureVehicles [j].halfDiag)) 
							if (FullCollisionDetection (currentLocation, GameController.futureVehicles [j].currentLocation, 
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

	//aDiag represents half the diagonal 
	private bool QuickCollisionDetection (Vector2 aIn, Vector2 bIn, float aDiag, float bDiag)
	{
		return (aIn.x -bIn.x)*(aIn.x -bIn.x) + (aIn.y -bIn.y)*(aIn.y -bIn.y) < (aDiag + bDiag)*(aDiag + bDiag);

	}

	//aWidth represents half the width of a vehicle, aLength represents half the length
	private bool FullCollisionDetection (Vector2 aIn, Vector2 bIn, float aWidth, float bWidth,
		float aLength, float bLength, Vector2 aDir, Vector2 bDir)
	{
		bool rtn = false;
		if (Mathf.Pow(aIn.x - bIn.x,2) + Mathf.Pow(aIn.y - bIn.y,2)  
			< Mathf.Pow(aWidth + bWidth,2))
			rtn = true;
		else 
		{
			Vector2 aToB = new Vector2 (aIn.x - bIn.x, aIn.y - bIn.y);

			Vector2 bTL = new Vector2 (-bWidth, bLength);
			Vector2 bBR = new Vector2 (bWidth, -bLength);

			float angle = bDir.y;

			aToB = RotateCCW (aToB, angle);

			Vector2 aDirV2 = RotateCCW (new Vector2 (0f, 1f), TWO_PI - aDir.y);
			aDirV2 = RotateCCW (aDirV2, angle);
			Vector2 aPerp = RotateCCW (aDirV2, TWO_PI / 4);
			aDirV2 *= aLength;
			aPerp *= aWidth;

			//generate a's points in the new system
			Vector2[] aPoints = {aToB + aDirV2 + aPerp, aToB + aDirV2 - aPerp, 
				aToB - aDirV2 + aPerp, aToB - aDirV2 - aPerp
			};


			for (int i = 0; i < 4; i++) {
				if (aPoints [i].x < bBR.x && aPoints [i].x > bTL.x)
				if (aPoints [i].y > bBR.y && aPoints [i].y < bTL.y) {
					rtn = true;
					i += 4;
				}
			}
		}
		return rtn;
	}

}
