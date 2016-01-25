using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public void ChangeDistance(float fIn)
	{
		this.transform.position = new Vector3 (-fIn, -3.4f +fIn, -fIn);


	}
}
