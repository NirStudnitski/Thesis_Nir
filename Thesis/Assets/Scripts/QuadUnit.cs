using UnityEngine;
using System.Collections;

public class QuadUnit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector2 temp = GameController.activeVList.GetCurrentPosition (0);
		//transform.rotation = Quaternion.identity * Quaternion.Euler (0f,(GameController.activeVList.GetLane(0) / 2) * 90f,0f);
		transform.position = new Vector3 (temp.x, -1.8f, temp.y);
		transform.localScale = new Vector3 (GameController.activeVList.GetWidth(0)*2, GameController.activeVList.GetLength(0)*2, 0);
	}
}
