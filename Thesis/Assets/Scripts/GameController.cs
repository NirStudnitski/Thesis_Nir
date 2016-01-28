using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class GameController : MonoBehaviour {


	private float waitTime;
	public static VehicleList activeVList; 
	float bla;
	private int vehicleCounter;
	public Text rateText, rateShadow;
	public Text countText, countShadow;
	public Text leftText, leftShadow;
	public Text rightText, rightShadow;
	private float leftTurningPercent, rightTurningPercent;
	public Button collisionButton;
	static public bool collisionOn;

	//-------------------- constants-----------------------------
	public const int ACTIVE_V_MAX = 200;
	private float TWO_PI = 6.28318f;
	public GameObject[] vehicles;
	float[] YCoor = { -2f, -0.2f, 0.4f, 0f,2.3f,0f,0f,0.3f,-2f,0f,0f,0f,0f };
	private float[,] laneXZ = { {  -270f, -6.5f }, { -270f, -18f },
								{  -6.5f, 270f }, { -18.3f, 270f },
								{  270f, 6.5f }, { 270f, 18.5f },
								{  6f, -270f }, { 18.3f, -270f }};

	//half each vehicle's length
	private float[] sizes = { 8f, 9f, 15f, 10f,6f,6f,6f,6f,6f,6f,6f,6f,6f };
	//half each vehicle's width
	private float[] widths = { 3.5f, 4f, 4f, 3f,2.5f,2.5f,2.5f,2.5f,2.5f,2.5f,2.5f,2.5f,2.5f };
	//half each vehicle's diagonal
	private float[] diagonals = { 8.75f, 9.85f, 15.55f, 10.45f,6.5f,6.5f,6.5f,6.5f,6.5f,6.5f,6.5f,6.5f,6.5f };
	//
	//
	//             !!!!!
	//
	//change this crap to collision detectors per lane that notify 'clear' via bool
	//this array notifies when a lane is clear to have a new vehicle enter
	private float[] clearTimes = { 0f, 0f, 0f, 0f, 0f, 0f,0f, 0f };
	//---------------------------------------------------------------


	// Use this for initialization
	void Start () {


		vehicleCounter = 0;


		collisionOn = true;
		waitTime = 4f;
		Slider (0.25f);
		LeftTurningSlider (25f);
		RightTurningSlider (25f);

		activeVList = new VehicleList();
		StartCoroutine(SpawnCars ());




	}
	
	// Update is called once per frame
	void Update () 
	{
		activeVList.UpdateList (Time.deltaTime);
		activeVList.PrintListLocations ();
			
	}

	IEnumerator SpawnCars ()
	{
		
		int lane; 
		float now, givenSpeed;
		int vehicleIndex = 0;
		bool spawn = false;
		while (true)
		{
			vehicleIndex = Random.Range (0, 13);
			lane = Random.Range(0,8);
			now = Time.time;
			givenSpeed = 40f;

			for (int i = 0; i < 8; i++) 
			{
				
				if (now < clearTimes [lane]) 
				{
					lane++;
					lane %= 8;
				}
				else 
				{
					spawn = true;
					break;
				}
			}

			if (spawn) 
			{
				Vector3 spawnPosition = new Vector3 (laneXZ [lane, 0], YCoor [vehicleIndex], laneXZ [lane, 1]);
				Quaternion spawnRotation = Quaternion.identity * Quaternion.Euler (0f, (lane / 2) * 90f, 0f);

				Vector2 directionGiven= new Vector2 (0, 1);
				switch (lane / 2) 
				{
					
					case 0:
						directionGiven = new Vector2 (1, 0);
						break;
					case 1:
						directionGiven = new Vector2 (0, -1);
						break;
					case 2:
						directionGiven = new Vector2 (-1, 0);
						break;
					case 3:
						directionGiven = new Vector2 (0, 1);
						break;
				}
				GameObject vehicle =  (GameObject) Instantiate (vehicles [vehicleIndex], spawnPosition, spawnRotation);

				vehicle.GetComponent<Vehicle>().SetSpeed (givenSpeed);
				vehicle.GetComponent<Vehicle>().SetSize (sizes [vehicleIndex]);
				vehicle.name = "" + ++vehicleCounter;



				int assignedTurn = 0;
				if (lane % 2 == 0)
					assignedTurn = (leftTurningPercent > Random.Range(0f,50f) ? 1 : 0);
				else
					assignedTurn = (rightTurningPercent > Random.Range(0f,50f) ? -1 : 0);
				
				vehicle.GetComponent<Vehicle>().SetTurnPlan (assignedTurn); 
				vehicle.GetComponent<Vehicle>().SetLane (lane); 
				clearTimes [lane] = now + (30f / givenSpeed);

				countText.text = "Vehicle Count: " + vehicleCounter;
				countShadow.text = "Vehicle Count: " + vehicleCounter;

				activeVList.Add(new Vector2 (laneXZ [lane, 0], laneXZ [lane, 1]), assignedTurn, lane, 
					vehicleCounter, givenSpeed, sizes[vehicleIndex],widths[vehicleIndex], directionGiven); 
				

			
				//activeVList.PrintListLocations ();
				//CheckFutureCollision (lane);



			}


			yield return new WaitForSeconds (Random.Range(0.90f, 1.10f)*waitTime);
		}


	}


	public void Slider(float fIn)
	{
		waitTime = 1f / fIn;
		rateText.text = "Vehicle Rate: " + fIn.ToString ("F") + " v/sec";
		rateShadow.text = "Vehicle Rate: " + fIn.ToString ("F") + " v/sec";
	}

	public void LeftTurningSlider(float fIn)
	{
		leftTurningPercent = fIn;
		leftText.text = "Left Turning: " + fIn.ToString ("F") + "%";
		leftShadow.text = "Left Turning: " + fIn.ToString ("F") + "%";
	}

	public void RightTurningSlider(float fIn)
	{
		rightTurningPercent = fIn;
		rightText.text = "Right Turning: " + fIn.ToString ("F") + "%";
		rightShadow.text = "Right Turning: " + fIn.ToString ("F") + "%";
	}

	public void ChangeColliderOn()
	{
		collisionOn = !collisionOn;
		if (collisionOn) collisionButton.GetComponentInChildren<Text>().text = "Collisions: ON";
		else collisionButton.GetComponentInChildren<Text>().text = "Collisions: OFF";
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

	private Vector2 RotateCCW (Vector2 vIn, float angle)
	{
		return new Vector2 (vIn.x * Mathf.Cos (angle) - vIn.y * Mathf.Sin (angle),
			vIn.x * Mathf.Sin (angle) + vIn.y * Mathf.Cos (angle));
	}

	private void CheckFutureCollision (int lane)
	{
		int numV = activeVList.lastIndex + 1;
		LocationDirectionDU[] futureVehicles = new LocationDirectionDU[numV];

		int numCloseV = 0, index = 0;


		for (int i = 0; i < numV; i++) 
		{
			int tempLane = activeVList.GetLane (i);
			bool closeLanes = false;
			if (lane == tempLane) closeLanes = true; 
			else if ((lane == tempLane+1 && lane%2==1) || (lane == tempLane-1 && lane%2==0)) closeLanes = true;

			if (!closeLanes)
			{
				//check candidates for collision
				Vector2 tempLoc = activeVList.GetCurrentPosition (i);
				bool probableProximity = false;
				//Debug.Log ("x = " + tempLoc.x + ", y = " + tempLoc.y);
				if (Mathf.Abs(tempLoc.x) < 240f && Mathf.Abs(tempLoc.y) < 240f)
					probableProximity = true;
				if (!probableProximity) 
				{
					futureVehicles [index++] = new LocationDirectionDU (activeVList.GetSpeed (i), activeVList.GetCurrentPosition (i), 
						activeVList.GetDirection (i), activeVList.GetTurnCounter (i), activeVList.GetLane (i), activeVList.GetTurnPlan (i),
						activeVList.GetTurnInitiate (i));
					numCloseV++;
				}
			}
		}
		//Debug.Log ("Num Close" +numCloseV);
		for (int i = 0; i < numCloseV; i++) 
		{
			for (int j = 0; j < numCloseV; j++) 
			{
				bool bla = false;
				if (i!=j) bla = FullCollisionDetection (futureVehicles[i].location, futureVehicles[j].location, 5f, 2f,
					4f, 4f, futureVehicles[i].direction, futureVehicles[j].direction);

			}
		}
	}


}
