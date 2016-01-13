using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {

	public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		GetComponent<Rigidbody>().velocity = transform.right * speed;
	}

	public void SetSpeed(float x)
	{
		speed = x;
	}
}
