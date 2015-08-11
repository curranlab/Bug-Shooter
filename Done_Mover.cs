using UnityEngine;
using System.Collections;

public class Done_Mover : MonoBehaviour
{
	public float speed;
	[HideInInspector]
	public Vector3 tempVector;
	private Done_GameController3 gameController3;

	void Start ()
	{
		/* Get original vector information when bug first created for directional speed, 
		 otherwise when we reset it'll set the direction randomly instead of the forward that
		 was set when the bug was created. */
		GetComponent<Rigidbody> ().velocity = transform.forward * speed;
		tempVector = GetComponent<Rigidbody> ().velocity;
		// get access to game controller script
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController3 = gameControllerObject.GetComponent <Done_GameController3> ();
		}
		if (gameController3 == null) {
			UnityEngine.Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	public void Update ()
	{
		// if this isn't the example bug, don't change speed based on pause
		if (!gameObject.name.Substring (0, 4).Equals (gameController3.exampleBug.name.Substring (0, 4))) {
			if (gameController3.Paused ()) {
				GetComponent<Rigidbody> ().velocity = tempVector * 0;
			} else {
				GetComponent<Rigidbody> ().velocity = tempVector;
			}
		}
	}
}