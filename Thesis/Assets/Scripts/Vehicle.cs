using UnityEngine;
using System.Collections;
using System;

public class Vehicle : MonoBehaviour {

	public float speed;


	//these are half the length and width of each vehicle
	public float size, width;
	public int lane;
	public int type;
	private bool turnInitiate = false, noCollision = true;
	public int turnPlan; // 0 = keep going straight, 1 = turn right, -1 = turn left 
	private float turnCounter = 0;



	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!GameController.pause) 
		{
			
			GetComponent<Rigidbody> ().velocity = transform.right * speed;

			if (turnPlan == 1) {
				if (!turnInitiate)
				if (-33f < transform.position.x && transform.position.x < 33f
				                       && -33f < transform.position.z && transform.position.z < 33f)
					turnInitiate = true;
				if (noCollision)
				if (turnInitiate && turnCounter <= 90) {
					transform.rotation = Quaternion.Euler (0f, -turnCounter + (lane / 2) * 90f, 0f);
					turnCounter++;
				}
			} else if (turnPlan == -1) {
				if (!turnInitiate)
				if (-39f < transform.position.x && transform.position.x < 39f
				                      && -39f < transform.position.z && transform.position.z < 39f)
					turnInitiate = true;
				if (noCollision)
				if (turnInitiate && turnCounter <= 90) {
					transform.rotation = Quaternion.Euler (0f, turnCounter + (lane / 2) * 90f, 0f);
					turnCounter += 2;
				}
			}
			if (-290f > transform.position.x || transform.position.x > 290f
			   || -290f > transform.position.z || transform.position.z > 290f) {
			
				GameController.activeVList.Remove (Int32.Parse (this.name));
				//GameController.activeVList.PrintList ();

				Destroy (this.gameObject);
			}
		}

	}

	public void SetSpeed(float x)
	{
		speed = x;
	}

	public void SetSize(float x)
	{
		size = x;
	}

	public float GetSpeed()
	{
		return speed;
	}

	public float GetSize()
	{
		return size;
	}

	public void SetTurnPlan(int x)
	{
		turnPlan = x;
	}

	public void SetLane(int x)
	{
		lane = x;
	}

	public void SetType(int x)
	{
		type = x;
	}

	public int GetType()
	{
		return type;
	}

	public void CollisionHappened()
	{
		noCollision = false;
	}
}
