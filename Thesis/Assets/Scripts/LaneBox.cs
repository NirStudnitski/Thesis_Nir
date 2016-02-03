using UnityEngine;
using System.Collections;

public class LaneBox : MonoBehaviour {
	
	public int lane;

	void OnTriggerExit(Collider other){

		GameController.laneAvailable [lane] = true;
		GameController.PrintLaneAvail ();


	}
}
