using UnityEngine;
using System.Collections;


// a data structure for the vehicle simulation
using System;


public class VehicleList : MonoBehaviour{

	public int lastIndex;
	public VehicleStat[] activeVehicles = new VehicleStat[GameController.ACTIVE_V_MAX];

	public VehicleList()
	{
		for (int i = 0; i < GameController.ACTIVE_V_MAX; i++)
			activeVehicles [i] = null;
	}

	void Start () 
	{
		lastIndex = -1;
	}

	public void Add(Vector2 spawnIn, int turnPlanIn, int laneIn, 
					int nameIn, float speedIn, float timeIn, 
					float halfLengthIn, float halfWidthIn)
	{
		activeVehicles [lastIndex++] = new VehicleStat (spawnIn, turnPlanIn, laneIn, 
								nameIn, speedIn, timeIn, halfLengthIn, halfWidthIn);
	}	

	public void Remove(int nameIn)
	{
		int index = -1;
		while (index++<=lastIndex)
			if (Int32.Parse(activeVehicles[index].name) == nameIn) break;
		if (index == lastIndex)
			activeVehicles [index] = null;
		while (index++ <= lastIndex)
			activeVehicles [index-1] = activeVehicles [index];
		lastIndex--;
	}
}
