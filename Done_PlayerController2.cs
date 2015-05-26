using UnityEngine;
using System.Collections;

[System.Serializable]

public class Done_PlayerController2 : MonoBehaviour
{
	public float speed;
	public float resetRate;
	public Material newmat;

	private float resetTime;
	private float sitStill;
	private Material origmat;
	private bool wait;
	private Done_GameController3 gameController3;

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
	}

	/* Keep as Update instead of FixedUpdate because Update is faster. 
	 * FixedUpdate doesn't always detect resp on first key press. */
	void Update ()
	{	
		// responses use left & right arrow keys
		Vector3 home = new Vector3 (0.0f, 0.0f, -5);

		if ((gameController3.activeHazards.Count % 2 == 0) && (wait == false))
		{
			// left response
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				Vector3 end = new Vector3 (-7, 0.0f, -5);
				GetComponent<Rigidbody>().transform.position = Vector3.Lerp (transform.position, end, speed);
				resetTime = Time.time + resetRate;
			}
			// right response
			if (Input.GetKeyDown (KeyCode.RightArrow))
			{
				Vector3 end = new Vector3 (7, 0.0f, -5);
				GetComponent<Rigidbody>().transform.position = Vector3.Lerp (transform.position, end, speed);
				resetTime = Time.time + resetRate;
			}
		}
		/* Resets the bucket position. Checks that it's been in response direction location for a period
		 of time (set in inspector), then changes position back to original/home position. */
		if (GetComponent<Rigidbody>().position != home && Time.time > resetTime)
		{
			GetComponent<Rigidbody>().transform.position = Vector3.Lerp (transform.position, home, speed);
		}
	}

	public void SetWait(bool newWait)
	{
		wait = newWait;
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