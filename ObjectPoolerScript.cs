using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ObjectPoolerScript : MonoBehaviour
{
	// list data structure to store all objects we'll use/need
	public List<string> activeHazards;
	public List<GameObject> hazards;
    public List<GameObject> matchPool;
	List<GameObject> bigLurePool;
	List<GameObject> blockLurePool;
	List<GameObject> examplePool;
	public GameObject example;

	private Done_GameController3 gameController3;

	private int subjNum;

	// ASSET MANAGER VARIABLES
	public static ObjectPoolerScript instance;
	public string[] bundleVariants;
	[SerializeField] string pathToBundles;
	Dictionary<string, AssetBundle> bundles;
	string platform;
	string path;

	
	void Awake()
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

		matchPool = new List<GameObject>();
		bigLurePool = new List<GameObject>();
		blockLurePool = new List<GameObject>();
		examplePool = new List<GameObject>();

		/* Asset bundler makes folders for each separate OS. I'm using these folders for now to
		 keep it clearer what version of assets are incldued with games and that they are OS specific. */
		#if UNITY_IOS
		platform = "iOS";
		#elif UNITY_ANDROID
		platform = "Android";
		#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		platform = "PC";
		path = @"C:\Unity\BugCatchingGame\stim_lists\";
		#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		platform = "OSX";
		path = "/Unity/BugCatchingGame/stim_lists/";
		#elif UNITY_WEBPLAYER
		platform = "Web";
		#else
		platform = "error";
		Debug.Log("unsupported platform");
		#endif

		pathToBundles += (platform + "/");

		/* Too many stims to make 1 bundle/family, so 1 bundle/species. Load all species for that family
		 based on subject number. */
		bundles = new Dictionary<string, AssetBundle> ();

		if (subjNum%2 == 0)
		{
			LoadBundle("sa");
			LoadBundle("sb");
			LoadBundle("sd");
			LoadBundle("si");
			LoadBundle("sj");
			LoadBundle("sl");
			LoadBundle("sm");
			LoadBundle("sn");
			LoadBundle("so");
			LoadBundle("sp");
		}
		else
		{
			LoadBundle("aa");
			LoadBundle("ab");
			LoadBundle("ac");
			LoadBundle("ad");
			LoadBundle("ae");
			LoadBundle("af");
			LoadBundle("ag");
			LoadBundle("ai");
			LoadBundle("aj");
			LoadBundle("ak");
		}
		
		while (!IsBundleLoaded("sp") && !IsBundleLoaded("ak")) {
			return;
		}
	}

	/* Use this for initialization of all possible stimuli in session */
	public void LoadHazards()
	{
		hazards = new List<GameObject>();
		string fileName = path + subjNum + ".txt"; // stim list is in same directory as asset bundles

		if (!File.Exists(fileName))
		{
			gameController3.FeedbackText("Stim list not found!", Color.red);
			Application.Quit();
		}

		using (StreamReader reader = new StreamReader(fileName))
		{
			string line;
			line = reader.ReadLine(); // skip the first line of the stim_list file (header)
			while ((line = reader.ReadLine()) != null)
			{
//				// for using Resources folder: 
//				string folder = "bugs/" + line;
//				GameObject go = (GameObject)Instantiate(Resources.Load(folder)) as GameObject;

				/* Figure out bundle we need to pull item from based on exemplar name, then retrieve that exemplar
				 from the appropriate asset bundle. Set properties by script. */
				string bundleName = line.Substring(0,2);
				GameObject go = (GameObject)Instantiate(GetAssetFromBundle(bundleName, line)) as GameObject;
				
				go.SetActive(false);
				// add rigidbody
				go.AddComponent <Rigidbody>();
				go.GetComponent <Rigidbody>().useGravity = false;
				go.GetComponent <Rigidbody>().angularDrag = 0.0f;
				
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

	/* Make list of all lures to be used in that session 
	 and a list of examples */
	public void MakeBigPools()
	{
		foreach (GameObject go in hazards)
		{
			bigLurePool.Add(go);
			bigLurePool.Add(go);
			bigLurePool.Add(go);
			bigLurePool.Add(go);
			bigLurePool.Add(go);
			bigLurePool.Add(go);
			bigLurePool.Add(go);
			examplePool.Add(go);
		}
	}
	
	/* Activate example bug */
	public GameObject CreateExample()
	{
		example = GetPooledObject();
		Vector3 exampleSpawnPosition = new Vector3 (0 , 0, 5);
		Quaternion exampleSpawnRotation = Quaternion.identity;
		example.GetComponent<Transform>().position = exampleSpawnPosition;
		example.GetComponent<Transform>().rotation = exampleSpawnRotation;
		example.GetComponent<Done_Mover>().speed = 0;
		example.GetComponent<Rigidbody>().velocity = transform.forward * 0;
		example.SetActive(true);
		return example;
	}

	/* Select an example bug from list of all hazards */
	public GameObject GetPooledObject ()
	{
		int idx = UnityEngine.Random.Range (0, examplePool.Count-1);
		GameObject tempGo = examplePool[idx];
		examplePool.Remove (tempGo);
		return tempGo;
	}

	/* Make pool of bugs that match the species of the example */
	public void MakeMatchPool()
	{
		for (int i = 0; i < hazards.Count; i++)
		{
			if (gameController3.SpeciesMatch(hazards[i]) && !hazards[i].name.Equals(example.name))
			{
				// add to pool that matches example species
				matchPool.Add(hazards[i]);
			}
		}
	}

	/* Make pool of 5 lures for 1 block at at time from big lure pool */
	public void MakeBlockLurePool ()
	{
		string speciesUsed = "";
		// only need 5 lures per block
		for (int i = 0; i < 5; i++)
		{
			// big pool has each bug repeated twice, not shuffled so draw randomly
			int idx = UnityEngine.Random.Range(0, bigLurePool.Count-1);
			// don't want it to match example species or already be a species used in this block
			while (gameController3.SpeciesMatch(bigLurePool[idx]) || speciesUsed.Contains(bigLurePool[idx].name.Substring(1,1)))
			{
				idx = UnityEngine.Random.Range(0, bigLurePool.Count-1);
			}

			// add to string of species used
			speciesUsed = speciesUsed + bigLurePool[idx].name.Substring(1,1);
			// add to block lure pool
			blockLurePool.Add(bigLurePool[idx]);
			// remove from big pool
			bigLurePool.Remove(bigLurePool[idx]);
		}
	}

	/* Select bugs from the matching/block lure pools for trials based on match */
	public GameObject GetPooledObjects (bool match)
	{
		if (match == true)// if this bug should match the example species
		{
			int idx = UnityEngine.Random.Range(0, matchPool.Count-1);

			GameObject tempGo = matchPool[idx];
			matchPool.Remove(tempGo);
			return tempGo;
		}
		else // otherwise it should be from the block lure pool
		{
			int idx = UnityEngine.Random.Range(0, blockLurePool.Count-1);

			GameObject tempGo = blockLurePool[idx];
			blockLurePool.Remove(tempGo);
			return tempGo;
		}
	}

	/* Reset example bug */
	public void ResetExample()
	{
		example.GetComponent<Done_Mover>().speed = -2; // reset speed variable, probably redundant w/line below
		example.GetComponent<Done_Mover>().tempVector = new Vector3 (0,0,-2); // reset directional speed
		example.GetComponent<Rigidbody>().velocity = example.GetComponent<Done_Mover>().tempVector; // update reset directional speed
		example.SetActive(false); // make bug inactive
	}

	/* Reset matching pool between blocks */
	public void ClearMatchPool()
	{
		matchPool.Clear();
	}

	/* Reset block lure pool between blocks */
	public void ClearBlockLurePool()
	{
		blockLurePool.Clear();
	}

	/****************************************************************************
	* 	Asset manager functions - loads in external assets from asset bundles
	* **************************************************************************/
	public bool IsBundleLoaded(string bundleName)
	{
		return bundles.ContainsKey (bundleName);
	}
	
	public UnityEngine.Object GetAssetFromBundle(string bundleName, string assetName)
	{
		if (!IsBundleLoaded (bundleName))
			return null;
		
		return bundles [bundleName].LoadAsset (assetName);
	}
	
	public bool LoadBundle(string bundleName)
	{
		//See if bundle is already loaded
		if (IsBundleLoaded(bundleName))
			return true;
		
		//If bundle isn't loaded, load it
		// was originally coroutine so could load while game continued, but that's
		// the opposite of what we want
		LoadBundleCoroutine(bundleName); 
		return false;
	}
	
	public void LoadBundleCoroutine(string bundleName)
	{
		string url = pathToBundles + bundleName;
		
		using(WWW www = WWW.LoadFromCacheOrDownload(url, 0))
		{
			if(!string.IsNullOrEmpty(www.error))
			{
				Debug.Log(www.error);
			}
			
			bundles.Add(bundleName, www.assetBundle);
		}
	}
	
	void OnDisable()
	{

		foreach(KeyValuePair<string, AssetBundle> entry in bundles)
			entry.Value.Unload(false);
		bundles.Clear ();
	}
}