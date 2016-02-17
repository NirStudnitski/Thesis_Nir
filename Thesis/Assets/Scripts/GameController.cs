using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;




public class GameController : MonoBehaviour {


	private float waitTime;
	public static VehicleList activeVList; 
	float suggestedSpeed;
	public int vehicleCounter;
	public static int nameBeingChecked;
	public static int numCloseV=0;
	public Text rateText, rateShadow;
	public Text countText, countShadow;
	public Text leftText, leftShadow;
	public Text rightText, rightShadow;
	private float leftTurningPercent, rightTurningPercent;
	public Button collisionButton, simulationButton;
	static public bool collisionOn, simulationOn, pause, futureCollisionDetected = false, doneWithCheck = false;

	//-------------------- constants-----------------------------
	public const int ACTIVE_V_MAX = 200;

	public const float DELTA = 0.0160000f, rR = 39f-18.3f, rL = 33f+6.5f; // for unity's delta time inaccuracies
	private static float TWO_PI = 6.283185307f;
	public GameObject[] vehicles;
	public GameObject quad;
	public static VehicleStat[] futureVehicles = new VehicleStat[ACTIVE_V_MAX];
	float[] YCoor = { -2f, -0.2f, 0.4f, 0f,2.3f,0f,0f,0.3f,-2f,0f,0f,0f,0f };
	private float[,] laneXZ = { {  -270f, -6.5f }, { -270f, -18f },
								{  -6.5f, 270f }, { -18.3f, 270f },
								{  270f, 6.5f }, { 270f, 18.5f },
								{  6f, -270f }, { 18.3f, -270f }};


	static public Vector2[] centersOfRot = 
	{
		new Vector2 (-33f, -6.5f+rL), new Vector2 (-39f, -18f-rR),
		new Vector2 (-6.5f+rL , 33f), new Vector2 (-18f-rR, 39f),
		new Vector2 (33f, 6.5f-rL),   new Vector2 (39f, 18.5f+rR),
		new Vector2 (6.5f-rL ,-33f), new Vector2 (18.3f+rR, -39f),
	};

	static public Vector3[] rotators = {
		new Vector3 (0, 0, -rL), new Vector3 (0, 0, rR),
		new Vector3 (-rL, 0, 0), new Vector3 (rR,0,0),
		new Vector3 (0, 0, rL), new Vector3 (0, 0, -rR),
		new Vector3 (rL, 0, 0), new Vector3 (-rR,0,0)
	};

	static public Vector2[] rotators2 = {
		new Vector2 (0,  -rL), new Vector2 (0,  rR),
		new Vector2 (-rL,  0), new Vector2 (rR,0),
		new Vector2 (0,  rL), new Vector2 (0,  -rR),
		new Vector2 (rL,  0), new Vector2 (-rR,0)
	};



	static public bool[] laneAvailable = { true, true, true, true, true, true, true, true };
	//half each vehicle's length
	private float[] sizes = { 8.2f, 9.2f, 15.2f, 10.2f,6.8f,6.8f,6.8f,6.8f,6.8f,6.8f,6.8f,6.8f,6.8f };
	//half each vehicle's width
	private float[] widths = { 3.6f, 4.1f, 4.1f, 3.1f,2.8f,2.8f,2.8f,2.8f,2.8f,2.8f,2.8f,2.8f,2.8f };
	//half each vehicle's diagonal
	private float[] diagonals = { 8.955f, 10.07f, 15.75f, 10.65f,7.35f,7.35f,7.35f,7.35f,7.35f,7.35f,7.35f,7.35f,7.35f };

	//---------------------------------------------------------------


	// Use this for initialization
	void Start () {

			
		vehicleCounter = 0;

		pause = false;
		collisionOn = true;
		simulationOn = true;
		waitTime = 4f;
		Slider (0.25f);
		LeftTurningSlider (25f);
		RightTurningSlider (25f);

		activeVList = new VehicleList();


		for (int i = 0; i < ACTIVE_V_MAX/10; i++) 
		{
			Vector3 spawnPosition = new Vector3 (10000, 0, 0);
			Quaternion spawnRotation = Quaternion.identity;

			GameObject quadI = (GameObject)Instantiate (quad, spawnPosition, spawnRotation);
			quadI.GetComponent<QuadUnit>().SetIndex (i);
		}
		StartCoroutine(SpawnCars ());




	}
	
	// Update is called once per frame
	void Update () 
	{

		//ShowQuadData ();

		if (!pause) activeVList.UpdateList (DELTA);
		//activeVList.PrintList();

		if (simulationOn) UpdateQuads (numCloseV, 20);
		if (simulationOn && pause && doneWithCheck) 
		{
			doneWithCheck = false;

			if (futureCollisionDetected) 
			{
				futureCollisionDetected = false;
				suggestedSpeed += 0.2f;
				//reset list
				for (int i = 0; i < numCloseV; i++) 
				{
					futureVehicles [i].currentLocation = activeVList.GetCurrentPosition (futureVehicles [i].index);
					futureVehicles [i].direction = activeVList.GetDirection (futureVehicles [i].index);
					futureVehicles [i].turnInitiate = activeVList.GetTurnInitiate (futureVehicles [i].index);
					futureVehicles [i].turnCounter = activeVList.GetTurnCounter (futureVehicles [i].index);
				}
				futureVehicles [0].speed = suggestedSpeed;

			} 
			else 
			{
				//Debug.Log ("here at completion, suggested speed= " + suggestedSpeed);
				GameObject temp = GameObject.FindGameObjectWithTag ("Vehicle Checked");
				temp.GetComponent<Vehicle> ().SetSpeed (suggestedSpeed);
				activeVList.SetSpeed (activeVList.lastIndex, suggestedSpeed);
				temp.tag = "Untagged";
				pause = false;
			}


		}

			
	}

	IEnumerator SpawnCars ()
	{
		
		int lane; 
		float givenSpeed;
		int vehicleIndex = 0;
		bool spawn = false;
		while (true)
		{
			spawn = false;
			if (!pause) 
			{
				vehicleIndex = Random.Range (0, 13);
				lane = Random.Range (0, 8);

				givenSpeed = 40f;

				for (int i = 0; i < 8; i++) {

					if (!laneAvailable[lane]) {
						lane++;
						lane %= 8;
					} else {
						spawn = true;
						break;
					}
				}

				if (spawn) {
					Vector3 spawnPosition = new Vector3 (laneXZ [lane, 0], YCoor [vehicleIndex], laneXZ [lane, 1]);
					Quaternion spawnRotation = Quaternion.identity * Quaternion.Euler (0f, (lane / 2) * 90f, 0f);

					Vector2 directionGiven = new Vector2 (0, 1);
					switch (lane / 2) {
					
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
					GameObject vehicle = (GameObject)Instantiate (vehicles [vehicleIndex], spawnPosition, spawnRotation);

					Vector3 center = new Vector3 (centersOfRot [lane].x, YCoor [vehicleIndex], centersOfRot [lane].y);
					int type = (vehicleIndex > 4 ? 4 : vehicleIndex);
					vehicle.GetComponent<Vehicle> ().SetType (type);
					vehicle.GetComponent<Vehicle> ().SetSpeed (givenSpeed);
					vehicle.GetComponent<Vehicle> ().SetDirection (directionGiven);
					vehicle.GetComponent<Vehicle> ().SetCenter (center);
					suggestedSpeed = givenSpeed;
					vehicle.GetComponent<Vehicle> ().SetSize (sizes [vehicleIndex]);
					vehicle.tag = "Vehicle Checked";
					vehicle.name = "" + ++vehicleCounter;

					nameBeingChecked = vehicleCounter;

					int assignedTurn = 0;

					if (lane % 2 == 1)
						assignedTurn = (rightTurningPercent > Random.Range (0f, 50f) ? 1 : 0);
					else
						assignedTurn = (leftTurningPercent > Random.Range (0f, 50f) ? -1 : 0);
				
					vehicle.GetComponent<Vehicle> ().SetTurnPlan (assignedTurn); 
					vehicle.GetComponent<Vehicle> ().SetLane (lane); 


					countText.text = "Vehicle Count: " + vehicleCounter;
					countShadow.text = "Vehicle Count: " + vehicleCounter;

					//Debug.Log ("before last index = " + activeVList.lastIndex);
					activeVList.Add (new Vector2 (laneXZ [lane, 0], laneXZ [lane, 1]), assignedTurn, lane, 
						vehicleCounter, givenSpeed, sizes [vehicleIndex], widths [vehicleIndex], directionGiven, 
						type, diagonals [vehicleIndex]); 
					//Debug.Log ("after last index = " + activeVList.lastIndex);
					//begin prepping your shadow runners list
					futureVehicles [0] = new VehicleStat (new Vector2 (laneXZ [lane, 0], laneXZ [lane, 1]), assignedTurn, lane, 
						vehicleCounter, givenSpeed, sizes [vehicleIndex], 
						widths [vehicleIndex], activeVList.lastIndex, directionGiven, 
						type, true, diagonals [vehicleIndex], centersOfRot[lane]);
					doneWithCheck = false;
					futureCollisionDetected = false;
					laneAvailable[lane] = false;


					CheckFutureCollision (lane);

				}

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

	public void ChangeSimulationOn()
	{
		simulationOn = !simulationOn;
		pause = false;
		if (simulationOn) simulationButton.GetComponentInChildren<Text>().text = "Alg. Simulations: ON";
		else simulationButton.GetComponentInChildren<Text>().text = "Alg. Simulations: OFF";
	}
		

	//aDiag represents half the diagonal 
	public static bool QuickCollisionDetection (Vector2 aIn, Vector2 bIn, float aDiag, float bDiag)
	{
		Debug.Log ("QUICK triggered");
		return (aIn.x -bIn.x)*(aIn.x -bIn.x) + (aIn.y -bIn.y)*(aIn.y -bIn.y) < (aDiag + bDiag)*(aDiag + bDiag);
			
	}
		
	//aWidth represents half the width of a vehicle, aLength represents half the length
	public static bool FullCollisionDetection (Vector2 aIn, Vector2 bIn, float aWidth, float bWidth,
		float aLength, float bLength, Vector2 aDir, Vector2 bDir)
	{
		//Debug.Log ("FULL triggered");
		bool runDiagnostic = false;
		string rtnS = "";
		Vector2 aDirTemp = aDir;
		if (runDiagnostic) rtnS = "Collision detection: In: a(" + aIn.x + ", " + aIn.y + "), b("
		             + bIn.x + ", " + bIn.y + "), \naDir(" + aDir.x + ", " + aDir.y + "), bDir("
		             + bDir.x + ", " + bDir.y + ")\n";
		
		bool rtn = false;
		if (Mathf.Pow (aIn.x - bIn.x, 2) + Mathf.Pow (aIn.y - bIn.y, 2)
		    < Mathf.Pow (aWidth + bWidth, 2))
		{
			rtn = true;
			if (runDiagnostic) Debug.Log ("only width detection \n" + rtnS);
		}
		else 
		{
			Vector2 aToB = new Vector2 (aIn.x - bIn.x, aIn.y - bIn.y);
			if (runDiagnostic) rtnS += "aToB(" + aToB.x + ", " + aToB.y + ")\n";
			Vector2 bTL = new Vector2 (-bWidth, bLength);
			Vector2 bBR = new Vector2 (bWidth, -bLength);

			float angle;

			if (bDir.x == 0) 
			{
				if (bDir.y > 0) angle = 0;
				else angle = TWO_PI / 2;
			} 
			else 
			{
				float alpha = Mathf.Atan (Mathf.Abs (bDir.y / bDir.x));

				if (bDir.x * bDir.y > 0) 
				{
					if (bDir.x > 0) angle = TWO_PI * 0.75f + alpha;
					else angle = TWO_PI * 0.25f + alpha;
				} 
				else if (bDir.x * bDir.y == 0) 
				{
					if (bDir.x > 0) angle = TWO_PI * 0.75f;
					else angle = TWO_PI * 0.25f;
				} 
				else 
				{
					if (bDir.x > 0) angle = TWO_PI * 0.75f - alpha;
					else angle = angle = TWO_PI * 0.25f - alpha;
				}
			}

			aToB = RotateCW (aToB, angle);
			aDir = RotateCW (aDir, angle);
			Vector2 aPerp = RotateCCW (aDir, TWO_PI / 4);
			aDir *= aLength;
			aPerp *= aWidth;

			//generate a's points in the new system
			Vector2[] aPoints = {aToB + aDir + aPerp, aToB + aDir - aPerp, 
				aToB - aDir + aPerp, aToB - aDir - aPerp
			};


			if (runDiagnostic) rtnS +="Out: \naPoints[0](" + aPoints[0].x + ", " + aPoints[0].y + ")\n" 
				+ "aPoints[1](" + aPoints[1].x + ", " + aPoints[1].y + ")\n"
				+ "aPoints[2](" + aPoints[2].x + ", " + aPoints[2].y + ")\n"
				+ "aPoints[3](" + aPoints[3].x + ", " + aPoints[3].y + ")\n"

				+ "bTL(" + bTL.x + ", " + bTL.y + ")\n"
				+ "bBR(" + bBR.x + ", " + bBR.y + ")\n"

				+"\naDir(" + aDir.x + ", " + aDir.y + "), bDir("
				+ bDir.x + ", " + bDir.y + ")\n"
				+"aToB(" + aToB.x + ", " + aToB.y + ")\n";
			
			for (int i = 0; i < 4; i++) 
			{
				if (aPoints [i].x < bBR.x && aPoints [i].x > bTL.x)
					if (aPoints [i].y > bBR.y && aPoints [i].y < bTL.y) 
					{
						rtn = true;
						i += 4;
					}
			}
		}
		if (runDiagnostic)
		{
			rtnS += "\n\n\n Collision = " + rtn;
			Debug.Log (rtnS);
		}
		if (!rtn)
			rtn = FullCollisionDetectionReverse (bIn, aIn, bWidth, aWidth,
				bLength, aLength, bDir, aDirTemp);
		return rtn;
	}

	public static bool FullCollisionDetectionReverse (Vector2 aIn, Vector2 bIn, float aWidth, float bWidth,
		float aLength, float bLength, Vector2 aDir, Vector2 bDir)
	{
		///Debug.Log ("REVERSE triggered");
		bool runDiagnostic = false;
		string rtnS = "";
		if (runDiagnostic) rtnS = "Reverse Collision detection: In: a(" + aIn.x + ", " + aIn.y + "), b("
				+ bIn.x + ", " + bIn.y + "), \naDir(" + aDir.x + ", " + aDir.y + "), bDir("
				+ bDir.x + ", " + bDir.y + ")\n";

		bool rtn = false;
		if (Mathf.Pow (aIn.x - bIn.x, 2) + Mathf.Pow (aIn.y - bIn.y, 2)
			< Mathf.Pow (aWidth + bWidth, 2))
		{
			rtn = true;
			if (runDiagnostic) Debug.Log ("only width detection \n" + rtnS);
		}
		else 
		{
			Vector2 aToB = new Vector2 (aIn.x - bIn.x, aIn.y - bIn.y);
			if (runDiagnostic) rtnS += "aToB(" + aToB.x + ", " + aToB.y + ")\n";
			Vector2 bTL = new Vector2 (-bWidth, bLength);
			Vector2 bBR = new Vector2 (bWidth, -bLength);

			float angle;

			if (bDir.x == 0) 
			{
				if (bDir.y > 0) angle = 0;
				else angle = TWO_PI / 2;
			} 
			else 
			{
				float alpha = Mathf.Atan (Mathf.Abs (bDir.y / bDir.x));

				if (bDir.x * bDir.y > 0) 
				{
					if (bDir.x > 0) angle = TWO_PI * 0.75f + alpha;
					else angle = TWO_PI * 0.25f + alpha;
				} 
				else if (bDir.x * bDir.y == 0) 
				{
					if (bDir.x > 0) angle = TWO_PI * 0.75f;
					else angle = TWO_PI * 0.25f;
				} 
				else 
				{
					if (bDir.x > 0) angle = TWO_PI * 0.75f - alpha;
					else angle = angle = TWO_PI * 0.25f - alpha;
				}
			}

			aToB = RotateCW (aToB, angle);
			aDir = RotateCW (aDir, angle);
			Vector2 aPerp = RotateCCW (aDir, TWO_PI / 4);
			aDir *= aLength;
			aPerp *= aWidth;

			//generate a's points in the new system
			Vector2[] aPoints = {aToB + aDir + aPerp, aToB + aDir - aPerp, 
				aToB - aDir + aPerp, aToB - aDir - aPerp
			};


			if (runDiagnostic) rtnS +="Out: \naPoints[0](" + aPoints[0].x + ", " + aPoints[0].y + ")\n" 
					+ "aPoints[1](" + aPoints[1].x + ", " + aPoints[1].y + ")\n"
					+ "aPoints[2](" + aPoints[2].x + ", " + aPoints[2].y + ")\n"
					+ "aPoints[3](" + aPoints[3].x + ", " + aPoints[3].y + ")\n"

					+ "bTL(" + bTL.x + ", " + bTL.y + ")\n"
					+ "bBR(" + bBR.x + ", " + bBR.y + ")\n"

					+"\naDir(" + aDir.x + ", " + aDir.y + "), bDir("
					+ bDir.x + ", " + bDir.y + ")\n"
					+"aToB(" + aToB.x + ", " + aToB.y + ")\n";

			for (int i = 0; i < 4; i++) 
			{
				if (aPoints [i].x < bBR.x && aPoints [i].x > bTL.x)
				if (aPoints [i].y > bBR.y && aPoints [i].y < bTL.y) 
				{
					rtn = true;
					i += 4;
				}
			}
		}
		if (runDiagnostic)
		{
			rtnS += "\n\n\n Collision = " + rtn;
			Debug.Log (rtnS);
		}
		return rtn;
	}

	private static Vector2 RotateCCW (Vector2 vIn, float angle)
	{
		return new Vector2 (vIn.x * Mathf.Cos (angle) - vIn.y * Mathf.Sin (angle),
			vIn.x * Mathf.Sin (angle) + vIn.y * Mathf.Cos (angle));
	}

	private static Vector2 RotateCW (Vector2 vIn, float angle)
	{
		angle = TWO_PI - angle;
		return new Vector2 (vIn.x * Mathf.Cos (angle) - vIn.y * Mathf.Sin (angle),
			vIn.x * Mathf.Sin (angle) + vIn.y * Mathf.Cos (angle));
	}

	private void CheckFutureCollision (int lane)
	{
		if (simulationOn)
		{
			pause = true;


			numCloseV = 1; 
			int index = 1;

			//generate a list of vehicles
			for (int i = 0; i < activeVList.lastIndex ; i++)
			{
				int tempLane = activeVList.GetLane (i);
				bool closeLanes = false;
				if (lane == tempLane)
					closeLanes = true;
				else if ((lane == tempLane + 1 && lane % 2 == 1) || (lane == tempLane - 1 && lane % 2 == 0))
					closeLanes = true;

				if (!closeLanes)
				{
					//check candidates for collision
					Vector2 tempLoc = activeVList.GetCurrentPosition (i);
					bool probableProximity = false;
					//Debug.Log ("x = " + tempLoc.x + ", y = " + tempLoc.y);
					if (Mathf.Abs (tempLoc.x) > 220f || Mathf.Abs (tempLoc.y) > 220f)
						probableProximity = true;
					if (probableProximity)
					{

						//Debug.Log("for future vehicle "+ (index) + " the index given is " + activeVList.GetIndex (i));
						futureVehicles [index++] = new VehicleStat (
							activeVList.GetCurrentPosition (i), activeVList.GetTurnPlan (i), 
							activeVList.GetLane (i), activeVList.GetName (i), activeVList.GetSpeed (i), sizes [activeVList.GetType (i)], 
							widths [activeVList.GetType (i)], activeVList.GetIndex (i),
							activeVList.GetDirection (i), activeVList.GetType (i), true, 
							diagonals [activeVList.GetType (i)], centersOfRot[activeVList.GetLane (i)]);
						numCloseV++;
					}
				}
			}
		} 
		else pause = false;
		/*
		if (!simulationOn) 
		{
			

			UpdateQuads (numCloseV, 200);

			while (futureCollisionDetected) 
			{
				
				futureCollisionDetected = false;
				suggestedSpeed += 0.1f;
				//reset list
				for (int i = 0;i<numCloseV;i++)
				{
					futureVehicles[i].currentLocation = activeVList.GetCurrentPosition (i);
					futureVehicles[i].direction = activeVList.GetDirection (i);
					futureVehicles [i].turnInitiate = false;
					futureVehicles [i].turnCounter = 0;
				}
				futureVehicles[0].speed = suggestedSpeed;

				UpdateQuads (numCloseV, 200);

			} 

			GameObject temp = GameObject.FindGameObjectWithTag ("Vehicle Checked");
			temp.GetComponent<Vehicle> ().SetSpeed (suggestedSpeed);
			temp.tag = "Untagged";
			futureCollisionDetected = false; 
			pause = false;

		}*/


	}


	private void UpdateQuads(int numVehiclesClose, int multiplier)
	{
		
		for (int i=0;i<numVehiclesClose;i++)
		{
			futureVehicles[i].UpdateAndAdvance(DELTA, multiplier);
		}
	}

	public static void PrintLaneAvail()
	{
		string rtn = "Lanes available: ";
		for (int i = 0; i < 8; i++)
			rtn += i + ": " + laneAvailable [i] + ", ";
		Debug.Log (rtn);
	}


	private void ShowQuadData()
	{
		string rtn = "";
		for (int i = 0;i<numCloseV;i++)
		{
			rtn += "futureVehicle[" + i + "].location = (" + futureVehicles [i].currentLocation.x + ", " + futureVehicles [i].currentLocation.y + ")\n";
			rtn += "futureVehicle[" + i + "].direction = (" + futureVehicles [i].direction.x + ", " + futureVehicles [i].direction.y + ")\n";
			rtn += "futureVehicle[" + i + "].halfLength = " + futureVehicles [i].halfLength + "\n";
			rtn += "futureVehicle[" + i + "].halfWidth = " + futureVehicles [i].halfWidth + "\n";
			rtn += "futureVehicle[" + i + "].index = " + futureVehicles [i].index + "\n\n\n";


		}
		Debug.Log (rtn);
	}


}
