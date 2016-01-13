using UnityEngine;
using System.Collections;

public class Motion : MonoBehaviour 
{
	

	void Update ()
	{
		GetComponent<Rigidbody>().velocity = transform.right * GetComponent<Vehicle>().speed;
	}
}