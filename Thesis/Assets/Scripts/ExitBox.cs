using UnityEngine;
using System.Collections;
using System;

public class ExitBox : MonoBehaviour {

	void OnTriggerEnter(Collider other){

		if (GameController.currentMethod == (int) GameController.methods.v1)
			GameController.activeVList.Remove (Int32.Parse (other.name));
		else if (GameController.currentMethod == (int) GameController.methods.TL)
			GameController.activeV[other.GetComponent<Vehicle>().GetLane()].Remove (Int32.Parse (other.name));
		Destroy (other.gameObject);



	}
}
