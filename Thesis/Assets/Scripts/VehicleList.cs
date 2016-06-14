using UnityEngine;
using System.Collections;


// a data structure for the vehicle simulation
using System;


public class VehicleList : MonoBehaviour{

	public int lastIndex = -1;
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

	public void UpdateSpeeds(int lane)
	{
		for (int i = 0; i <= lastIndex; i++)
		{
			// conditionals for traffic control, if stopped, mark first five to turn left
			if (!activeVehicles [i].turnInitiate)
			{
				switch (lane)
				{
				case (1):
				case(0):
					if (activeVehicles [i].currentLocation.x > -60f)
					{
						if (GameController.changeLights)
						{
							activeVehicles [i].speed = 0;
							GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
							temp.GetComponent<Vehicle> ().SetSpeed (0);
						} else
						{
							activeVehicles [i].speed = GameController.everyonesSpeed;
							GameObject temp = GameObject.FindGameObjectWithTag ("V " + activeVehicles [i].name);
							temp.GetComponent<Vehicle> ().SetSpeed (activeVehicles [i].speed);
						}
					}
					break;

				}
			} 


		}
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
