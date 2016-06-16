using UnityEngine;
using System.Collections;


// a data structure for the vehicle simulation
using System;


public class VehicleList : MonoBehaviour{

	public int lastIndex = -1, numTurning = -1;
	GameObject temp2 = null;

	public VehicleStat[] activeVehicles = new VehicleStat[GameController.ACTIVE_V_MAX];

	public VehicleList()
	{
		for (int i = 0; i < GameController.ACTIVE_V_MAX; i++)
			activeVehicles [i] = null;
	}

	void Start () 
	{
		
	}

	public void Add(Vector2 spawnIn, int turnPlanIn, int laneIn, 
					int nameIn, float speedIn,  
		float halfLengthIn, float halfWidthIn, Vector2 dirIn, int typeIn, float diagIn)
	{
		activeVehicles [++lastIndex] = new VehicleStat (spawnIn, turnPlanIn, laneIn, 
								nameIn, speedIn,  halfLengthIn, halfWidthIn, 
			lastIndex, dirIn, typeIn, false, diagIn, GameController.centersOfRot[laneIn]);
		
		
	}	

	public void Remove(int nameIn)
	{
		int index = -1;
		while (++index <= lastIndex) if (activeVehicles [index].name == nameIn) break;
		while (++index <= lastIndex) 
		{
			activeVehicles [index - 1] = activeVehicles [index];
			activeVehicles [index - 1].index--;
		}
		activeVehicles [lastIndex] = null;
		lastIndex--;
	}

	public void PrintList()
	{
		int index = -1;
		string listS = "Last Index: " + lastIndex + " Active vehicles: ";

		while (++index <= lastIndex) listS +=activeVehicles [index].name 
				+ " turning=" +activeVehicles [index].turnPlan+ ", the index is " + activeVehicles [index].index + "\n";

		Debug.Log (listS + "\n");
	}

	public void PrintListLocations()
	{
		int index = -1;
		string listS = "Last Index: " + lastIndex + " Locations: ";

		while (++index <= lastIndex) 
		{
			Vector2 tempLoc = activeVehicles [index].currentLocation;
			listS += "(" + tempLoc.x + ", " + tempLoc.y + "), ";
		}

		Debug.Log (listS + "\n");
	}

	public void PrintListDirections()
	{
		int index = -1;
		string listS = "Last Index: " + lastIndex + " Directions: ";

		while (++index <= lastIndex) 
		{
			Vector2 tempLoc = activeVehicles [index].direction;
			listS += "(" + tempLoc.x + ", " + tempLoc.y + "), ";
		}

		Debug.Log (listS + "\n");
	}

	public VehicleStat GetStatOfIndex(int i)
	{
		return activeVehicles[i];
	}

	public int GetIndex(int index)
	{
		return activeVehicles [index].index;
	}
		
	public void UpdateList(float deltaTime)
	{
		for (int i=0;i <= lastIndex; i++) activeVehicles [i].UpdateAndAdvance (deltaTime);
	}

	public void OrderQueue(int lane)
	{
		int indexer = -1;
		GameController.doneTurning[lane] = false;
		for (int k = 0; k <= lastIndex; k++)
		activeVehicles [k].inLine = -255;
		numTurning = -1;

		//take care of ordering which cars are in line for the traffic lights

		for (int i = 0; i <= lastIndex; i++)
		{
			// conditionals for traffic control, if stopped, mark first five to turn left
			switch (lane)
			{
			case (1):
			case(0):
				if (GameController.changeLights)
				{
					if (activeVehicles [i].currentLocation.x < -50f)
					{
						if (i == 0)
							activeVehicles [i].inLine = 0;
						else
							activeVehicles [i].inLine = activeVehicles [i - 1].inLine + 1;
						if (activeVehicles [i].inLine < 0)
							activeVehicles [i].inLine = 0;

					}
				
				}
				break;

			case (4):
			case(5):
				if (GameController.changeLights)
				{
					if (activeVehicles [i].currentLocation.x > 50f)
					{
						if (i == 0)
							activeVehicles [i].inLine = 0;
						else
							activeVehicles [i].inLine = activeVehicles [i - 1].inLine + 1;
						if (activeVehicles [i].inLine < 0)
							activeVehicles [i].inLine = 0;

					}

				}
				break;

			}

			if (lane%2==0)
			if (activeVehicles [i].inLine >= 0 && activeVehicles [i].inLine < 5)
			{
				GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
				temp.GetComponent<Vehicle> ().SetTurnPlan(-1);
				numTurning = activeVehicles [i].inLine;
				indexer = i;
			}
		}
		if (indexer >= 0)
		{
			temp2 = GameObject.FindGameObjectWithTag ("V " + activeVehicles [indexer].name);

		}//else temp2 = null;
	}

	public void UpdateSpeeds(int lane, int frame)
	{	


		if (frame % 30 == 0)
		switch (lane)
		{
			case (0):
				if (temp2 != null)
				{
					Debug.Log (temp2.GetComponent<Vehicle> ().transform.position.x);
					if (temp2.GetComponent<Vehicle> ().transform.position.x > 0)
					{
						GameController.doneTurning [lane] = true;
						
					}
				} 
				break;

		case (4):
			if (temp2 != null)
			{
				Debug.Log (temp2.GetComponent<Vehicle> ().transform.position.x);
				if (temp2.GetComponent<Vehicle> ().transform.position.x < 0)
				{
					GameController.doneTurning [lane] = true;

				}
			} 
			break;
		}

				
			





		for (int i = 0; i <= lastIndex; i++)
		{
			// conditionals for traffic control, if stopped, mark first five to turn left
			switch (lane)
			{
			case (1):
			case(0):
				if (activeVehicles [i].currentLocation.x < -40f)
				{	
					if (activeVehicles [i].inLine == 0 && activeVehicles [i].currentLocation.x > -50f)
					{
						if (GameController.changeLights)
						{
							if (!activeVehicles [i].stopping)
							{
								activeVehicles [i].stopping = true;
								activeVehicles [i].accel = GameController.everyonesSpeed * GameController.everyonesSpeed
								/ (2f * (42f - Math.Abs (activeVehicles [i].currentLocation.x)));
								activeVehicles [i].accel *= GameController.DELTA;
							} else
							{
								if (activeVehicles [i].speed > 0)
									activeVehicles [i].speed += activeVehicles [i].accel;
								else
									activeVehicles [i].speed = 0;
								GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
								temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
							}
						} else
						{
							if (lane == 0)
							{
								activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
								if (activeVehicles [i].speed > GameController.everyonesSpeed)
									activeVehicles [i].speed = GameController.everyonesSpeed;
								else if (activeVehicles [i].speed < 0)
									activeVehicles [i].speed = 0;
						
								GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
								temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
							} else
							{
								if (GameController.doneTurning [4])
								{
									activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
									if (activeVehicles [i].speed > GameController.everyonesSpeed)
										activeVehicles [i].speed = GameController.everyonesSpeed;
									else if (activeVehicles [i].speed < 0)
										activeVehicles [i].speed = 0;

									GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
									temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
								}
							}
						}

					} else
					{
						if (i != 0)
						{
							float dist = Math.Abs (activeVehicles [i - 1].currentLocation.x - activeVehicles [i].currentLocation.x);
							dist -= (activeVehicles [i - 1].halfLength + activeVehicles [i].halfLength);
							float repulsion = (dist > 20f || activeVehicles [i - 1].currentLocation.y < -20f ? 0 : (float)Math.Pow (2, (20f - dist) / 5) - 1f);

							activeVehicles [i].speed += activeVehicles [i].stdAccel * (2 - repulsion);
							if (activeVehicles [i].speed > GameController.everyonesSpeed)
								activeVehicles [i].speed = GameController.everyonesSpeed;
							else if (activeVehicles [i].speed < 0)
								activeVehicles [i].speed = 0;

							GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
							temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
									
						} else
						{
							if (activeVehicles [i].stopping)
							{
								if (lane == 1 && !GameController.doneTurning [4])
									activeVehicles [i].speed = 0;
								else
								{
									activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
									if (activeVehicles [i].speed > GameController.everyonesSpeed)
										activeVehicles [i].speed = GameController.everyonesSpeed;
									else if (activeVehicles [i].speed < 0)
										activeVehicles [i].speed = 0;
								}
							} else
							{
								activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
								if (activeVehicles [i].speed > GameController.everyonesSpeed)
									activeVehicles [i].speed = GameController.everyonesSpeed;
								else if (activeVehicles [i].speed < 0)
									activeVehicles [i].speed = 0;
							}
							GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
							temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
						}
					}

				} else
				{
					activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
					if (activeVehicles [i].speed > GameController.everyonesSpeed)
						activeVehicles [i].speed = GameController.everyonesSpeed;
					else if (activeVehicles [i].speed < 0)
						activeVehicles [i].speed = 0;

					GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
					temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
				}


				break;

			case (4):
			case(5):
				if (activeVehicles [i].currentLocation.x > 40f)
				{	
					if (activeVehicles [i].inLine == 0 && activeVehicles [i].currentLocation.x < 50f)
					{
						if (GameController.changeLights)
						{
							if (!activeVehicles [i].stopping)
							{
								activeVehicles [i].stopping = true;
								activeVehicles [i].accel = GameController.everyonesSpeed * GameController.everyonesSpeed
								/ (2f * (42f - Math.Abs (activeVehicles [i].currentLocation.x)));
								activeVehicles [i].accel *= GameController.DELTA;
							} else
							{
								if (activeVehicles [i].speed > 0)
									activeVehicles [i].speed += activeVehicles [i].accel;
								else
									activeVehicles [i].speed = 0;
								GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
								temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
							}
						}else
						{
							if (lane == 4)
							{
								activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
								if (activeVehicles [i].speed > GameController.everyonesSpeed)
									activeVehicles [i].speed = GameController.everyonesSpeed;
								else if (activeVehicles [i].speed < 0)
									activeVehicles [i].speed = 0;

								GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
								temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
							} else
							{
								if (GameController.doneTurning [0])
								{
									activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
									if (activeVehicles [i].speed > GameController.everyonesSpeed)
										activeVehicles [i].speed = GameController.everyonesSpeed;
									else if (activeVehicles [i].speed < 0)
										activeVehicles [i].speed = 0;

									GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
									temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
								}
							}
						}

					} else
					{
						if (i != 0)
						{
							float dist = Math.Abs (activeVehicles [i - 1].currentLocation.x - activeVehicles [i].currentLocation.x);
							dist -= (activeVehicles [i - 1].halfLength + activeVehicles [i].halfLength);
							float repulsion = (dist > 20f || activeVehicles [i - 1].currentLocation.y > 20f ? 0 : (float)Math.Pow (2, (20f - dist) / 5) - 1f);

							activeVehicles [i].speed += activeVehicles [i].stdAccel * (2 - repulsion);
							if (activeVehicles [i].speed > GameController.everyonesSpeed)
								activeVehicles [i].speed = GameController.everyonesSpeed;
							else if (activeVehicles [i].speed < 0)
								activeVehicles [i].speed = 0;

							GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
							temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);

						} else
						{
							if (activeVehicles [i].stopping)
							{
								if (lane == 5 && !GameController.doneTurning [0])
									activeVehicles [i].speed = 0;
								else
								{
									activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
									if (activeVehicles [i].speed > GameController.everyonesSpeed)
										activeVehicles [i].speed = GameController.everyonesSpeed;
									else if (activeVehicles [i].speed < 0)
										activeVehicles [i].speed = 0;
								}
							} else
							{
								activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
								if (activeVehicles [i].speed > GameController.everyonesSpeed)
									activeVehicles [i].speed = GameController.everyonesSpeed;
								else if (activeVehicles [i].speed < 0)
									activeVehicles [i].speed = 0;
							}
							GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
							temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
						}
					}

				} else
				{
					AccelNormal (i);
				}


				break;
			}


		}
	}

	private void AccelNormal (int i)
	{
		activeVehicles [i].speed += activeVehicles [i].stdAccel * 2;
		if (activeVehicles [i].speed > GameController.everyonesSpeed)
			activeVehicles [i].speed = GameController.everyonesSpeed;
		else if (activeVehicles [i].speed < 0)
			activeVehicles [i].speed = 0;

		GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
		temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
	}

	public float GetSpeed (int i)
	{
		return activeVehicles[i].speed;
	}

	public void SetSpeed (int i, float speedIn)
	{
		activeVehicles[i].speed = speedIn;
	}

	public float GetLength (int i)
	{
		return activeVehicles[i].halfLength;
	}

	public float GetWidth (int i)
	{
		return activeVehicles[i].halfWidth;
	}

	public Vector2 GetCurrentPosition (int i)
	{
		return activeVehicles[i].currentLocation;
	}

	public Vector2 GetDirection (int i)
	{
		return activeVehicles [i].direction;
	}

	public int GetTurnCounter (int i)
	{
		return activeVehicles [i].turnCounter;
	}

	public int GetLane (int i)
	{
		return activeVehicles [i].lane;
	}

	public int GetName (int i)
	{
		return activeVehicles [i].name;
	}

	public int GetType (int i)
	{
		return activeVehicles [i].type;
	}

	public int GetTurnPlan (int i)
	{
		return activeVehicles [i].turnPlan;
	}

	public bool GetTurnInitiate (int i)
	{
		return activeVehicles [i].turnInitiate;
	}

	public bool GetTurnDone (int i)
	{
		return activeVehicles [i].turnDone;
	}
}
