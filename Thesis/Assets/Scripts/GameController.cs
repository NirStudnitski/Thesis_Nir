using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject schoolBus;
	public GameObject van;
	public GameObject ambulance;
	public GameObject truck;
	public GameObject car1;
	public GameObject car2;
	public GameObject car3;
	public GameObject car4;
	public GameObject car5;
	public GameObject car6;
	public GameObject car7;
	public GameObject car8;
	public GameObject car9;

	// Use this for initialization
	void Start () {
	
		StartCoroutine(SpawnCars ());

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SpawnCars ()
	{
		int i = 0;
		while (true)
		{
			Vector3 spawnPosition = new Vector3 (0, 0, 0);
			Quaternion spawnRotation = Quaternion.identity* Quaternion.Euler (0f, i*90f, 0f);;
			GameObject vehicle = (GameObject) Instantiate (car4, spawnPosition, spawnRotation);
			vehicle.GetComponent<Vehicle> ().SetSpeed (5);


			yield return new WaitForSeconds (5);
			i++;
			i %= 4;
		}
	}

}
