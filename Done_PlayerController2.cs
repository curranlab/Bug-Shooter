using UnityEngine;
using System.Collections;
using System;

[System.Serializable]

public class Done_PlayerController2 : MonoBehaviour
{
	public float speed;
	public Material newmat;

	private float sitStill;
	private Material origmat;
	private bool wait;
	private Done_GameController3 gameController3;
	private Vector3 home = new Vector3 (0.0f, 0.0f, -5);
	private float endWaitTime = 0;
	private OutputFile outputFile;

	void Start()
	{
		// get original material for reseting later
		origmat = gameObject.GetComponent<Renderer>().material;

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
		// get access to output file script
		GameObject outputFileObject = GameObject.FindGameObjectWithTag ("ObjectPooler");
		if (outputFileObject != null) {
			outputFile = outputFileObject.GetComponent <OutputFile> ();
		}
		if (outputFile == null) {
			Debug.Log ("Cannot find 'OutputFile' script");
		}
	}

	/* Keep as Update instead of FixedUpdate because Update is faster. 
	 * FixedUpdate doesn't always detect resp on first key press. */
	void Update ()
	{// responses use left & right arrow keys
		if ((gameController3.activeHazards.Count % 2 == 0) && (Time.time > endWaitTime))
		{
			int trialNum = gameController3.GetResponseNumber();
			DateTime respTime = System.DateTime.Now;
			string timeString = respTime.ToString ("HH:mm:ss.ffff");
			int runs = gameController3.GetRunNumber()+1;
			int blocks = gameController3.GetBlockNumber()+1;

			// left response
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				Vector3 end = new Vector3 (-8, 0.0f, -5);
				GetComponent<Rigidbody>().transform.position = Vector3.Lerp (transform.position, end, speed);
				outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t{3}\tLEFT", timeString, runs, blocks, trialNum));
			}
			// right response
			if (Input.GetKeyDown (KeyCode.RightArrow))
			{
				Vector3 end = new Vector3 (8, 0.0f, -5);
				GetComponent<Rigidbody>().transform.position = Vector3.Lerp (transform.position, end, speed);
				outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t{3}\tRIGHT", timeString, runs, blocks, trialNum));
			}
		}
		/* Resets the bucket position. Checks that it's been in response direction location for a period
		 of time (set in inspector), then changes position back to original/home position. */
//		if (GetComponent<Rigidbody>().position != home && Time.time > resetTime)
//		{
//			GetComponent<Rigidbody>().transform.position = Vector3.Lerp (transform.position, home, speed);
//		}
	}

	void OnTriggerEnter(Collider other) {
		endWaitTime = Time.time + 1.0f;
	}

	public void ResetPlayer ()
	{
		GetComponent<Rigidbody>().transform.position = Vector3.Lerp (transform.position, home, speed);
	}

	public void StartWait() {
		endWaitTime = Time.time + 2.0f;
	}

	public void ChangeMat()
	{
		// set material to alternate material attached to script in inspector
		gameObject.GetComponent<Renderer>().material = newmat;
	}

	public void ResetMat()
	{
		// reset material to bucket_mat
		gameObject.GetComponent<Renderer>().material = origmat;
	}
}