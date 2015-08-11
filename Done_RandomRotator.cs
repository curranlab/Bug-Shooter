using UnityEngine;
using System.Collections;

public class Done_RandomRotator : MonoBehaviour 
{
	public float tumble;
	Vector3 tempvel;
	
	void Start ()
	{
		/* Random.insideUnitSphere returns -1 to 1, but numbers approaching 0 make the bugs
		 look like they're not spinning on one axis and can prevent studying all views of bug.
		 If values are too close to 0, reset them to -2/2 based on if the original number was -/+. */
		tempvel = Random.insideUnitSphere * tumble;

		// while tempvel is too close to 0, keep randomly drawing from insideUnitSphere
		while(Mathf.Sqrt(tempvel.x*tempvel.x + tempvel.y*tempvel.y + tempvel.z*tempvel.z) < 0.5f)
		{
			tempvel = Random.insideUnitSphere * tumble;
		}

		// then set angular velocity
		GetComponent<Rigidbody>().angularVelocity = tempvel;
	}

	void Update ()
	{
		GetComponent<Rigidbody>().angularVelocity = tempvel;
	}
}