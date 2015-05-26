using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Done_GameController3 : MonoBehaviour
{
	// variables we want to set in editor
	public Vector3 spawnValues;
	public int totalBlocks;
	public int trialsPerBlock;
//	public float blockWait;
//	public float trialWait;
//	public float penaltyWait;
	public float blockTimer; // set duration time in seconds in the Inspector
	
	// list data structure to store all objects we'll use/need
	public List<string> activeHazards;

//	[HideInInspector] public GameObject exampleBug;
	public GameObject exampleBug;

	// text variables
	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;
	public GUIText exampleText;
	public GUIText feedbackText;
	public GUIText timerText;
	public GUIText blockCounter;

	// don't set in inspector
	int blocks;
	[HideInInspector] public float pause;
	bool isMatch;
	bool isFinishedLevel; // while this is false, timer counts down
	private bool restart;
	private int score;
	private bool gameOver;
	private float blockTimerCopy;
	private Done_PlayerController2 playerController2;
	private ObjectPoolerScript objectPooler;
	private AudioSource timeoutSound;

	WaitForSeconds secondWFS = new WaitForSeconds(1);
	WaitForSeconds blockWFS = new WaitForSeconds(10); //blockWait);
	WaitForSeconds halfBlockWFS = new WaitForSeconds(5); //blockWait/2);
	
	/****************************************************************************
	* 	Start/on load of game
	* **************************************************************************/
	void Start ()
	{
		blocks = 0;
		restart = false;
		isFinishedLevel = true;
		feedbackText.text = "";
//		restartText.text = "";
		gameOverText.text = "";
		blockCounter.text = "Block:   ";
		blockTimerCopy = blockTimer;
		timerText.text = string.Format("Time: 00:{0:00}", (int)(blockTimer));
		exampleText.text = "Target Species";
		score = 0;
		UpdateScore ();

		// get access to player controller script
		GameObject objectPoolerObj = GameObject.FindGameObjectWithTag ("ObjectPooler");
		if (objectPoolerObj != null)
		{
			objectPooler = objectPoolerObj.GetComponent <ObjectPoolerScript>();
		}
		if (objectPoolerObj == null)
		{
			Debug.Log ("Cannot find 'ObjectPoolerScript' script");
		}
		// load objects
		objectPooler.LoadHazards();

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

		// get access to game object with incorrect sound
		GameObject timesoundObject = GameObject.FindGameObjectWithTag ("TimeoutSound");
		if (timesoundObject != null)
		{
			timeoutSound = timesoundObject.GetComponent <AudioSource>();
		}
		if (timeoutSound == null)
		{
			Debug.Log ("Cannot find 'timeoutSound' auodiosouce ");
		}

		StartCoroutine ("SpawnWaves");
	}
	/****************************************************************************
	* 	Update
	* **************************************************************************/
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			Application.Quit();
		}
		if (gameOver)
		{
			// create file

			// save player prefs to file

			// delete all player prefs
			PlayerPrefs.DeleteAll();
			// quit application
			Application.Quit();
		}
		if (restart)
		{
			if (Input.GetKeyDown (KeyCode.N))
			{
				Application.LoadLevel (Application.loadedLevel);
			}
		}
		/* Timer for blocks (w/in Update to keep track of state) */
		// only count down while we're in a block
		if (!isFinishedLevel) // has the level been completed
		{
			blockTimer -= Time.deltaTime; // I need timer which from a particular time goes to zero
			if (blockTimer < 0) blockTimer = 0;
			int seconds = (int)(blockTimer % 60);
			int minutes = (int)(blockTimer / 60);
			timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
		} 
		// if we haven't reached 0 yet, do nothing	
		if (blockTimer > 0) return; 
		// once the countdown timer reaches 0, run game over function & update timer text
		else
		{
			StopCoroutine("SpawnWaves");
			timerText.text = "Time: 00:00";
			TimeOut();
		}
	}

	/****************************************************************************
	 * 	Main experiment block/trial loop
	 * **************************************************************************/
	IEnumerator SpawnWaves ()
	{
		/* Wait a second before starting each block */
		yield return secondWFS;
		/* Set loop to run for however many blocks we want. Each block trains 1 species
		 for trialCount number of trials. */
		while (blocks < totalBlocks)
		{
			blockCounter.text = string.Format("Block: {0}", blocks+1);
			gameOverText.text = "";
			/* Reset pause */
			pause = 0.0f;
			/* Make new example bug for each block */
			exampleBug = objectPooler.CreateExample();
			/* Make pool of matching bugs based on exmaple bug species */
			objectPooler.MakeMatchPool();
			/* Wait long enough to view whole bug before starting block */
			yield return blockWFS;

			/* Start timer again */
			isFinishedLevel = false;

			/* For the total number of trials. */
			for (int i = 0; i < trialsPerBlock; i++)
			{
				/* Randomly generates # greater than or equal to 0 and less than 10. If random number is
				 in upper half of range assign false else assign true */
				isMatch = (Random.Range(0,10) < 5) ? true : false;

				/* Left bug always gets isMatch result (true/false) and Right bug always
				 gets opposite (false/true). Pass this assignment for GetPooledObject. */

				/* Both */
				Quaternion spawnRotation2 = Quaternion.identity;

				/* Left */
				GameObject hazard1 = objectPooler.GetPooledObjects(isMatch); 
				Vector3 spawnPosition2 = new Vector3 (-8 , spawnValues.y, spawnValues.z);
				hazard1.GetComponent<Transform>().position = spawnPosition2;
				hazard1.GetComponent<Transform>().rotation = spawnRotation2;
				/* Add prefab name to list that keeps track of active bugs */
				string name1 = hazard1.GetComponent<Transform>().name;
				activeHazards.Add(name1);
				hazard1.SetActive(true);

				/* Right */
				GameObject hazard2 = objectPooler.GetPooledObjects(!isMatch); 
				Vector3 spawnPosition3 = new Vector3 (8 , spawnValues.y, spawnValues.z);
				hazard2.GetComponent<Transform>().position = spawnPosition3;
				hazard2.GetComponent<Transform>().rotation = spawnRotation2;
				/* Add prefab name to list that keeps track of active bugs */
				string name2 = hazard2.GetComponent<Transform>().name;
				activeHazards.Add(name2);
				hazard2.SetActive(true);

				yield return new WaitForSeconds (TrialWait());
				/* Wait a little before spawning the next pair, depending on where rest of bugs are */
				while (pause > 1)
				{
					yield return secondWFS;
				}
			}
			/* Don't remove example until all active bugs have passed through game area */
			while (activeHazards.Count != 0)
			{
				yield return secondWFS;
			}

			/* Set as finished level to stop countdown, reset timer */
			isFinishedLevel = true;
			blockTimer = blockTimerCopy;
			timerText.text = string.Format("Time: 00:{0:00}", (int)(blockTimerCopy));

			/* Reset example bug clone and matchPool list */
			exampleBug.SetActive(false);
			exampleBug.GetComponent<Done_Mover>().speed = -2;
			objectPooler.ClearMatchPool();

			/* Wait briefly between blocks before making next example bug */
			yield return halfBlockWFS;
			blocks +=1;
		}
		gameOver = true;
	}

	/****************************************************************************
	 * 	Updating setters & helper functions
	 * **************************************************************************/
	public void FeedbackText (string acc, Color color)
	{
		feedbackText.text = acc;
		feedbackText.color = color;
	}
	
	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}
	
	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}

	/* Getter/setter for pause */
	public float GetPause ()
	{
		return pause;
	}
	public void SetPause (float newPause)
	{
		pause = newPause;
	}
	/* Helper function to determine if species are the same between 2 bugs */
	public bool SpeciesMatch(GameObject go)
	{
		// parse gameObject & example bug names to species level
		// name format = aa13(Clone)
		string species = go.name.Substring(0,2);
		string exampleSpecies = exampleBug.name.Substring(0,2);
		
		// if correct return true
		if (species.Equals(exampleSpecies)) return true;
		else
			return false;
	}

	/****************************************************************************
	 * 	Go faster as score increases
	 * **************************************************************************/
	public float TrialWait()
	{
		/* how long we want to wait between trials */
		float trialWait = 5;
		int totalPoints = totalBlocks * trialsPerBlock;
		/* trialWait set to 5 at beginning, so can only speed up 4 times,
		 evenly distributes speed up as points increase */
		if (score >= (totalPoints*0.8)) return trialWait-4;
		else if (score >= (totalPoints*0.6)) return trialWait-3;
		else if (score >= (totalPoints*0.4)) return trialWait-2;
		else if (score >= (totalPoints*0.2)) return trialWait-1;
		// if no speed up
		else return trialWait;
	}

	/****************************************************************************
	 * 	Block over
	 * **************************************************************************/
	public void TimeOut ()
	{
		gameOverText.text = "Time's up!";
		// play lower tone/beep
		timeoutSound.Play ();
		/* Reset all currently active bugs */
		int i = 0;
		while (activeHazards.Count > 0)
		{
			GameObject.Find(activeHazards[i]).SetActive(false);
			activeHazards.Remove(activeHazards[i]);
		}
		/* Reset example bug clone */
		exampleBug.SetActive(false);
		exampleBug.GetComponent<Done_Mover>().speed = -2;
		/* Reset feedback & timer */
		FeedbackText("", Color.gray); /* Reset feedback text */
		playerController2.ResetMat(); /* Reset bucket color */
		blocks +=1;
		isFinishedLevel = true;
		blockTimer = blockTimerCopy;
		timerText.text = string.Format("Time: 00:{0:00}", (int)(blockTimerCopy));
		StartCoroutine(BlockOver());
	}
	IEnumerator BlockOver ()
	{
		/* Wait briefly between blocks before making next example bug */
		yield return halfBlockWFS;
		/* Start main block coroutine over again */
		StartCoroutine ("SpawnWaves");
	}

}