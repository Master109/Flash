using UnityEngine;
using System.Collections;

public class NoScreenShotZone : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.name == "Player")
			other.GetComponent<Player>().inNoScreenShotZone = true;
	}

	void OnTriggerExit (Collider other)
	{
		if (other.name == "Player")
			other.GetComponent<Player>().inNoScreenShotZone = false;
	}
}
