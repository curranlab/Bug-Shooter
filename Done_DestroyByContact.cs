using UnityEngine;
using System.Collections;
using System;

public class Done_DestroyByContact : MonoBehaviour
{
	public int scoreValue;
	Color bucketColor;
	private Done_GameController3 gameController3;
	private Done_PlayerController2 playerController2;
	private OutputFile outputFile;
	private AudioSource correctSound;
	private AudioSource incorrectSound;

	WaitForSeconds shortWFS = new WaitForSeconds(0.1f);

	void Start ()
	{
		// get access to game controller script
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController3 = gameControllerObject.GetComponent <Done_GameController3> ();
		}
		if (gameController3 == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
		// get access to player controller script
		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		if (playerObject != null) {
			playerController2 = playerObject.GetComponent <Done_PlayerController2> ();
		}
		if (playerController2 == null) {
			Debug.Log ("Cannot find 'PlayerController2' script");
		}
		// get access to 2 game objects that each have one sound
		GameObject corsoundObject = GameObject.FindGameObjectWithTag ("CorrectSound");
		if (corsoundObject != null) {
			correctSound = corsoundObject.GetComponent <AudioSource> ();
		}
		if (correctSound == null) {
			Debug.Log ("Cannot find 'correctSound' auodiosouce ");
		}
		GameObject incorsoundObject = GameObject.FindGameObjectWithTag ("IncorrectSound");
		if (incorsoundObject != null) {
			incorrectSound = incorsoundObject.GetComponent <AudioSource> ();
		}
		if (incorrectSound == null) {
			Debug.Log ("Cannot find 'incorrectSound' auodiosouce ");
		}
		// get access to output file script
		GameObject outputFileObject = GameObject.FindGameObjectWithTag ("ObjectPooler");
		if (outputFileObject != null) {
			outputFile = outputFileObject.GetComponent <OutputFile> ();
		}
		if (outputFile == null) {
			Debug.Log ("Cannot find 'OutputFile' script");
		}
	}

	/* When a bug hits the bucket, this function (this bug) determines accuracy of response */
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") { 
			playerController2.ResetPlayer (); // move bucket back to center
			int trialNum = gameController3.GetResponseNumber();
			DateTime respTime = System.DateTime.Now;
			string timeString = respTime.ToString ("HH:mm:ss.ffff");
			string accuracy;
			int runs = gameController3.GetRunNumber()+1;
			int blocks = gameController3.GetBlockNumber()+1;

			bool acc = gameController3.SpeciesMatch (gameObject); // does this bug's species match the example?
			if (acc == true) {	// correct
				gameController3.AddScore (scoreValue); // increase score
				gameController3.FeedbackText ("Correct", Color.green); // feedback text
				correctSound.Play (); // play higher tone/beep
				accuracy = "1";

			} else {	// incorrect
				gameController3.FeedbackText ("Incorrect", Color.red); // feedback text
				playerController2.ChangeMat (); // change bucket color
				incorrectSound.Play (); // play lower tone/beep
				accuracy = "0";

			}
			outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t{3}\tRESP\t{4}\t{5}", timeString, runs, blocks, trialNum, gameObject.name, accuracy));
			StartCoroutine (Wait (acc));
		}
	}
	/* If response was correct, this function will just reset the bugs and value of pause.
	 If response was incorrect, it hides the bug (but still needs to keep it active),
	 pauses for a specified number of seconds so subject can study the remaining correct bug,
	 and then resets the bugs and value of pause. This function (& the pause) are attached to
	 the bug, so we can't reset it until we're ready to be done with the script. */
	IEnumerator Wait (bool resp)
	{
		if (resp == true) {
			gameObject.SetActive (false); // reset bug
			gameController3.activeHazards.Remove (gameObject.name); // remove from active hazards
			gameController3.PlayerAnswered (true);
		} else {
			Vector3 hidePosition = new Vector3 (-100, 0, 0); // hide bug but keep active
			gameObject.GetComponent<Transform> ().position = hidePosition;

			yield return shortWFS; // wait this many seconds, set at top of script
			gameObject.SetActive (false); // reset bug
			gameController3.activeHazards.Remove (gameObject.name); // remove from active hazards
			gameController3.PlayerAnswered (false);
		}
	}

	void OnDisable ()
	{
		CancelInvoke (); // to prevent invoking while resetting properties
	}
}	