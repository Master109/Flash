using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour
{
	public int parScreenshots;
	public string quote;
	public bool paused;
	
	// Use this for initialization
	void Start ()
	{
		Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnGUI ()
	{
		GUI.skin = GameObject.Find("Player").GetComponent<Player>().guiSkin2;
		if (paused)
		{
			//GUI.Label(new Rect(0, 0, 9999999999, 9999999999), quote.Replace("Ω", "\n"));
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2, 9999999999, 50), "Press any key");

			if (Input.anyKeyDown)
			{
				paused = false;
				Time.timeScale = 1;
			}
			return;
		}
		GUI.Label(new Rect(0, 0, 9999999999, 50), "Par screenshot number: " + parScreenshots);
	}
}
