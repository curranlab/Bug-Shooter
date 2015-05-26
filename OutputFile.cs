using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class OutputFile : MonoBehaviour {

	private int subjNum;
	private int sesNum;

	private string FILE_NAME;


	// Use this for initialization
	void Start () 
	{
		subjNum = PlayerPrefs.GetInt("subNum");
		
		sesNum = PlayerPrefs.GetInt("sesNum");

		FILE_NAME = subjNum + "_" + sesNum + "_data";

//		date = DateTime.Date;

		if (File.Exists(FILE_NAME))
		{
			// quit game??
			Debug.Log(FILE_NAME + " already exists!");
			return;
		}

		using (StreamWriter fs = File.CreateText(FILE_NAME))
		{
			fs.WriteLine("Date: " + DateTime.Now.Date);
			fs.WriteLine("Subject Number: " + subjNum);
			fs.WriteLine("Session Number: " + sesNum);
		}

	}
	public void WriteString (string message)
	{

	}
	public void CloseFile ()
	{
//		fs.Close();
	}
}
