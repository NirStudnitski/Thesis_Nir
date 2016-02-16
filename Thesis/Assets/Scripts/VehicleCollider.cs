using UnityEngine;
using System.Collections;

public class VehicleCollider : MonoBehaviour {

	void OnTriggerEnter(Collider other){
				
		if (other.tag != "Lane Box") if (GameController.collisionOn) 
		{
			this.GetComponent<Vehicle> ().SetSpeed (0); 
			this.GetComponent<Vehicle> ().CollisionHappened ();
		}


	}
}
