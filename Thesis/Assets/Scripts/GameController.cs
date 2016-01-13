using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {


	public GameObject[] vehicles;
	float[] YCoor = { -1.9f, -1.9f, 2.1f, -2f,1f,1,1,1,1,1,1,1,1 };
	private float[,] laneXZ = { {  -270f, -6.5f }, { -270f, -18f },
								{  6f, 270f }, { 18.3f, 270f },
								{  270f, 6.5f }, { 270f, 18.5f },
								{  -6f, -270f }, { -18.3f, -270f }
	
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
		int direction = 0;
		int lane=6;
		while (true)
		{
			Vector3 spawnPosition = new Vector3 (laneXZ[lane,0], YCoor[0], laneXZ[lane,1]);
			Quaternion spawnRotation = Quaternion.identity* Quaternion.Euler (0f, (lane/2)*90f, 0f);
			GameObject vehicle = (GameObject) Instantiate (vehicles[7], spawnPosition, spawnRotation);
			vehicle.GetComponent<Vehicle> ().SetSpeed (0);


			yield return new WaitForSeconds (20);

			direction %= 4;
		}
	}

}
