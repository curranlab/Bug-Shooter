using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LoadOnClick : MonoBehaviour {

	public GameObject loadingImage;
	public GameObject confirmImage;

	[HideInInspector] public GameObject IFGo;
	[HideInInspector] public GameObject IFGo2;

	private string subjStr;
	private string sesNumStr;
	[HideInInspector] public int subjNumber;
	[HideInInspector] public int sesNumber;

	public Text confirmText;

	public void LoadScene(int choice)
	{
		switch (choice)
		{
			// if they pressed start button
			case 1:
			{
				/* Reset player prefs if any carryover from previous session?
				 Could be useful for game to remember subjects for what sessions were competed, but
				 bad if using multiple computers. */
				if (PlayerPrefs.GetInt("subNum") > 0) PlayerPrefs.DeleteAll();
				
				/* SET SUBJCECT NUMBER */
				// get input field game object to get it's text input
				IFGo = GameObject.Find("Canvas/SubjectNumber/SubjNumText");
				subjStr = IFGo.GetComponent<Text>().text;
				// parse input string into an int
				int.TryParse(subjStr, out subjNumber);
				PlayerPrefs.SetInt("subNum", subjNumber);
				
				/* SET SESSION NUMBER */
				// get input field game object to get it's text input
				IFGo2 = GameObject.Find("Canvas/SessionNumber/SesNumText");
				sesNumStr = IFGo2.GetComponent<Text>().text;
				// parse input string into an int
				int.TryParse(sesNumStr, out sesNumber);
				PlayerPrefs.SetInt("sesNum", sesNumber);
				
				string date = "" + DateTime.Now;
				PlayerPrefs.SetString("Date", date);

				// set confirm text to have subj & ses numbers
				confirmText.text = String.Format("Are you sure you want to run subject {0} session {1}?", subjNumber, sesNumber);
				
				// confirm window set active true
				confirmImage.SetActive(true);
				break;
			}

			// if they choose to continue
			case 2:
			{
				// activate loading screen
				loadingImage.SetActive(true);
				// load game
				Application.LoadLevel(1);
				break;
			}


			// if they choose to quit
			case 3:
			{
				PlayerPrefs.DeleteAll();
				Application.Quit();
				break;
			}

		}
			



	}
}
