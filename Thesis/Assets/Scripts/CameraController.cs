using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public void ChangeDistance(float fIn)
	{
		if (GameController.cameraAbove)
		{
			this.transform.position = new Vector3 (0, -3.4f +fIn, 0);
			this.transform.rotation = Quaternion.identity * Quaternion.Euler (90f, 0f, 0f);

		}
		else this.transform.position = new Vector3 (-fIn, -3.4f +fIn, -fIn);


	}
}
