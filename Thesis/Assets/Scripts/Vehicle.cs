using UnityEngine;
using System.Collections;
using System;

public class Vehicle : MonoBehaviour {

	public float speed;
	private static float TWO_PI = 6.283185307f, HALF_PI = 1.570796326f, TO_DEG = 360f/TWO_PI;

	//these are half the length and width of each vehicle
	public float size, width;
	public float theta = 0, extra; // theta is the roation angle for turning
	private Vector3 XYZdir = new Vector3 (0, 0, 0), center;
	private Vector2 XYdir;
	public int lane;
	public int type;
	private bool turnInitiate = false, noCollision = true, turnDone = false, initiate = true;
	public int turnPlan; // 0 = keep going straight, 1 = turn right, -1 = turn left 
	private float turnCounter = 1;



	void Start () {
	

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (initiate)
		{
			XYZdir.x = XYdir.x;
			XYZdir.z = XYdir.y;
			initiate = false;
		}
			
		if (!GameController.pause && noCollision) 
		{
			if (turnPlan ==0) transform.position += XYZdir * speed * GameController.DELTA;
			else if (turnPlan == 1) // turn right
			{
				switch (lane)
				{
					case (1):
						if (!turnInitiate)
						if (transform.position.x > -39f)
						{
							extra = transform.position.x + 39f;
							turnInitiate = true;
						}
						
						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rR;
							XYdir = RotateCW (new Vector2 (1, 0), theta);
							
							if (theta < HALF_PI)
							{
								transform.position = center + RotateCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rR;
								XYdir = new Vector2 (0, -1);
								transform.position = center + RotateCW (GameController.rotators [lane], HALF_PI)
													 + extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, 90f + (lane / 2) * 90f, 0f);

								turnDone = true;
							}
						}
						break;

					case (3):
						if (!turnInitiate)
						if (transform.position.z < 39f)
						{
							extra = 39f - transform.position.z;
							turnInitiate = true;
						}
						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rR;
							XYdir = RotateCW (new Vector2 (0, -1), theta);

							if (theta < HALF_PI)
							{
								transform.position = center + RotateCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rR;
								XYdir = new Vector2 (-1, 0);
								transform.position = center + RotateCW (GameController.rotators [lane], HALF_PI)
									+ extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, 90f + (lane / 2) * 90f, 0f);

								turnDone = true;
							}
						}
						break;

					case (5):
						if (!turnInitiate)
						if (transform.position.x < 39f)
						{
							extra = 39f - transform.position.x;
							turnInitiate = true;
						}

						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rR;
							XYdir = RotateCW (new Vector2 (-1, 0), theta);

							if (theta < HALF_PI)
							{
								transform.position = center + RotateCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rR;
								XYdir = new Vector2 (0, 1);
								transform.position = center + RotateCW (GameController.rotators [lane], HALF_PI)
									+ extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, 90f + (lane / 2) * 90f, 0f);

								turnDone = true;
							}
						}
						break;

					case (7):
						if (!turnInitiate)
						if (transform.position.z > -39f)
						{
							extra = transform.position.z + 39f;
							turnInitiate = true;
						}

						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rR;
							XYdir = RotateCW (new Vector2 (0, 1), theta);

							if (theta < HALF_PI)
							{
								transform.position = center + RotateCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rR;
								XYdir = new Vector2 (1, 0);
								transform.position = center + RotateCW (GameController.rotators [lane], HALF_PI)
									+ extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, 90f + (lane / 2) * 90f, 0f);

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
						if (transform.position.x > -33f)
						{
							extra = transform.position.x + 33f;
							turnInitiate = true;
						}
						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rL;
							XYdir = RotateCCW (new Vector2 (1, 0), theta);

							if (theta < HALF_PI)
							{
								transform.position = center + RotateCCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, -theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rL;
								XYdir = new Vector2 (0, 1);
								transform.position = center + RotateCCW (GameController.rotators [lane], HALF_PI)
									+ extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, -90f + (lane / 2) * 90f, 0f);

								turnDone = true;
							}
						}
						break;

					case (2):
						if (!turnInitiate)
						if (transform.position.z < 33f)
						{
							extra = 33f - transform.position.z;
							turnInitiate = true;
						}

						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rL;
							XYdir = RotateCCW (new Vector2 (0, -1), theta);

							if (theta < HALF_PI)
							{
								transform.position = center + RotateCCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, -theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rL;
								XYdir = new Vector2 (1, 0);
								transform.position = center + RotateCCW (GameController.rotators [lane], HALF_PI)
									+ extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, -90f + (lane / 2) * 90f, 0f);

								turnDone = true;
							}
						}
						break;

					case (4):
						if (!turnInitiate)
						if (transform.position.x < 33f)
						{
							extra = 33f - transform.position.x;
							turnInitiate = true;
						}

						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rL;
							XYdir = RotateCCW (new Vector2 (-1, 0), theta);

							if (theta < HALF_PI)
							{
								transform.position = center + RotateCCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, -theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rL;
								XYdir = new Vector2 (0, -1);
								transform.position = center + RotateCCW (GameController.rotators [lane], HALF_PI)
									+ extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, -90f + (lane / 2) * 90f, 0f);

								turnDone = true;
							}
						}
						break;

					case (6):
						if (!turnInitiate)
						if (transform.position.z > -33f)
						{
							extra = transform.position.z + 33f;
							turnInitiate = true;
						}

						if (!turnInitiate || turnDone)
						{
							XYZdir.x = XYdir.x;
							XYZdir.z = XYdir.y;
							transform.position += XYZdir * speed * GameController.DELTA;
						} 
						else
						{
							
							float arcLength = extra + speed * GameController.DELTA * turnCounter;
							theta = arcLength / GameController.rL;
							XYdir = RotateCCW (new Vector2 (0, 1), theta);

							if (theta < HALF_PI)
							{
								transform.position = center + RotateCCW (GameController.rotators [lane], theta);	
								transform.rotation = Quaternion.Euler (0f, -theta*TO_DEG + (lane / 2) * 90f, 0f);
								turnCounter++;
							} 
							else
							{
								float extraAngle = theta - HALF_PI;
								float extraDist = extraAngle*GameController.rL;
								XYdir = new Vector2 (-1, 0);
								transform.position = center + RotateCCW (GameController.rotators [lane], HALF_PI)
									+ extraDist * (new Vector3(XYdir.x,0,XYdir.y));
								transform.rotation = Quaternion.Euler (0f, -90f + (lane / 2) * 90f, 0f);

								turnDone = true;
							}
						}
						break;
				}
			}

		}


	}

	public void SetSpeed(float x)
	{
		speed = x;
	}

	public void SetDirection(Vector2 dirIn)
	{
		XYdir = dirIn;
	}

	public void SetSize(float x)
	{
		size = x;
	}

	public float GetSpeed()
	{
		return speed;
	}

	public float GetSize()
	{
		return size;
	}

	public void SetTurnPlan(int x)
	{
		turnPlan = x;
	}

	public void SetLane(int x)
	{
		lane = x;
	}

	public int GetLane()
	{
		return lane;
	}

	public void SetType(int x)
	{
		type = x;
	}

	public void SetCenter(Vector3 centerIn)
	{
		center = centerIn;
	}

	public int GetType()
	{
		return type;
	}

	public void CollisionHappened()
	{
		noCollision = false;
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

	private Vector3 RotateCW (Vector3 vIn, float angle)
	{
		angle = TWO_PI - angle;
		return new Vector3 (vIn.x * Mathf.Cos (angle) - vIn.z * Mathf.Sin (angle),0,
			vIn.x * Mathf.Sin (angle) + vIn.z * Mathf.Cos (angle));
	}

	private Vector3 RotateCCW (Vector3 vIn, float angle)
	{
		return new Vector3 (vIn.x * Mathf.Cos (angle) - vIn.z * Mathf.Sin (angle),0,
			vIn.x * Mathf.Sin (angle) + vIn.z * Mathf.Cos (angle));
	}
}
