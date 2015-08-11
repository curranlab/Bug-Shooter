using UnityEngine;
using System.Collections;

public class Done_PauseBoundary : MonoBehaviour
{	
	private Done_GameController3 gameController3;
	private int collisionCount = 0;
	private string colliderOne;
	private string colliderTwo;

	void Start ()
	{
		collisionCount = 0;
		// get access to game controller script
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController3 = gameControllerObject.GetComponent <Done_GameController3> ();
		}
		if (gameController3 == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Enemy") {
			/* Add to pause for each enemy, we don't want to stop if there's only
			 1 enemy, so when this becomes 2 the Done_Mover will stop all bugs */

			/* NB: rotating bugs will continue to hit this boundary, so pause will
			 * continue to be incremented past 2 or 3, but always gets reset to 0
			 * for next trial */
			if (collisionCount == 0) {
//				colliderOne = other.name;  // don't count a second collision by same rotating object
				collisionCount++;
			}
			if (collisionCount == 1) {
//					colliderTwo = other.name;
					collisionCount = 0;  // reset
				if ((gameController3.activeHazards.Count % 2) == 0) {
					gameController3.StartPause (40.0f);
				}
			}
		}
	}
}