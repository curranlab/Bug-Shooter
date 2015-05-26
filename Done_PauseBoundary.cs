using UnityEngine;
using System.Collections;

public class Done_PauseBoundary : MonoBehaviour
{	
	private Done_GameController3 gameController3;

	void Start ()
	{
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

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Enemy") {
			/* Add to pause for each enemy, we don't want to stop if there's only
			 1 enemy, so when this becomes 2 the Done_Mover will stop all bugs */
			gameController3.pause += 1;
		}
	}
}