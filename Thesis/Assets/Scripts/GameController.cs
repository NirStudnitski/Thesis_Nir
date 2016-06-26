using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;





public class GameController : MonoBehaviour {

	public static List<GameObject> cars;
	public static int currentMethod = 1;
	public enum methods { v1, v2, TL};
	private float waitTime;
	public static VehicleList activeVList;
	public static VehicleList[] activeV; //per lane
	float suggestedSpeed, elapsedTime = 0, deltaSpeed=0;
	public static float everyonesSpeed = 100;
	public int vehicleCounter;
	public static int nameBeingChecked;
	public static string timeText;
	public static int numCloseV=0, simulationSpeed = 5, frameCounter=0;
	public Text rateText, rateShadow;
	public Text countText, countShadow;
	public Text noSolText, noSolShadow;
	public Text leftText, leftShadow;
	public Text rightText, rightShadow;
	public Text time, timeShadow;
	private float leftTurningPercent, rightTurningPercent;
	public Button collisionButton, simulationButton;
	static public bool collisionOn, simulationOn, pause, futureCollisionDetected = false, doneWithCheck = false;

	//for V2
	public static float[,] dataCenter, dataCenter2; //0= loc.x, 1= loc.y, 2= dir.x, 3= dir.y, 4= halflength, 5= halfwidth, 6= halfdiagonal
	public static int nowFrame = 0; // the index of the current time frame
	public static int[] lastIndexes, lastIndexes2;
	public static int rowsInDataCenter = 1500;
	public static bool inStepSim = true; //to toggle is simulation is of future or now
	public static bool cameraAbove = false;

	// for STOP LIGHTS
	public static int TLSeconds = 20;
	public static bool changeLights = false;

	public static bool[] doneTurning;

	//-------------------- constants-----------------------------
	public const int ACTIVE_V_MAX = 400;

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

	// bunch of data for max speed per lane
	static public int[] frameAtInstantiation = { 0, 0, 0, 0, 0, 0, 0,0 };
	static public float[] halfLengthAtInst = { 0, 0, 0, 0, 0, 0, 0, 0 };
	static public float[] lastVehicleSpeed = { 0, 0, 0, 0, 0, 0, 0, 0 };
	static float currentMaxSpeed = 100f;
	static bool notExceedMax = true;

	//half each vehicle's length
	private float[] sizes = { 8.6f, 10.5f, 15.95f, 10.7f,7.15f,7.15f,7.15f,8f,7.15f,7.15f,7.15f,7.15f,7.15f };
	//half each vehicle's width
	private float[] widths = { 3.8f, 4.5f, 4.3f, 3.25f,2.95f,2.95f,2.95f,3.3f,2.95f,2.95f,2.95f,2.95f,2.95f };
	//half each vehicle's diagonal
	private float[] diagonals = { 9.4f, 11.5f, 16.5f, 11.2f,7.7f,7.7f,7.7f,8.7f,7.7f,7.7f,7.7f,7.7f,7.7f };

	//---------------------------------------------------------------


	// Use this for initialization
	void Start () {

		cars = new List<GameObject>();
		vehicleCounter = 0;

		pause = false;
		collisionOn = true;
		simulationOn = true;
		waitTime = 4f;
		Slider (0.25f);
		LeftTurningSlider (25f);
		RightTurningSlider (25f);

		if (currentMethod == (int) methods.v1) activeVList = new VehicleList();
		if (currentMethod == (int)methods.TL)
		{
			activeV = new VehicleList[8];
			for (int i=0;i<8;i++) activeV[i] = new VehicleList ();


			doneTurning = new bool[8];
			for (int i = 0; i < 8; i++)
				doneTurning [i] = false;
		}

		if (currentMethod == (int)methods.v2)
		{
			dataCenter = new float[rowsInDataCenter, ACTIVE_V_MAX];
			lastIndexes = new int[rowsInDataCenter];


		}



		for (int i = 0; i < ACTIVE_V_MAX/10; i++) 
		{
			Vector3 spawnPosition = new Vector3 (10000, 0, 0);
			Quaternion spawnRotation = Quaternion.identity;

			GameObject quadI = (GameObject)Instantiate (quad, spawnPosition, spawnRotation);
			quadI.GetComponent<QuadUnit>().SetIndex (i);
		}
		StartCoroutine(SpawnCars ());




	}

	void FixedUpdate () 
	{
		elapsedTime += DELTA;
		frameCounter++;
		if (frameCounter % 10 == 0)
		{
			timeText = "Elapsed Time: " + elapsedTime.ToString ("F2") + " s";
			time.text = timeText;
			timeShadow.text = timeText;
		}

		if (currentMethod == (int)methods.TL)
		{
			if (!pause)
			{
				if (frameCounter % ((125 * TLSeconds) / 2) == 0)
				{
					changeLights = !changeLights;

					if (!changeLights)
						for (int i = 0; i < 8; i += 4)
						{
							activeV [i].OrderQueue (i);
							activeV [i + 1].OrderQueue (i + 1);
						}
					else
						for (int i = 2; i < 8; i += 4)
						{
							activeV [i].OrderQueue (i);
							activeV [i + 1].OrderQueue (i + 1);
						}
					


					if (changeLights)
					{
						GameObject temp = GameObject.FindGameObjectWithTag ("Green Light");
						temp.transform.position = new Vector3 (-12f, 0.2f, 23f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 0f);

						temp = GameObject.FindGameObjectWithTag ("Red Light");
						temp.transform.position = new Vector3 (-23.5f, 0.2f, -12f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 90f);

						temp = GameObject.FindGameObjectWithTag ("Green Light 2");
						temp.transform.position = new Vector3 (12f, 0.2f, -23f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 0f);

						temp = GameObject.FindGameObjectWithTag ("Red Light 2");
						temp.transform.position = new Vector3 (23.5f, 0.2f, 12f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 90f);

					} else
					{
						GameObject temp = GameObject.FindGameObjectWithTag ("Green Light");
						temp.transform.position = new Vector3 (-23.5f, 0.2f, -12f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 90f);

						temp = GameObject.FindGameObjectWithTag ("Red Light");
						temp.transform.position = new Vector3 (-12f, 0.2f, 23f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 0f);

						temp = GameObject.FindGameObjectWithTag ("Green Light 2");
						temp.transform.position = new Vector3 (23.5f, 0.2f, 12f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 90f);

						temp = GameObject.FindGameObjectWithTag ("Red Light 2");
						temp.transform.position = new Vector3 (12f, 0.2f, -23f);
						temp.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 90f, 0f);

					}
				}

			}
		} else
		{
			GameObject temp = GameObject.FindGameObjectWithTag ("Green Light");
			Destroy (temp);

			temp = GameObject.FindGameObjectWithTag ("Red Light");
			Destroy (temp);

			temp = GameObject.FindGameObjectWithTag ("Green Light 2");
			Destroy (temp);

			temp = GameObject.FindGameObjectWithTag ("Red Light 2");
			Destroy (temp);

		}

		if (currentMethod == (int)methods.v2)
		{
			if (!pause)
			{
				lastIndexes [nowFrame] = 0;
				nowFrame++;
				nowFrame %= rowsInDataCenter;

			}
		}

		if (currentMethod == (int) methods.v1)
		{
			//ShowQuadData ();

			if (!pause)
			{
				activeVList.UpdateList (DELTA);

			}


			if (simulationOn)
			{	
				for (int i = 0; i < simulationSpeed && pause &&
				!doneWithCheck && !futureCollisionDetected; i++)
					UpdateQuads (numCloseV, 1);
				if (pause && doneWithCheck)
				{
					doneWithCheck = false;

					if (futureCollisionDetected)
					{
						futureCollisionDetected = false;

						// the attempted speed is slower and faster alternatingly, 
						//but no more than currentMaxSpeed to avoid hitting the car in front
						if (notExceedMax)
						if (suggestedSpeed > currentMaxSpeed)
							notExceedMax = false;
							

						
						if (notExceedMax)
						{
							if (deltaSpeed >= 0)
								deltaSpeed += 1f;
							suggestedSpeed = everyonesSpeed + deltaSpeed;
							deltaSpeed *= -1;
						} else
						{
							suggestedSpeed = everyonesSpeed + deltaSpeed;
							deltaSpeed -= 1f;
						}

						//reset list
						for (int i = 0; i < numCloseV; i++)
						{
							futureVehicles [i].currentLocation = activeVList.GetCurrentPosition (futureVehicles [i].index);
							futureVehicles [i].direction = activeVList.GetDirection (futureVehicles [i].index);
							futureVehicles [i].turnInitiate = activeVList.GetTurnInitiate (futureVehicles [i].index);
							futureVehicles [i].turnCounter = activeVList.GetTurnCounter (futureVehicles [i].index);
							futureVehicles [i].turnDone = activeVList.GetTurnDone (futureVehicles [i].index);
						}
						futureVehicles [0].speed = suggestedSpeed;

					} else
					{
						//Debug.Log ("here at completion, suggested speed= " + suggestedSpeed);
						GameObject temp = GameObject.FindGameObjectWithTag ("Vehicle Checked");
						temp.GetComponent<Vehicle> ().SetSpeed (suggestedSpeed);
						int laneNow = temp.GetComponent<Vehicle> ().GetLane ();
						activeVList.SetSpeed (activeVList.lastIndex, suggestedSpeed);
						temp.tag = "Untagged";

						frameAtInstantiation [laneNow] = frameCounter;
						halfLengthAtInst [laneNow] = temp.GetComponent<Vehicle> ().GetSize ();
						lastVehicleSpeed [laneNow] = suggestedSpeed;
						notExceedMax = true;
						deltaSpeed = 0;



						pause = false;

					}


				}
			}
		}

		if (currentMethod == (int)methods.TL)
		{
			//ShowQuadData ();

			if (!pause)
			{
				for (int i = 0; i < 8; i++)
				{
					activeV [i].UpdateList (DELTA);
					activeV [i].UpdateSpeeds (i, frameCounter);
				}


			}
		}

			
	}

	IEnumerator algorithmV1()
	{
		while (Mathf.Abs(suggestedSpeed) < 1.75*everyonesSpeed)
		{
			// there is a bug here, that if this does not find a solution, it never unpauses. maybe a good thing. 

			futureCollisionDetected = false;
			doneWithCheck = false;
			while(!futureCollisionDetected && !doneWithCheck) UpdateQuads (numCloseV, 1);

			if (futureCollisionDetected)
			{
				futureCollisionDetected = false;
				doneWithCheck = false;

				// the attempted speed is slower and faster alternatingly, 
				if (deltaSpeed>=0) deltaSpeed+=1f;
				suggestedSpeed += deltaSpeed;
				deltaSpeed *= -1;
				//reset list
				for (int i = 0; i < numCloseV; i++)
				{
					futureVehicles [i].currentLocation = activeVList.GetCurrentPosition (futureVehicles [i].index);
					futureVehicles [i].direction = activeVList.GetDirection (futureVehicles [i].index);
					futureVehicles [i].turnInitiate = activeVList.GetTurnInitiate (futureVehicles [i].index);
					futureVehicles [i].turnCounter = activeVList.GetTurnCounter (futureVehicles [i].index);
					futureVehicles [i].turnDone = activeVList.GetTurnDone (futureVehicles [i].index);
				}
				futureVehicles [0].speed = suggestedSpeed;

			}
			else
			{
				GameObject temp = GameObject.FindGameObjectWithTag ("Vehicle Checked");
				temp.GetComponent<Vehicle> ().SetSpeed (suggestedSpeed);
				activeVList.SetSpeed (activeVList.lastIndex, suggestedSpeed);
				temp.tag = "Untagged";
				pause = false;
				deltaSpeed = 0;
				break;
			}

		}
		if (Mathf.Abs (suggestedSpeed) >= 1.75*everyonesSpeed)
			Debug.Log ("No solution found.");
		yield return new WaitForSeconds (0);
	}

	IEnumerator SpawnCars ()
	{
		
		int lane; 

		int vehicleIndex = 0;
		bool spawn = false;
		while (true)
		{
			spawn = false;
			if (!pause) 
			{
				vehicleIndex = Random.Range (0, 13);
				lane = Random.Range (0, 8);



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
					cars.Add (vehicle);
					Vector3 center = new Vector3 (centersOfRot [lane].x, YCoor [vehicleIndex], centersOfRot [lane].y);
					int type = (vehicleIndex > 4 ? 4 : vehicleIndex);
					vehicle.GetComponent<Vehicle> ().SetType (type);

					vehicle.GetComponent<Vehicle> ().SetDirection (directionGiven);
					vehicle.GetComponent<Vehicle> ().SetCenter (center);

					vehicle.GetComponent<Vehicle> ().SetSize (sizes [vehicleIndex]);
					vehicle.tag = "Vehicle Checked";
					vehicle.name = "" + ++vehicleCounter;
					vehicle.GetComponent<Vehicle> ().SetName(vehicleCounter);
					nameBeingChecked = vehicleCounter;

					int assignedTurn = 0;

					if (lane % 2 == 1)
						assignedTurn = (rightTurningPercent > Random.Range (0f, 50f) ? 1 : 0);
					else
					{
						if (currentMethod == (int)methods.TL)
							assignedTurn = 0;
						else assignedTurn = (leftTurningPercent > Random.Range (0f, 50f) ? -1 : 0);
					}
					
					vehicle.GetComponent<Vehicle> ().SetTurnPlan (assignedTurn); 
					vehicle.GetComponent<Vehicle> ().SetLane (lane); 


					countText.text = "Vehicle Count: " + vehicleCounter;
					countShadow.text = "Vehicle Count: " + vehicleCounter;

					doneWithCheck = false;
					futureCollisionDetected = false;
					laneAvailable [lane] = false;

					currentMaxSpeed = CalcMaxSpeed (lane, sizes [vehicleIndex]);

					suggestedSpeed = System.Math.Min(everyonesSpeed, currentMaxSpeed);
					//Debug.Log ("for car number " + vehicleCounter +" max speed = " + currentMaxSpeed + ", suggested speed = " + suggestedSpeed);
					vehicle.GetComponent<Vehicle> ().SetSpeed (suggestedSpeed);


					if (currentMethod == (int) methods.v1 )
					{
					//Debug.Log ("before last index = " + activeVList.lastIndex);
						activeVList.Add (new Vector2 (laneXZ [lane, 0], laneXZ [lane, 1]), assignedTurn, lane, 
							vehicleCounter, everyonesSpeed, sizes [vehicleIndex], widths [vehicleIndex], directionGiven, 
							type, diagonals [vehicleIndex]); 


							//begin prepping your shadow runners list
							futureVehicles [0] = new VehicleStat (new Vector2 (laneXZ [lane, 0], laneXZ [lane, 1]), assignedTurn, lane, 
								vehicleCounter, everyonesSpeed, sizes [vehicleIndex], 
								widths [vehicleIndex], activeVList.lastIndex, directionGiven, 
								type, true, diagonals [vehicleIndex], centersOfRot [lane]);
						
							CheckFutureCollisionV1 (lane);

					}

					if (currentMethod == (int) methods.TL)
					{
						activeV[lane].Add (new Vector2 (laneXZ [lane, 0], laneXZ [lane, 1]), assignedTurn, lane, 
							vehicleCounter, everyonesSpeed, sizes [vehicleIndex], widths [vehicleIndex], directionGiven, 
							type, diagonals [vehicleIndex]); 
						

					}

					if (currentMethod == (int)methods.v2)
					{
						pause = true;
						StartCoroutine(CheckFutureCollisionV2 (new Vector2 (laneXZ [lane, 0], laneXZ [lane, 1]), 
							assignedTurn, lane, sizes [vehicleIndex], widths [vehicleIndex], directionGiven, 
							diagonals [vehicleIndex]));
					}

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

	public void QuitGame()
	{
		Application.Quit();
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
		//Debug.Log ("QUICK triggered");
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
		if (runDiagnostic) rtnS = "Collision detection: \nIn: a(" + aIn.x + ", " + aIn.y + "), b("
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

	private void CheckFutureCollisionV1 (int lane)
	{

		pause = true;

		numCloseV = 1; 
		int index = 1;

		//generate a list of vehicles
		for (int i = 0; i < activeVList.lastIndex ; i++)
		{
			int tempLane = activeVList.GetLane (i);
			bool closeLanes = false;

			if ((lane == tempLane + 1 && lane % 2 == 1) || (lane == tempLane - 1 && lane % 2 == 0))
				closeLanes = true;

			if (!closeLanes)
			{
				//check candidates for collision
				Vector2 tempLoc = activeVList.GetCurrentPosition (i);
				bool probableProximity = false;
				//Debug.Log ("x = " + tempLoc.x + ", y = " + tempLoc.y);
				if (Mathf.Abs (tempLoc.x) > 200f || Mathf.Abs (tempLoc.y) > 200f)
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
		if (numCloseV == 1)
		{
			futureCollisionDetected = false;
			doneWithCheck = true;
			pause = false;
			GameObject temp = GameObject.FindGameObjectWithTag ("Vehicle Checked");
			temp.tag = "Untagged";
		}
		else if (!simulationOn) StartCoroutine(algorithmV1());



	}

	IEnumerator CheckFutureCollisionV2 (Vector2 locIn, int turnPlanIn, int laneIn,   
												float halfLengthIn, float halfWidthIn, 
												Vector2 dirIn, float diagIn)
	{
		bool notExceedMax = true;
		int deltaFrame, startFrame;
		Vector2 newLoc;
		float beginningSpeed = suggestedSpeed-1;
		float increment = everyonesSpeed / 40f;
		while (suggestedSpeed < 1.75*everyonesSpeed && suggestedSpeed > 0.25*everyonesSpeed)
		{
			
			futureCollisionDetected = false;

			if (notExceedMax)
			if (suggestedSpeed > currentMaxSpeed)
			{
				if (deltaSpeed > 0)
					deltaSpeed *= -1;
				notExceedMax = false;
			}

			if (notExceedMax)
			{
				if (deltaSpeed >= 0)
					deltaSpeed += increment;
				suggestedSpeed = beginningSpeed + deltaSpeed;
				deltaSpeed *= -1;
			} else
			{
				suggestedSpeed = beginningSpeed + deltaSpeed;
				deltaSpeed -= increment;
			}


			deltaFrame = (int) Mathf.Floor(231f / (suggestedSpeed*DELTA));
			//string blal = "speed = " + suggestedSpeed + ", now frame = " + nowFrame + ", delta Frame = " + deltaFrame;
			startFrame = nowFrame + deltaFrame;
			startFrame %= rowsInDataCenter;
			//blal += "\n startFrame = " + startFrame;


			newLoc = locIn + dirIn * suggestedSpeed * deltaFrame*DELTA;
			//blal += "\n new location = (" + newLoc.x + ", "  + newLoc.y+ ")";
			//Debug.Log (blal + "\n before addition: \n");
			//PrintDataCenterRange (startFrame, startFrame+140);
			futureCollisionDetected = AdvanceAndCheck (suggestedSpeed, startFrame, newLoc,laneIn, turnPlanIn, dirIn, halfLengthIn, halfWidthIn, diagIn);

			if (!futureCollisionDetected)
			{
				AdvanceAndUpdate (suggestedSpeed, startFrame, newLoc,laneIn, turnPlanIn, 
									dirIn, halfLengthIn, halfWidthIn, diagIn);

				GameObject temp = GameObject.FindGameObjectWithTag ("Vehicle Checked");
				temp.GetComponent<Vehicle> ().SetSpeed (suggestedSpeed);
				int laneNow = temp.GetComponent<Vehicle> ().GetLane ();
				temp.tag = "Untagged";
			//	Debug.Log (blal + "\n after addition: \n");
				//PrintDataCenterRange (startFrame, startFrame+140);
				frameAtInstantiation [laneNow] = frameCounter;
				halfLengthAtInst [laneNow] = temp.GetComponent<Vehicle> ().GetSize ();
				lastVehicleSpeed [laneNow] = suggestedSpeed;
				//Debug.Log ("car number " + vehicleCounter + "final speed is " + suggestedSpeed);
				break;

			}
		}
		if (suggestedSpeed >= 1.75* everyonesSpeed  || suggestedSpeed <= 0.25* everyonesSpeed)
		{
			noSolText.text = "No solution found.";
			noSolShadow.text = "No solution found.";
		}
		
		notExceedMax = true;
		deltaSpeed = 0;

		//PrintDataCenter ();
		yield return new WaitForSeconds (0);
		pause = false;

	}


	private void UpdateQuads(int numVehiclesClose, int multiplier)
	{
		
		for (int i=0;i<numVehiclesClose;i++)
		{
			futureVehicles[i].UpdateAndAdvance(DELTA, multiplier);
		}
	}

	public void ToggleCam()
	{

		cameraAbove = !cameraAbove;
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

	public float CalcMaxSpeed(int lane, float myHalfLength)
	{
		float rtn = 0;
		if (lastVehicleSpeed [lane] == 0)
			rtn = 1000f;
		else
		{
			int deltaFrame = frameCounter - frameAtInstantiation [lane];
			float distanceRemaining = 231f - ((float)deltaFrame) * lastVehicleSpeed [lane]*DELTA
				+ halfLengthAtInst [lane];

			if (distanceRemaining < 0)
				rtn = 1000f;
			else
			{
				float timeRemaining = distanceRemaining / lastVehicleSpeed [lane];
				//Debug.Log ("time remainin = " + timeRemaining);
				rtn = (231f - myHalfLength) / timeRemaining;
			}
		}
		return rtn;
	}

	private bool AdvanceAndCheck(float speed, int startFrame, Vector2 newLoc,int laneIn, int turnPlan, Vector2 dirIn, 
									float halfLengthIn, float halfWidthIn, float diagIn)
	{
		Vector2 currentLocation = newLoc, direction = dirIn, center = centersOfRot[laneIn];
		bool turnInitiate = false, turnDone = false, futureCollisionDetected = false;
		float extra=0, theta = 0f, HALF_PI = 1.5707963f;
		int turnCounter = 1;

		for (int i=0;i<140 && !futureCollisionDetected ;i++)
		{
			startFrame++;
			startFrame %= rowsInDataCenter;
			// check collision every other frame
			for (int j = 0; j < lastIndexes[startFrame] && !futureCollisionDetected; j++)
			{

				if (QuickCollisionDetection (currentLocation, new Vector2(dataCenter[startFrame,7*j],dataCenter[startFrame,7*j+1]),
					diagIn, dataCenter[startFrame,7*j+6]))
				{
					//Debug.Log ("Quick Returned TRUE on frame " + (startFrame));	
					if (FullCollisionDetection (currentLocation, new Vector2(dataCenter[startFrame,7*j],dataCenter[startFrame,7*j+1]), 
						halfWidthIn, dataCenter[startFrame,7*j+5],
						halfLengthIn, dataCenter[startFrame,7*j+4],
						direction, new Vector2(dataCenter[startFrame,7*j+2],
							dataCenter[startFrame,7*j+3])))
					{
						Debug.Log ("Collision");
						futureCollisionDetected = true;
					}


				}
			} 

			//advance
			if (turnPlan ==0) currentLocation += direction * speed * DELTA;
			else if (turnPlan == 1) // turn right
			{
				switch (laneIn)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}
			} 
			else if (turnPlan == -1) 
			{
				switch (laneIn)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}

			}


		}

		return futureCollisionDetected;

	}

	private void AdvanceAndUpdate(float speed, int startFrame, Vector2 newLoc,int laneIn, int turnPlan, Vector2 dirIn, 
		float halfLengthIn, float halfWidthIn, float diagIn)
	{
		
		Vector2 currentLocation = newLoc, direction = dirIn, center = centersOfRot[laneIn];
		bool turnInitiate = false, turnDone = false, futureCollisionDetected = false;
		float extra=0, theta = 0f, HALF_PI = 1.5707963f;
		int turnCounter = 1;

		for (int i=0;i<140 ;i++)
		{

			int j = startFrame+i;
			j %= rowsInDataCenter;
			int ind = lastIndexes[j]*7;
			lastIndexes [j]++;
			dataCenter[j,ind] = currentLocation.x;
			dataCenter[j,ind+1] = currentLocation.y;
			dataCenter[j,ind+2] = direction.x;
			dataCenter[j,ind+3] = direction.y;
			dataCenter[j,ind+4] = halfLengthIn;
			dataCenter[j,ind+5] = halfWidthIn;
			dataCenter[j,ind+6] = diagIn;

			if (turnPlan ==0) currentLocation += direction * speed * DELTA;
			else if (turnPlan == 1) // turn right
			{
				switch (laneIn)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rR;
						direction = RotateCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rR;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCW (rotators2 [laneIn], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}
			} 
			else if (turnPlan == -1) 
			{
				switch (laneIn)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (0, 1);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (0, -1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (1, 0);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (-1, 0), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (0, -1);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
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
						currentLocation += direction * speed * DELTA;
					} 
					else
					{

						float arcLength = extra + speed * DELTA * turnCounter;
						theta = arcLength / rL;
						direction = RotateCCW (new Vector2 (0, 1), theta);

						if (theta < HALF_PI)
						{
							currentLocation = center + RotateCCW (rotators2 [laneIn], theta);	
							turnCounter++;
						} 
						else
						{
							float extraAngle = theta - HALF_PI;
							float extraDist = extraAngle*rL;
							direction = new Vector2 (-1, 0);
							currentLocation = center + RotateCCW (rotators2 [laneIn], HALF_PI)
								+ extraDist * direction;
							turnDone = true;
						}
					}
					break;
				}

			}




		}



	}

	public void PrintDataCenter ()
	{
		string s = "";

		for (int i = 0; i < rowsInDataCenter; i++)
		{
			s += "Index = " + i + " ";
			for (int j = 0; j < lastIndexes [i]; j++)
				for (int k = 0; k < 7; k++)
					s += dataCenter [i, 7 * j + k] + ", ";
			
			s += " last index: " + lastIndexes [i] + "\n";
		}
		Debug.Log (s);
	}

	public void PrintDataCenterRange (int a, int b)
	{
		string s = "";

		for (int i = a; i < b; i++)
		{
			int iM = i % rowsInDataCenter;
			s += "Index = " + iM + " ";
			for (int j = 0; j < lastIndexes [iM]; j++)
				for (int k = 0; k < 7; k++)
					s += dataCenter [iM, 7 * j + k] + ", ";

			s += " last index: " + lastIndexes [iM] + "\n";
		}
		Debug.Log (s);
	}




}
