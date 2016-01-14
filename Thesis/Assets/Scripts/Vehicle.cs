using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {

	public float speed;
	public int lane;
	private bool turnInitiate = false;
	public int turnPlan; // 0 = keep going straight, 1 = turn right, -1 = turn left 
	float turnCounter = 0;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		GetComponent<Rigidbody>().velocity = transform.right * speed;
		if (turnPlan!=0)
			if (   -33f < transform.position.x && transform.position.x < 33f
		    	&& -33f < transform.position.z && transform.position.z < 33f)
					turnInitiate = true;
		if (turnInitiate && turnCounter<=90)
		{
			transform.rotation = Quaternion.Euler (0f, -turnCounter + (lane/2)*90f, 0f);
			turnCounter++;
		}
	}

	public void SetSpeed(float x)
	{
		speed = x;
	}

	public void SetTurnPlan(int x)
	{
		turnPlan = x;
	}

	public void SetLane(int x)
	{
		lane = x;
	}
}
