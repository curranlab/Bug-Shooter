using UnityEngine;
using System.Collections;

public class Done_Mover : MonoBehaviour
{
	public float speed;
	private Vector3 tempVector;
	
	private Done_GameController3 gameController3;
	
	void Start ()
	{
		/* Get original vector information when bug first created for directional speed, 
		 otherwise when we reset it'll set the direction randomly instead of the forward that
		 was set when the bug was created. */
		GetComponent<Rigidbody> ().velocity = transform.forward * speed;
		tempVector = GetComponent<Rigidbody> ().velocity;
		Debug.Log("velocity = " + tempVector);
		// get access to game controller script
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController3 = gameControllerObject.GetComponent <Done_GameController3>();
		}
		if (gameController3 == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	public void FixedUpdate ()
	{
		// if this isn't the example bug, don't change speed based on pause
		if (!gameObject.name.Substring(0,4).Equals(gameController3.exampleBug.name.Substring(0,4)))
		{
			// if there are multiple bugs in the danger zone, stop moving
			if (gameController3.GetPause() > 1)
			{
				GetComponent<Rigidbody>().velocity = transform.forward * 0;
			}
			// after pause is reset AND if bug is not moving, start moving again
			if (gameController3.GetPause() <= 1 && GetComponent<Rigidbody> ().velocity == transform.forward * 0)
			{
				GetComponent<Rigidbody>().velocity = tempVector;
			}
		}
	}
}