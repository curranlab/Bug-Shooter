using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class OutputFile : MonoBehaviour {

	private int subjNum;
	private int sesNum;
	string path;
	private string FILE_NAME;
	private Done_GameController3 gameController3;


	// Use this for initialization
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

		subjNum = PlayerPrefs.GetInt("subNum");
		
		sesNum = PlayerPrefs.GetInt("sesNum");

		#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		path = @"C:\Unity\BugCatchingGame\Data\";
		#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		path = "/Unity/BugCatchingGame/Data/";
		#else
		path = "error";
		Debug.Log("unsupported platform");
		#endif

		// make data folder if it doesn't exist
//		string path = Application.persistentDataPath + @"\data";
		Directory.CreateDirectory(path); //only creates directory if it doesn't exist

//		FILE_NAME = @"C:\Users\Display\Documents\Unity\Bug Shooter\data\" + subjNum + "_" + sesNum + "_trainingData.txt";
		//@ so it doesn't think "\" is an escape character
		FILE_NAME = path + subjNum + "_" + sesNum + "_trainingData.txt";

		if (File.Exists(FILE_NAME))
		{
			gameController3.FeedbackText(String.Format("{0} already exists!", FILE_NAME), Color.red);
			Application.Quit();	
		}
		else
		{
			using (StreamWriter fs = File.CreateText(FILE_NAME))
			{
				fs.WriteLine("Date " + DateTime.Now);
				fs.WriteLine(String.Format ("Subject {0}\tSession {1}", subjNum, sesNum));
//				fs.WriteLine("Session " + sesNum);
				fs.WriteLine ("Time\tRun\tBlock\tTrial\tType\tStim\tResp");
				fs.Close();
			}
		}
	}
	public void WriteLine (string message)
	{
		File.AppendAllText(FILE_NAME, message + Environment.NewLine);
	}
}