using UnityEngine;
using System.Collections;

public class VehicleCollider : MonoBehaviour {

	void OnTriggerEnter(Collider other){

		this.GetComponent<Vehicle> ().SetSpeed (0); 
		this.GetComponent<Vehicle> ().CollisionHappened ();
		Debug.Log ("collision happened" + other);

	}
}
