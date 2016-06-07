using UnityEngine;
using System.Collections;

public class VehicleCollider : MonoBehaviour {

	void OnTriggerEnter(Collider other){
				
		if (other.tag != "Lane Box" && other.tag != "Exit Box") if (GameController.collisionOn) 
		{
			int myLane = this.GetComponent<Vehicle> ().GetLane ();
			int otherLane = other.GetComponent<Vehicle> ().GetLane ();
			bool next = false;
			if ((myLane % 2 == 0 && myLane == otherLane - 1) 
				|| (myLane % 2 == 1 && myLane == otherLane + 1))
				next = true;
			if (!next)
			{
				this.GetComponent<Vehicle> ().SetSpeed (0); 
				this.GetComponent<Vehicle> ().CollisionHappened ();
			}
		}


	}
}
