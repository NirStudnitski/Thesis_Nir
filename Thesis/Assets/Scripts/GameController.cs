using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {


	public GameObject[] vehicles;
	float[] YCoor = { -2f, -0.2f, 0.4f, 0f,2.3f,0f,0f,0.3f,-2f,0f,0f,0f,0f };
	private float[,] laneXZ = { {  -270f, -6.5f }, { -270f, -18f },
								{  -6.5f, 270f }, { -18.3f, 270f },
								{  270f, 6.5f }, { 270f, 18.5f },
								{  6f, -270f }, { 18.3f, -270f }
	
							};

	// Use this for initialization
	void Start () {
	
		StartCoroutine(SpawnCars ());

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SpawnCars ()
	{
		
		int lane=0;
		int vehicleIndex = 0;
		while (true)
		{
			Vector3 spawnPosition = new Vector3 (laneXZ[lane,0], YCoor[vehicleIndex], laneXZ[lane,1]);
			Quaternion spawnRotation = Quaternion.identity* Quaternion.Euler (0f, (lane/2)*90f, 0f);
			GameObject vehicle = (GameObject) Instantiate (vehicles[vehicleIndex], spawnPosition, spawnRotation);

			vehicle.GetComponent<Vehicle> ().SetSpeed (40);

			int assignedTurn = 0;
			if (lane % 2 == 0)
				assignedTurn = 1;
			vehicle.GetComponent<Vehicle> ().SetTurnPlan (assignedTurn); 
			vehicle.GetComponent<Vehicle> ().SetLane (lane); 


			lane++;
			lane %= 8;
			vehicleIndex++;
			vehicleIndex %= 13;


			yield return new WaitForSeconds (1f);
		}
	}

}
