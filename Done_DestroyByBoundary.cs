using UnityEngine;
using System.Collections;

public class Done_DestroyByBoundary : MonoBehaviour
{	
	private Done_GameController3 gameController3;
	private Done_PlayerController2 playerController2;
	
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
		// get access to player controller script
		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		if (playerObject != null)
		{
			playerController2 = playerObject.GetComponent <Done_PlayerController2>();
		}
		if (playerController2 == null)
		{
			Debug.Log ("Cannot find 'PlayerController2' script");
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Enemy")
		{
			// reset feedback text as remaining bug leaves and before next set reaches danger zone
			gameController3.FeedbackText("", Color.gray);
			// reset bucket color
			playerController2.ResetMat();

			// reset bug
			other.gameObject.SetActive(false);
			// remove from active hazards
			gameController3.activeHazards.Remove(other.gameObject.name);
		}
	}
}