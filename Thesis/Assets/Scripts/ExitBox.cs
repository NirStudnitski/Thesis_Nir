using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ExitBox : MonoBehaviour {

	void OnTriggerEnter(Collider other){

		if (GameController.currentMethod == (int)GameController.methods.v1)
			GameController.activeVList.Remove (Int32.Parse (other.name));
		else if (GameController.currentMethod == (int)GameController.methods.TL)
		{
			GameObject temp = GameController.cars.Find(x => x.GetComponent<Vehicle> ().name == Int32.Parse (other.name));
			GameController.cars.Remove (temp);
			GameController.activeV [other.GetComponent<Vehicle> ().GetLane ()].Remove (Int32.Parse (other.name));


		}
		Destroy (other.gameObject);
		GameController.sumOfFrames += other.GetComponent<Vehicle> ().framesAlive;
		GameController.madeItThrough++;


	}
}
