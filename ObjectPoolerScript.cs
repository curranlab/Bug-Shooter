using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ObjectPoolerScript : MonoBehaviour
{
	// object pooler variables
	public int pooledAmount;
	
	// list data structure to store all objects we'll use/need
	public List<string> activeHazards;
	[HideInInspector] public List<GameObject> hazards;
	[HideInInspector] public List<GameObject> matchPool;

	[HideInInspector] public GameObject example;

	private Done_GameController3 gameController3;

	private int subjNum;
	private int sesNum;
	
	void Start()
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
	}

	/* Use this for initialization of all possible stimuli in session */
	public void LoadHazards()
	{
		hazards = new List<GameObject>();
		string fileName = "Assets/Resources/stim_lists/" + subjNum + "_" + sesNum + ".txt";

		if (!File.Exists(fileName))
		{
//			Application.Quit();
		}

		using (StreamReader reader = new StreamReader(fileName))
		{
			string line;
			line = reader.ReadLine();
			while ((line = reader.ReadLine()) != null)
			{
				string folder = "bugs/" + line;
				GameObject go = (GameObject)Instantiate(Resources.Load(folder)) as GameObject;

				go.SetActive(false);
				// add rigidbody
				go.AddComponent <Rigidbody>();
				go.GetComponent <Rigidbody>().useGravity = false;
				
				// add scripts
				go.AddComponent <Done_RandomRotator>();
				go.GetComponent <Done_RandomRotator>().tumble = 1;
				go.AddComponent <Done_DestroyByContact>();
				go.GetComponent <Done_DestroyByContact>().scoreValue = 1;
				go.AddComponent <Done_Mover>();
				go.GetComponent <Done_Mover>().speed = -2;
				go.tag = "Enemy";
				Destroy(go.GetComponent<Animator>());
				
				// add capsule collider
				go.AddComponent <CapsuleCollider>();
				go.GetComponent <CapsuleCollider>().radius = 1.5f;
				go.GetComponent <CapsuleCollider>().height = 6;
				hazards.Add(go);
			}
		}
	}

	/* Activate example bug */
	public GameObject CreateExample()
	{
		example = GetPooledObject();
		Vector3 spawnPosition1 = new Vector3 (0 , 0, 5);
		Quaternion spawnRotation = Quaternion.identity;
		example.GetComponent<Transform>().position = spawnPosition1;
		example.GetComponent<Transform>().rotation = spawnRotation;
		example.GetComponent<Done_Mover>().speed = 0;
		example.GetComponent<Rigidbody>().velocity = transform.forward * 0;
		example.SetActive(true);
		return example;
	}

	/* Select an example bug from list of all hazards */
	public GameObject GetPooledObject ()
	{
		int idx = Random.Range(0, hazards.Count);
		if(hazards[idx] == null)
		{
			GameObject obj = (GameObject)Instantiate(hazards[idx]);
			obj.SetActive (false);
			hazards[idx] = obj;
			return hazards[idx];
		}
		if(!hazards[idx].activeInHierarchy)
		{
			return hazards[idx];
		}
		// If selected bug is already active or is example species, return next bug
		if(hazards[idx].activeInHierarchy)
		{
			return GetPooledObject();
		}
		// if nothing to instantiate
		return null;
	}

	/* Make pool of bugs that match the species of the example */
	public void MakeMatchPool()
	{
		matchPool = new List<GameObject>();
		foreach (GameObject go in hazards)
		{
			if (gameController3.SpeciesMatch(go))
			{
				matchPool.Add(go);
			}
		}
	}

	/* Select bugs from the matching/regular pools for trials based on match */
	public GameObject GetPooledObjects (bool match)
	{
		if (match == true)// if this bug should match the example species
		{
			int idx = Random.Range(0, matchPool.Count);
			if(matchPool[idx] == null)
			{
				GameObject obj = (GameObject)Instantiate(matchPool[idx]);
				obj.SetActive (false);
				matchPool[idx] = obj;
				return matchPool[idx];
			}
			if(!matchPool[idx].activeInHierarchy)
			{
				return matchPool[idx];
			}
			/* If selected bug is already active, return next bug */
			if(matchPool[idx].activeInHierarchy)
			{
				return GetPooledObjects(true);
			}
			// if nothing to instantiate
			return null;
		}
		else // otherwise it should be any other species (including other family)
		{
			int idx = Random.Range(0, hazards.Count);
			if(hazards[idx] == null)
			{
				GameObject obj = (GameObject)Instantiate(hazards[idx]);
				obj.SetActive (false);
				hazards[idx] = obj;
				return hazards[idx];
			}
			if(!hazards[idx].activeInHierarchy && !gameController3.SpeciesMatch(hazards[idx]))
			{
				return hazards[idx];
			}
			// If selected bug is already active or is example species, return next bug
			if(hazards[idx].activeInHierarchy || (gameController3.SpeciesMatch(hazards[idx])))
			{
				return GetPooledObjects(false);
			}
			// if nothing to instantiate
			return null;
		}
	}

	/* Reset matching pool between blocks */
	public void ClearMatchPool()
	{
		matchPool.Clear();
	}
//	void OnGUI() 
//	{
//		GUI.Box(new Rect(0,0,Screen.width/2,Screen.height/2), "Stim list doesn't exist!");
//	}
}

//public class ObjectPoolerScript : MonoBehaviour 
//{
//	public static ObjectPoolerScript current;
//	public GameObject pooledObject;
//	public int pooledAmount = 20;
//	public bool willGrow = true;
//
//	List<GameObject> pooledObjects;
//
//	void Awake()
//	{
//		current = this;
//	}
//	// Use this for initialization
//	void Start() 
//	{
//		pooledObjects = new List<GameObject>();
//		for (int i = 0; i < pooledAmount; i++)
//		{
//			GameObject obj = (GameObject)Instantiate(pooledObject);
//			obj.SetActive(false);
//			pooledObjects.Add(obj);
//		}
//	}
//	
//	public GameObject GetPooledObject ()
//	{
//		// pass pointer to object, and keep it in the list
//		for(int i = 0; i < pooledObjects.Count; i++)
//		{
//			if(pooledObjects[i] == null)
//			{
//				GameObject obj = (GameObject)Instantiate(pooledObject);
//				obj.SetActive (false);
//				pooledObjects[i] = obj;
//				return pooledObjects[i];
//			}
//
//			if(!pooledObjects[i].activeInHierarchy)
//			{
//				return pooledObjects[i];
//			}
//		}
//
//		// if not enough items in list
//		if(willGrow)
//		{
//			GameObject obj = (GameObject)Instantiate(pooledObject);
//			pooledObjects.Add(obj);
//			return obj;
//		}
//
//		// if nothing to instantiate
//		return null;
//	}
//}