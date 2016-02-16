using UnityEngine;
using System.Collections;
using System;

public class ExitBox : MonoBehaviour {

	void OnTriggerEnter(Collider other){

		GameController.activeVList.Remove (Int32.Parse (other.name));
		Destroy (other.gameObject);



	}
}
