using UnityEngine;
using System.Collections;

public class End : MonoBehaviour
{
	
	// Use this for initialization
	void Start ()
	{
		name = name.Replace("(Clone)", "");
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.name == "Player")
		{
			if (PlayerPrefs.GetInt("Screenshots" + Application.loadedLevel, -1) == -1 || GameObject.Find("Player").GetComponent<Player>().screenShots < PlayerPrefs.GetInt("Screenshots" + Application.loadedLevel, -1))
				PlayerPrefs.SetInt("Screenshots" + Application.loadedLevel, GameObject.Find("Player").GetComponent<Player>().screenShots);
			if (Application.levelCount > Application.loadedLevel + 1)
				Application.LoadLevel(Application.loadedLevel + 1);
			else
				Application.LoadLevel(0);
		}
	}
}
