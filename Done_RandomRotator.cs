using UnityEngine;
using System.Collections;

public class Done_RandomRotator : MonoBehaviour 
{
	public float tumble;
	
	void Start ()
	{
//		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;

		/* Random.insideUnitSphere returns -1 to 1, but numbers approaching 0 make the bugs
		 look like they're not spinning on one axis and can prevent studying all views of bug.
		 If values are too close to 0, reset them to -2/2 based on if the original number was -/+. */
		Vector3 tempvel = Random.insideUnitSphere * tumble;

		if ((tempvel.x > -0.09f) && (tempvel.x < 0.0f)) tempvel.x = -0.2f;
		if ((tempvel.x < 0.09f) && (tempvel.x > 0.0f)) tempvel.x = 0.2f;

		if ((tempvel.y > -0.09f) && (tempvel.y < 0.0f)) tempvel.y = -0.2f;
		if ((tempvel.y < 0.09f) && (tempvel.y > 0.0f)) tempvel.y = 0.2f;

		if ((tempvel.z > -0.09f) && (tempvel.z < 0.0f)) tempvel.z = -0.2f;
		if ((tempvel.z < 0.09f) && (tempvel.z > 0.0f)) tempvel.z = 0.2f;

		// then set angular velocity
		GetComponent<Rigidbody>().angularVelocity = tempvel;
	}
}