using UnityEngine;
using System.Collections;
using System;

public class ExitBox : MonoBehaviour {

	void OnTriggerEnter(Collider other){

		if (GameController.currentMethod != (int) GameController.methods.v2)
			GameController.activeVList.Remove (Int32.Parse (other.name));
		Destroy (other.gameObject);



	}
}
