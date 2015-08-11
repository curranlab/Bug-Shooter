using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class Done_GameController3 : MonoBehaviour
{
	// variables we want to set in editor
	public Vector3 spawnValues;
	public int blockRuns; // how many groups of blocks for the session, score & speed reset between runs
	public int totalBlocks; // how many total blocks for the session (how many exemplars to train)
	public int trialsPerBlock; // how many trials per block (how many matches for each exemplar)
	public float trialWait; // how long to wait between making next pair of bugs visible, spaces out trials
	public float blockTimer; // time in seconds for each block
	public float penaltyWait;
	
	// list data structure to store all objects we'll use/need
	public List<string> activeHazards;

//	[HideInInspector] public GameObject exampleBug;
	public GameObject exampleBug;

	// text variables
	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;
//	public GUIText exampleText;
	public GUIText feedbackText;
	public GUIText timerText;

	// don't set in inspector
	int blocks = 0;
	int blocksPerRun;
	int runs = 0;
	int trials = 0;
	int possibleResponses;
	float possiblePoints;
	[HideInInspector]
	public string exampleName;
	bool isMatch;
	bool isFinishedLevel = true; // while this is false, timer counts down
	private bool restart = false;
	private int score = 0;
	private bool gameOver;
	private float blockTimerCopy;
	private DateTime nowTime;
	private string timeString;
	private Done_PlayerController2 playerController2;
	private ObjectPoolerScript objectPooler;
	private OutputFile outputFile;
	private AudioSource timeoutSound;
	WaitForSeconds blockWFS = new WaitForSeconds (0.5f);
	WaitForSeconds breakWFS = new WaitForSeconds (10);
	WaitForSeconds secondWFS = new WaitForSeconds (0.01f);
	public float maxBlockTime = 40.0f;
	public float nextSpawnTime = 0;
	public float remainingSpawnTime = 0;
	public bool  hadFirstSpawn = false;
	public float pauseEndTime = 0;
	private bool paused = false;

	/****************************************************************************
	* 	Start/on load of game
	* **************************************************************************/
	void Start ()
	{
		blocksPerRun = totalBlocks / blockRuns;
		possiblePoints = blocksPerRun * trialsPerBlock;
		feedbackText.text = "";
//		restartText.text = "";
		gameOverText.text = "";
		blockTimerCopy = blockTimer;
		timerText.text = string.Format ("Time: 00:{0:00}", (int)(blockTimer));
//		exampleText.text = "Target Species";
		UpdateScore ();

		// get access to player controller script
		GameObject objectPoolerObj = GameObject.FindGameObjectWithTag ("ObjectPooler");
		if (objectPoolerObj != null) {
			objectPooler = objectPoolerObj.GetComponent <ObjectPoolerScript> ();
		}
		if (objectPoolerObj == null) {
			UnityEngine.Debug.Log ("Cannot find 'ObjectPoolerScript' script");
		}
		// load objects
		while (objectPooler.IsBundleLoaded("sp")==false && objectPooler.IsBundleLoaded("ak")==false) {
			return;
		}

		objectPooler.LoadHazards ();
		objectPooler.MakeBigPools ();

		// get access to player controller script
		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		if (playerObject != null) {
			playerController2 = playerObject.GetComponent <Done_PlayerController2> ();
		}
		if (playerController2 == null) {
			UnityEngine.Debug.Log ("Cannot find 'PlayerController2' script");
		}

		// get access to game object with incorrect sound
		GameObject timesoundObject = GameObject.FindGameObjectWithTag ("TimeoutSound");
		if (timesoundObject != null) {
			timeoutSound = timesoundObject.GetComponent <AudioSource> ();
		}
		if (timeoutSound == null) {
			UnityEngine.Debug.Log ("Cannot find 'timeoutSound' auodiosouce ");
		}
		// get access to output file script
		GameObject outputFileObject = GameObject.FindGameObjectWithTag ("ObjectPooler");
		if (outputFileObject != null) {
			outputFile = outputFileObject.GetComponent <OutputFile> ();
		}
		if (outputFile == null) {
			UnityEngine.Debug.Log ("Cannot find 'OutputFile' script");
		}

		StartCoroutine ("SpawnWaves");
	}
	/****************************************************************************
	* 	Update
	* **************************************************************************/
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			// delete all player prefs
			PlayerPrefs.DeleteAll ();
			// quit application
			Application.Quit ();
		}
		if (gameOver) {
			// delete all player prefs
			PlayerPrefs.DeleteAll ();
			// quit application
			Application.Quit ();
		}
		if (restart) {
			if (Input.GetKeyDown (KeyCode.N)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
		/* Timer for blocks (w/in Update to keep track of state) */
		// only count down while we're in a block
		if (!isFinishedLevel) { // has the level been completed
			blockTimer -= Time.deltaTime; // I need timer which from a particular time goes to zero
			if (blockTimer < 0)
				blockTimer = 0;
			int seconds = (int)(blockTimer % 60);
			int minutes = (int)(blockTimer / 60);
			timerText.text = string.Format ("Time: {0:00}:{1:00}", minutes, seconds);
		} 
		// if we haven't reached 0 yet, do nothing	
		if (blockTimer > 0)
			return; 
		// once the countdown timer reaches 0, run game over function & update timer text
		else {
			StopCoroutine ("SpawnWaves");
			timerText.text = "Time: 00:00";
			TimeOut ();
		}
	}

	/****************************************************************************
	 * 	Main experiment block/trial loop
	 * **************************************************************************/
	IEnumerator SpawnWaves ()
	{
		/* Wait a second before starting each block */
		yield return secondWFS;

		Vector3 spawnPositionLeft = new Vector3 (-8, spawnValues.y, spawnValues.z);
		Vector3 spawnPositionRight = new Vector3 (8, spawnValues.y, spawnValues.z);
		Quaternion spawnRotation = Quaternion.identity; /* Both */
		GameObject hazard = null;
		string name;

		for (; runs < blockRuns; runs++) {
			// prints beginning of run, also when restarting after a time out
//			outputFile.WriteLine("Run " + (runs+1));
			
//			blocks = 0; // reset at the end of runs so time out doesn't restart block count early

			/* Set loop to run for however many blocks we want. Each block trains 1 species
			 for trialCount number of trials. */
			for (; blocks < blocksPerRun; blocks++) {
				nowTime = System.DateTime.Now;
				timeString = nowTime.ToString ("HH:mm:ss.ffff");
//				outputFile.WriteLine(String.Format ("Block {0} started at {1}", blocks+1, timeString));
				outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t0", timeString, runs+1, blocks+1));
				playerController2.ResetPlayer();
				gameOverText.text = "";
				yield return blockWFS; // makes example transition more obvious
				exampleBug = objectPooler.CreateExample (); /* Make new example bug for each block */
//				outputFile.WriteLine("Example: " + exampleBug.name);
				outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t0\tEXAMPLE\t{3}", timeString, runs+1, blocks+1, exampleBug.name));
				objectPooler.MakeMatchPool (); /* Make pool of matching bugs based on example bug species */
				objectPooler.MakeBlockLurePool (); /* Make pool of lures we'll use for each block */
				isFinishedLevel = false; /* Start timer again */

				Reset ();
				trials = 0;
				possibleResponses = trialsPerBlock; // keeps track of how many responses are left
				float trialWaitTime = TrialWait(); // how long to wait between trials based on score
				float blockEndTime = Time.time + blockTimer;

				while (trials < trialsPerBlock && Time.time < blockEndTime) {		
					if (!Paused () && IsTimeToSpawn ()) {
						/* Randomly generates # greater than or equal to 0 and less than 10. If random number is
					 in upper half of range assign false else assign true */
						isMatch = (UnityEngine.Random.Range (0, 10) < 5) ? true : false;

						/* Left bug always gets isMatch result (true/false) and Right bug always opposite */
						hazard = objectPooler.GetPooledObjects (isMatch); 
						hazard.GetComponent<Transform> ().position = spawnPositionLeft;
						hazard.GetComponent<Transform> ().rotation = spawnRotation;
						/* Add prefab name to list that keeps track of active bugs */
						name = hazard.name;
						activeHazards.Add (name);
						hazard.SetActive (true);
						nowTime = System.DateTime.Now;
						timeString = nowTime.ToString ("HH:mm:ss.ffff");
						outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", timeString, runs+1, blocks+1, trials+1, isMatch, name));

						/* Right */
						hazard = objectPooler.GetPooledObjects (!isMatch); 
						hazard.GetComponent<Transform> ().position = spawnPositionRight;
						hazard.GetComponent<Transform> ().rotation = spawnRotation;
						/* Add prefab name to list that keeps track of active bugs */
						name = hazard.name;
						activeHazards.Add (name);
						hazard.SetActive (true);
						nowTime = System.DateTime.Now;
						timeString = nowTime.ToString ("HH:mm:ss.ffff");
						outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", timeString, runs+1, blocks+1, trials+1, !isMatch, name));

						/* Wait a little before spawning the next pair, depending on incorrect penalty wait and pause */
						nextSpawnTime = Time.time + trialWaitTime;
						trials += 1;
					}

					yield return secondWFS;
				} // end of trial loop
				/* Don't remove example until all active bugs have passed through game area */
				while (activeHazards.Count != 0) {
					yield return secondWFS;
				}

				/* Set as finished level to stop countdown, reset timer */
				isFinishedLevel = true;
				blockTimer = blockTimerCopy;
				timerText.text = string.Format ("Time: 00:{0:00}", (int)(blockTimerCopy));

				/* Reset example bug, lure pool and matchPool list */
				objectPooler.ResetExample ();
				objectPooler.ClearMatchPool ();
				objectPooler.ClearBlockLurePool ();

			} // end block loop

			if (runs < blockRuns) {
				blocks = 0;
				FeedbackText ("Brief break", Color.white);
				yield return breakWFS;
				// reset score so speed starts off slower at beginning of new run
				score = 0;
				UpdateScore ();
				FeedbackText ("", Color.grey);
			}
		} // end run loop
		gameOverText.text = "This session is finished, thank you!";
		yield return new WaitForSeconds (5);
		gameOver = true;
		FeedbackText ("", Color.grey);
	}

	/****************************************************************************
	 * 	Updating setters & helper functions
	 * **************************************************************************/
	public bool IsTimeToSpawn ()
	{
		if (!hadFirstSpawn || Time.time > nextSpawnTime) {
			hadFirstSpawn = true;
			return true;
		} else {
			return false;
		}
	}

	public bool Paused ()
	{
		if (Time.time < pauseEndTime) {
			return true;
		} else {
			ClearPause ();
			return false;
		}
	}

	public void StartPause (float duration)
	{
		if (!paused) {
			remainingSpawnTime = nextSpawnTime - Time.time;
			pauseEndTime = Time.time + duration;
			nextSpawnTime += duration;
			paused = true;
		}
	}

	public void ClearPause ()
	{
		if (paused) {
			// reset the spawn time
			nextSpawnTime = remainingSpawnTime + Time.time;
			pauseEndTime = 0;
			paused = false;
		}
	}
	
	public void Reset ()
	{
		pauseEndTime = 0;
		nextSpawnTime = 0;
		hadFirstSpawn = false;
		paused = false;
	}

	public void PlayerAnswered (bool wasCorrect)
	{
		if (wasCorrect) {
			ClearPause ();
			possibleResponses-=1; // decrement # of responses left
		} else {
			ClearPause ();  // clear nay existing pause
			StartPause (penaltyWait);
			possibleResponses-=1;
		}
	}
	public int GetResponseNumber()
	{
		return trialsPerBlock - possibleResponses + 1;
	}
	public int GetRunNumber()
	{
		return runs;
	}
	public int GetBlockNumber()
	{
		return blocks;
	}
	/* Feedback text */
	public void FeedbackText (string acc, Color color)
	{
		feedbackText.text = acc;
		feedbackText.color = color;
	}
	/* Update score */
	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}
	
	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}

	/* Helper function to determine if species are the same between 2 bugs */
	public bool SpeciesMatch (GameObject go)
	{
		// parse gameObject & example bug names to species level
		// name format = aa13(Clone)
		//string species = go.name.Substring (0, 2);
		//string exampleSpecies = exampleBug.name.Substring (0, 2);
		string species = go.name.Substring (1, 1);
		string exampleSpecies = exampleBug.name.Substring (1, 1);
		
		// if correct return true
		if (species.Equals (exampleSpecies))
			return true;
		else
			return false;
	}

	/****************************************************************************
	 * 	Go faster as score increases
	 * **************************************************************************/
	public float TrialWait ()
	{
		/* trialWait set in inspector for starting wait time between trials, 
		 * gradually decrease wait time as score increases, with a minimum of 
		 * 2 seconds so bucket doesn't select 2 bugs at a time */
		float waitCalc = Mathf.Max((1.0f-score/possiblePoints)*trialWait,2.5f);
		return waitCalc;
	}

	/****************************************************************************
	 * 	Block over
	 * **************************************************************************/
	public void TimeOut ()
	{
		nowTime = System.DateTime.Now;
		timeString = nowTime.ToString ("HH:mm:ss.ffff");
		string accuracy = "0";
		string name = "0";
		for (; possibleResponses > 0; possibleResponses--)
		{
			outputFile.WriteLine (String.Format ("{0}\t{1}\t{2}\t{3}\tRESP\t{4}\t{5}", timeString, runs+1, blocks+1, trials+1, name, accuracy));
			trials++;
		}
//		gameOverText.text = "Time's up!";
		// play lower tone/beep
		timeoutSound.Play ();
		/* Reset all currently active bugs */
		int i = 0;
		while (activeHazards.Count > 0) {
			GameObject.Find (activeHazards [i]).SetActive (false);
			activeHazards.Remove (activeHazards [i]);
		}
		/* Reset example bug clone */
		objectPooler.ResetExample ();
		objectPooler.ClearMatchPool ();
		objectPooler.ClearBlockLurePool ();
	
		FeedbackText ("", Color.gray); // reset feedback text
		playerController2.ResetMat (); // reset bucket color
		playerController2.ResetPlayer (); // reset bucket location
		isFinishedLevel = true; // keeps timer from counting between blocks
		blockTimer = blockTimerCopy; // reset timer
		timerText.text = string.Format ("Time: 00:{0:00}", (int)(blockTimerCopy)); // reset timer text

		// if it's the end of a run, reset and prep for next run
		if (blocks == blocksPerRun) {
			blocks = 0;
			runs += 1;
			// reset score so speed starts off slower at beginning of new run
			score = 0;
			UpdateScore ();
		} else { // otherwise just increase blocks
			blocks += 1;
		}
		StartCoroutine ("SpawnWaves");	
	}
}
