using UnityEngine;
using System.Collections;

public class VehicleStat  {

	const float TWO_PI = 6.2831853f, HALF_PI = 1.570796326f, TO_DEG = 360f/TWO_PI;
	public Vector2 currentLocation, direction, center;
	public bool turnInitiate = false, isQuad = false, turnDone = false, stopping = false;
	public bool turnIn = false, turnD = false, stopped2 = false;
	public int turnPlan, lane, name, index, turnCounter = 0, type, inLine = -255;
	public float speed,  halfLength, halfWidth, halfDiag, extra, theta,  accel = 0, stdAccel = 0.3f;




	public VehicleStat (Vector2 locIn, int turnPlanIn, int laneIn, 
						int nameIn, float speedIn, 
		float halfLengthIn, float halfWidthIn, int indexIn, Vector2 dirIn, 
		int typeIn, bool isQuadIn, float halfDiagIn, Vector2 centerIn)
	{
		currentLocation = locIn;
		turnPlan= turnPlanIn;
		lane = laneIn;
		name = nameIn;
		speed = speedIn;
		halfLength = halfLengthIn;
		halfWidth = halfWidthIn;
		index = indexIn; // the index in VList of the vehicle
		direction = dirIn;
		type = typeIn;
		isQuad = isQuadIn;
		halfDiag = halfDiagIn;
		center = centerIn;



	}

	public int GetName()
	{
		return name;
	}


	public void UpdateAndAdvance (float deltaTime)
	{
		

		if (turnPlan ==0) currentLocation += direction * speed * GameController.DELTA;
		else if (turnPlan == 1) // turn right
		{
			switch (lane)
			{
				case (1):
					if (!turnInitiate)
					if (currentLocation.x > -39f)
					{
						extra = currentLocation.x + 39f;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (1, 0), theta);
						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
							+ extraDist * direction;

							turnDone = true;
						}
					}
					break;

				case (3):
					if (!turnInitiate)
					if (currentLocation.y < 39f)
					{
						extra = 39f - currentLocation.y;
						turnInitiate = true;
					}
					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (5):
					if (!turnInitiate)
					if (currentLocation.x < 39f)
					{
						extra = 39f - currentLocation.x;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (7):
					if (!turnInitiate)
					if (currentLocation.y > -39f)
					{
						extra = currentLocation.y + 39f;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}
			} 
			else if (turnPlan == -1) 
			{
				switch (lane)
				{
				case (0):
					if (!turnInitiate)
					if (currentLocation.x > -33f)
					{
						extra = currentLocation.x + 33f;
						turnInitiate = true;
					}
					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (2):
					if (!turnInitiate)
					if (currentLocation.y < 33f)
					{
						extra = 33f - currentLocation.y;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (4):
					if (!turnInitiate)
					if (currentLocation.x < 33f)
					{
						extra = 33f - currentLocation.x;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (6):
					if (!turnInitiate)
					if (currentLocation.y > -33f)
					{
						extra = currentLocation.y + 33f;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}


		}


	}

	//for fast updates of quads
	public void UpdateAndAdvance (float deltaTime, int multiplier)
	{
		
		for (int i=0;i<multiplier ;i++)
		{
			

			if (turnPlan ==0) currentLocation += direction * speed * GameController.DELTA;
			else if (turnPlan == 1) // turn right
			{
				switch (lane)
				{
				case (1):
					if (!turnInitiate)
					if (currentLocation.x > -39f)
					{
						extra = currentLocation.x + 39f;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;

							turnDone = true;
						}
					}
					break;

				case (3):
					if (!turnInitiate)
					if (currentLocation.y < 39f)
					{
						extra = 39f - currentLocation.y;
						turnInitiate = true;
					}
					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (5):
					if (!turnInitiate)
					if (currentLocation.x < 39f)
					{
						extra = 39f - currentLocation.x;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (7):
					if (!turnInitiate)
					if (currentLocation.y > -39f)
					{
						extra = currentLocation.y + 39f;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rR;
						direction = RotateCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rR;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}
			} 
			else if (turnPlan == -1) 
			{
				switch (lane)
				{
				case (0):
					if (!turnInitiate)
					if (currentLocation.x > -33f)
					{
						extra = currentLocation.x + 33f;
						turnInitiate = true;
					}
					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (2):
					if (!turnInitiate)
					if (currentLocation.y < 33f)
					{
						extra = 33f - currentLocation.y;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (4):
					if (!turnInitiate)
					if (currentLocation.x < 33f)
					{
						extra = 33f - currentLocation.x;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;

				case (6):
					if (!turnInitiate)
					if (currentLocation.y > -33f)
					{
						extra = currentLocation.y + 33f;
						turnInitiate = true;
					}

					if (!turnInitiate || turnDone)
					{
						currentLocation += direction * speed * GameController.DELTA;
					} 
					else
					{

						float arcLength = extra + speed * GameController.DELTA * turnCounter;
						theta = arcLength / GameController.rL;
						direction = RotateCCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*GameController.rL;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCCW (GameController.rotators2 [lane], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}


			}

			if (!GameController.doneWithCheck)
			if (-290f > currentLocation.x || currentLocation.x > 290f
			                                       || -290f > currentLocation.y || currentLocation.y > 290f)
				GameController.doneWithCheck = true;
			
			string bla = "";
			//now check collision
			if (name == GameController.nameBeingChecked ) 
			{
				bla += "name = " + name + "\n";
				if (!GameController.futureCollisionDetected)
				{
					bla += "future collision = " + GameController.futureCollisionDetected + "\n";
					for (int j = 0; j < GameController.numCloseV && !GameController.futureCollisionDetected; j++)
					{
					
						if (name != GameController.futureVehicles [j].name)
						{
							/*
							bla +=name +"!="+ GameController.futureVehicles [j].name + "\n";
							bla += "current Location = (" + currentLocation.x + ", " + currentLocation.y + ")\n";
							bla += "B current Location = (" + GameController.futureVehicles [j].currentLocation.x + ", " + GameController.futureVehicles [j].currentLocation.y + ")\n";
							bla += "diag = " + halfDiag+ ")\n";
							bla += "B diag = " + GameController.futureVehicles [j].halfDiag+ ")\n";
							*/
							if (GameController.QuickCollisionDetection (currentLocation, GameController.futureVehicles [j].currentLocation,
									    halfDiag, GameController.futureVehicles [j].halfDiag))
							{
								bla+="quick was positive\n";
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
			} //Debug.Log (bla);
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
