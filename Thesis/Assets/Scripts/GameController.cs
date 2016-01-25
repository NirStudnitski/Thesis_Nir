using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {


	private float waitTime;
	float bla;
	private int vehicleCounter;
	public Text rateText, rateShadow;
	public Text countText, countShadow;
	public Text leftText, leftShadow;
	public Text rightText, rightShadow;
	private float leftTurningPercent, rightTurningPercent;
	public Button collisionButton;
	static public bool collisionOn;
	float lastUpdate, thisUpdate;
	private int lastVehicleCount;

	public GameObject[] vehicles;
	float[] YCoor = { -2f, -0.2f, 0.4f, 0f,2.3f,0f,0f,0.3f,-2f,0f,0f,0f,0f };
	private float[,] laneXZ = { {  -270f, -6.5f }, { -270f, -18f },
								{  -6.5f, 270f }, { -18.3f, 270f },
								{  270f, 6.5f }, { 270f, 18.5f },
								{  6f, -270f }, { 18.3f, -270f }
							};

	private float[] sizes = { 16f, 18f, 30f, 20f,12f,12f,12f,12f,12f,12f,12f,12f,12f };

	//this array notifies when a lane is clear to have a new vehicle enter
	private float[] clearTimes = { 0f, 0f, 0f, 0f, 0f, 0f,0f, 0f };

	// Use this for initialization
	void Start () {

		vehicleCounter = 0;


		collisionOn = true;
		waitTime = 4f;
		Slider (0.25f);
		LeftTurningSlider (25f);
		RightTurningSlider (25f);
		StartCoroutine(SpawnCars ());




	}
	
	// Update is called once per frame
	void Update () {
	
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
				GameObject vehicle = (GameObject)Instantiate (vehicles [vehicleIndex], spawnPosition, spawnRotation);

				vehicle.GetComponent<Vehicle> ().SetSpeed (givenSpeed);
				vehicle.GetComponent<Vehicle> ().SetSize (sizes [vehicleIndex]);


				int assignedTurn = 0;
				if (lane % 2 == 0)
					assignedTurn = (leftTurningPercent > Random.Range(0f,50f) ? 1 : 0);
				else
					assignedTurn = (rightTurningPercent > Random.Range(0f,50f) ? -1 : 0);
				vehicle.GetComponent<Vehicle> ().SetTurnPlan (assignedTurn); 
				vehicle.GetComponent<Vehicle> ().SetLane (lane); 
				clearTimes [lane] = now + (30f / givenSpeed);
				vehicleCounter++;
				countText.text = "Vehicle Count: " + vehicleCounter;
				countShadow.text = "Vehicle Count: " + vehicleCounter;


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
}
