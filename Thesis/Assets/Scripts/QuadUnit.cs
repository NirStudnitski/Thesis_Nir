using UnityEngine;
using System.Collections;

public class QuadUnit : MonoBehaviour {

	const float HALF_PI = 1.570796f;
	public int index = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (GameController.simulationOn)
		{
			if (GameController.currentMethod == (int)GameController.methods.v1)
			{
				if (GameController.doneWithCheck)
					transform.position = new Vector3 (10000, 0, 0);
				else if (index < GameController.numCloseV &&
				        GameController.numCloseV > 1 && GameController.simulationOn)
				{
			
					Vector2 finalTemp;
					float tempLength = GameController.futureVehicles [index].halfLength;
					float tempWidth = GameController.futureVehicles [index].halfWidth;
					Vector2 temp = GameController.futureVehicles [index].currentLocation;
					Vector2 temp2 = GameController.futureVehicles [index].direction;
					Vector2 temp2Perp = RotateCCW (temp2, HALF_PI) * tempWidth;
					temp2 *= tempLength;
					finalTemp = temp;
					Mesh mesh = GetComponent<MeshFilter> ().mesh;
					Vector3[] vertices = mesh.vertices;


					int i = 0;
					while (i < vertices.Length)
					{
						switch (i)
						{
						case 0:
							finalTemp = temp2 + temp2Perp;	
							break;
						case 1:
							finalTemp = -temp2 + temp2Perp;			
							break;
						case 2:
							finalTemp = -temp2 - temp2Perp;
							break;
						case 3:
							finalTemp = temp2 - temp2Perp;
							break;
						}
						vertices [i] = new Vector3 (finalTemp.x, 0, finalTemp.y);

						i++;
					}
					mesh.vertices = vertices;

					mesh.triangles = new int[] { 0, 3, 2, 0, 2, 1 };
					transform.position = new Vector3 (temp.x, -1.8f, temp.y);


				} 
			} else if (GameController.currentMethod == (int)GameController.methods.v2)
			{
				if (index < GameController.lastIndexes [GameController.nowFrame])
				{

					Vector2 finalTemp, temp, temp2, temp2Perp;
					float tempLength, tempWidth;
					if (GameController.inStepSim)
					{
						int nextFrame = GameController.nowFrame + 1;
						nextFrame %= GameController.rowsInDataCenter;
						tempLength = GameController.dataCenter [nextFrame, index * 7 + 4];
						tempWidth = GameController.dataCenter [nextFrame, index * 7 + 5];
						temp = new Vector2 (GameController.dataCenter [nextFrame, index * 7], 
							GameController.dataCenter [nextFrame, index * 7 + 1]);
						temp2 = new Vector2 (GameController.dataCenter [nextFrame, index * 7 + 2], 
							GameController.dataCenter [nextFrame, index * 7 + 3]);
						temp2Perp = RotateCCW (temp2, HALF_PI) * tempWidth;
						temp2 *= tempLength;
						finalTemp = temp;
					} else
					{
						tempLength = GameController.dataCenter [GameController.nowFrame, index * 7 + 4];
						tempWidth = GameController.dataCenter [GameController.nowFrame, index * 7 + 5];
						temp = new Vector2 (GameController.dataCenter [GameController.nowFrame, index * 7], 
							GameController.dataCenter [GameController.nowFrame, index * 7 + 1]);
						temp2 = new Vector2 (GameController.dataCenter [GameController.nowFrame, index * 7 + 2], 
							GameController.dataCenter [GameController.nowFrame, index * 7 + 3]);
						temp2Perp = RotateCCW (temp2, HALF_PI) * tempWidth;
						temp2 *= tempLength;
						finalTemp = temp;
					}
					
					Mesh mesh = GetComponent<MeshFilter> ().mesh;
					Vector3[] vertices = mesh.vertices;


					int i = 0;
					while (i < vertices.Length)
					{
						switch (i)
						{
						case 0:
							finalTemp = temp2 + temp2Perp;	
							break;
						case 1:
							finalTemp = -temp2 + temp2Perp;			
							break;
						case 2:
							finalTemp = -temp2 - temp2Perp;
							break;
						case 3:
							finalTemp = temp2 - temp2Perp;
							break;
						}
						vertices [i] = new Vector3 (finalTemp.x, 0, finalTemp.y);

						i++;
					}
					mesh.vertices = vertices;

					mesh.triangles = new int[] { 0, 3, 2, 0, 2, 1 };
					transform.position = new Vector3 (temp.x, -1.8f, temp.y);


				} else
					transform.position = new Vector3 (10000, 0, 0);
			}
		} else
			transform.position = new Vector3 (10000, 0, 0);

		
	}

	public void SetIndex(int i)
	{
		index = i;
	}


	private Vector2 RotateCCW (Vector2 vIn, float angle)
	{
		return new Vector2 (vIn.x * Mathf.Cos (angle) - vIn.y * Mathf.Sin (angle),
			vIn.x * Mathf.Sin (angle) + vIn.y * Mathf.Cos (angle));
	}
}
