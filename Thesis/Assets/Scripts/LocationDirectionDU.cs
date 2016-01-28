using UnityEngine;
using System.Collections;

public class LocationDirectionDU : MonoBehaviour {

	public float speed;
	public Vector2 location, direction;
	public int turnCounter, lane, turnPlan;
	bool turnInitiate = false;


	public LocationDirectionDU(float speedIn, Vector2 locIn, Vector2 dirIn, int turnCountIn, int laneIn, int turnPlanIn, bool turnInit)
	{
		speed = speedIn;
		turnInitiate = turnInit;
		location = locIn;
		direction = dirIn;
		lane = laneIn;
		turnCounter = turnCountIn;
		turnPlan = turnPlanIn;
	}

	public void Advance()
	{

		Vector2 directionV2 = RotateCCW (new Vector2 (0f, 1f), direction.y);
		location += directionV2 * speed;
		if (turnPlan == 1) 
		{
			if (!turnInitiate ) if (-33f < location.x && location.x < 33f
				&& -33f < location.y && location.y < 33f)
				turnInitiate = true;
			if (turnInitiate && turnCounter <= 90) 
			{
				transform.rotation = Quaternion.Euler (0f, -turnCounter + (lane / 2) * 90f, 0f);
				turnCounter++;
			}
		} 
		else if (turnPlan == -1) 
		{
			if (!turnInitiate) if (-39f < location.x && location.x < 39f
				&& -39f < location.y && location.y < 39f)
				turnInitiate = true;
			if (turnInitiate && turnCounter <= 90) {
				transform.rotation = Quaternion.Euler (0f, turnCounter + (lane / 2) * 90f, 0f);
				turnCounter+=2;
			}
		}
	}

	private Vector2 RotateCCW (Vector2 vIn, float angle)
	{
		return new Vector2 (vIn.x * Mathf.Cos (angle) - vIn.y * Mathf.Sin (angle),
			vIn.x * Mathf.Sin (angle) + vIn.y * Mathf.Cos (angle));
	}

}
