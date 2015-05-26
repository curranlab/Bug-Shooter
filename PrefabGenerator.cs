using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetDatabase {
	// creates menu option
	[MenuItem ("Assets/Create Bug Prefab")]

	static void CreateBugPrefab ()
	{
		// where prefabs should go
		string prefabPath = "Assets/Resources/bugs/";

		// for each model selected in the editor
		foreach (Object t in Selection.objects)
		{
			// cast it as a game object
			GameObject obj = t as GameObject;
			Debug.Log(t.name);
			// create empty prefab
			Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath + obj.name + ".prefab");
			// reset transform
			obj.GetComponent <Transform>().Translate (0,0,0);
			// connect model to empty prefab
			PrefabUtility.ReplacePrefab (obj, prefab, ReplacePrefabOptions.Default);
		}
		Debug.Log("Finished");
	}
}
