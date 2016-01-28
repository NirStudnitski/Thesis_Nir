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
		float halfLengthIn, float halfWidthIn, Vector2 dirIn)
	{
		activeVehicles [++lastIndex] = new VehicleStat (spawnIn, turnPlanIn, laneIn, 
								nameIn, speedIn,  halfLengthIn, halfWidthIn, lastIndex, dirIn);
		
		
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

		while (++index <= lastIndex) listS +=activeVehicles [index].name + ", ";

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

	public int GetIndex(int nameIn)
	{
		int index = -1;
		while (++index <= lastIndex) if (activeVehicles [index].name == nameIn) break;
		return index;
	}
		
	public void UpdateList(float deltaTime)
	{
		for (int i=0;i <= lastIndex; i++) activeVehicles [i].UpdateAndAdvance (deltaTime);
	}

	public float GetSpeed (int i)
	{
		return activeVehicles[i].speed;
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

	public int GetTurnPlan (int i)
	{
		return activeVehicles [i].turnPlan;
	}

	public bool GetTurnInitiate (int i)
	{
		return activeVehicles [i].turnInitiate;
	}
}
