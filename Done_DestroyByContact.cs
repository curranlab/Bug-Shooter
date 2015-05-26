using UnityEngine;
using System.Collections;
using System;

public class Done_DestroyByContact : MonoBehaviour
{
	public int scoreValue;
	
	Color bucketColor;
	private Done_GameController3 gameController3;
	private Done_PlayerController2 playerController2;
	private AudioSource correctSound;
	private AudioSource incorrectSound;

	WaitForSeconds penaltyWFS = new WaitForSeconds(5);//gameController3.penaltyWait);

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
		// get access to 2 game objects that each have one sound
		GameObject corsoundObject = GameObject.FindGameObjectWithTag ("CorrectSound");
		if (corsoundObject != null)
		{
			correctSound = corsoundObject.GetComponent <AudioSource>();
		}
		if (correctSound == null)
		{
			Debug.Log ("Cannot find 'correctSound' auodiosouce ");
		}
		GameObject incorsoundObject = GameObject.FindGameObjectWithTag ("IncorrectSound");
		if (incorsoundObject != null)
		{
			incorrectSound = incorsoundObject.GetComponent <AudioSource>();
		}
		if (incorrectSound == null)
		{
			Debug.Log ("Cannot find 'incorrectSound' auodiosouce ");
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
		{ 
			bool acc = gameController3.SpeciesMatch(gameObject);
			if (acc == true)
			{	// correct
				gameController3.AddScore(scoreValue);
				// trigger feedback text
				gameController3.FeedbackText("Correct", Color.green);
				// play higher tone/beep
				correctSound.Play();		
			}
			else
			{	// incorrect
				// trigger feedback text
				gameController3.FeedbackText("Incorrect", Color.red);
				// change bucket color
				playerController2.ChangeMat();
				// prevent bucket from moving
				playerController2.SetWait(true);
				// play lower tone/beep
				incorrectSound.Play();
			}
			StartCoroutine(Wait(acc));
		}
	}
	/* If response was correct, this function will just reset the bug and pause.
	 If response was incorrect, it hides the bug (but still needs to keep it active),
	 pauses for a specified number of seconds so subject can study the remaining correct bug,
	 and then continues to reset the bug and pause. This function (& the pause) are attached to
	 the bug, so we can't reset it until we're ready to be done with the script. */
	IEnumerator Wait(bool resp)
	{
		if (resp == true)
		{
			// reset instead of destroy
			gameObject.SetActive(false); 
			// remove from active hazards
			gameController3.activeHazards.Remove(gameObject.name);
			// reset pause
			gameController3.SetPause(0.0f);
		}
		else
		{
			// wait until bug is in danger zone?

			// hide bug but keep active
			Vector3 hidePosition = new Vector3 (-100 , 0, 0);
			gameObject.GetComponent<Transform>().position = hidePosition;
			// wait this many seconds, set in inspector/game controller script
			yield return penaltyWFS;
			// reset instead of destroy
			gameObject.SetActive(false); 
			// remove from active hazards
			gameController3.activeHazards.Remove(gameObject.name);
			// let bucket move again
			playerController2.SetWait(false);
			// reset pause
			gameController3.SetPause(0.0f);
		}
	}

	void OnDisable() 
	{
		CancelInvoke(); // to prevent invoking while resetting properties
	}
}	