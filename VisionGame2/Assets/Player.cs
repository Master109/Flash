using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	ArrayList gos;
	Renderer[] renderers;
	public int screenShots;
	public Material red;
	public Material majenta;
	public Material noCopy;
	public float pushPower = 2f;
	ArrayList pParents;
	bool showLookingAtRedNotification;
	public GUISkin guiSkin;
	public GUISkin guiSkin2;
	public bool[] showTip;
	public string[] tips;
	bool haveScreenShot;
	public bool onDevice;
	CharacterMotor motor;
	bool takeScreenShot;
	public int gridZoomRate;
	public int gridZoomMax;
	public bool inNoScreenShotZone;
	bool showInFogNotification;
	ArrayList alphas = new ArrayList();
	public Shader shader;

	// Use this for initialization
	void Start ()
	{
		motor = GetComponent<CharacterMotor>();
		gos = new ArrayList();
		pParents = new ArrayList();
		if (Application.loadedLevel == 0)
		{
			showTip[0] = true;
			showTip[1] = true;
			showTip[2] = true;
			showTip[3] = true;
			showTip[4] = true;
		}
		else if (Application.loadedLevel == 1)
		{
			showTip[5] = true;
			showTip[6] = true;
		}
		else if (Application.loadedLevel == 6)
		{
			showTip[7] = true;
		}
		else if (Application.loadedLevel == 9)
		{
			showTip[8] = true;
		}
		Input.multiTouchEnabled = true;
	}

	// Update is called once per frame
	void Update ()
	{
		if (GameObject.Find("Scripts").GetComponent<Level>().paused)
			return;
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Background"))
		{
			if (g.renderer.material.mainTextureScale.x < gridZoomMax)
				g.renderer.material.mainTextureScale += Vector2.one * gridZoomRate;
			else
				g.renderer.enabled = false;
		}
		if (Input.touchCount > 0)
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - Input.touches[0].deltaPosition.y, transform.rotation.eulerAngles.y + Input.touches[0].deltaPosition.x, 0);
		if (Input.GetKeyUp(KeyCode.R))
			Application.LoadLevel(Application.loadedLevel);
		if (Input.GetKeyDown(KeyCode.LeftShift) || takeScreenShot)
		{
			takeScreenShot = false;
			if (!haveScreenShot)
			{
				renderers = (Renderer[])(FindObjectsOfType(typeof(Renderer)));
				foreach (Renderer r in renderers)
					if ((r.isVisible && r.material.mainTexture == red.mainTexture) || inNoScreenShotZone)
					{
						if (inNoScreenShotZone)
							StartCoroutine("InFogNotification");
						else
							StartCoroutine("LookingAtRedNotification");
						return;
					}
				foreach (Renderer r in renderers)
				{
					if (r.isVisible && r.material.mainTexture != noCopy.mainTexture && r.gameObject.GetComponent<ExcludeFromVision>() == null)
					{
						GameObject g = (GameObject) GameObject.Instantiate(r.gameObject);
						g.collider.enabled = false;
						alphas.Add(g.renderer.material.color.a);
						g.AddComponent<SetAlpha>();
						g.renderer.material.shader = Shader.Find("Transparent/Diffuse");
						g.renderer.material.color = new Color(g.renderer.material.color.r, g.renderer.material.color.g, g.renderer.material.color.b, g.renderer.material.color.a * .25f);
						if (r.transform.parent != null)
							pParents.Add(r.transform.parent.transform);
						else
							pParents.Add(null);
						g.transform.parent = Camera.main.transform;
						if (g.rigidbody != null)
							g.rigidbody.isKinematic = true;
						gos.Add(g);
					}
				}
			}
			else
			{
				foreach (Renderer r in renderers)
					if ((r.isVisible && r.material.mainTexture == red.mainTexture) || inNoScreenShotZone)
					{
						foreach (GameObject g in gos)
							Destroy(g);
						gos.Clear();
					haveScreenShot = false;
						if (inNoScreenShotZone)
							StartCoroutine("InFogNotification");
						else
							StartCoroutine("LookingAtRedNotification");
						return;
					}
				ArrayList gos2 = new ArrayList();
				foreach (Renderer r in renderers)
					if (r.isVisible && r.GetComponent<ExcludeFromVision>() == null)
						gos2.Add(r.gameObject);
				foreach (GameObject g in gos2)
					if (g.renderer.material.mainTexture != majenta.mainTexture)
						Destroy(g);
				for (int i = 0; i < gos.Count; i ++)
				{
					GameObject g = (GameObject) gos[i];
					g.collider.enabled = true;
					g.renderer.enabled = true;
					Transform pParent = (Transform) pParents[i];
					g.transform.parent = pParent;
					if (g.rigidbody != null)
						g.rigidbody.isKinematic = false;
					Debug.Log((float) alphas[i]);
					if ((float) alphas[i] == .25f)
						g.renderer.material.color = new Color(g.renderer.material.color.r, g.renderer.material.color.g, g.renderer.material.color.b, .25f);
					else
						g.renderer.material.color = new Color(g.renderer.material.color.r, g.renderer.material.color.g, g.renderer.material.color.b, 1);
				}
				gos.Clear();
				pParents.Clear();
				alphas.Clear();
				screenShots ++;
				showTip[5] = false;
			}
			haveScreenShot = !haveScreenShot;
		}
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		Rigidbody body = hit.collider.attachedRigidbody;
		
		// no rigidbody
		if (body == null || body.isKinematic) { return; }
		
		// We dont want to push objects below us
		if (hit.moveDirection.y < -0.3) {
						return;
				}
		// Calculate push direction from move direction,
		// we only push objects to the sides never up and down
		Vector3 pushDir = new Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);
		
		// If you know how fast your character is trying to move,
		// then you can also multiply the push velocity by that.
		
		// Apply the push
		body.velocity += pushDir * pushPower;
		showTip[3] = false;
	}

	void OnGUI ()
	{
		if (GameObject.Find("Scripts").GetComponent<Level>().paused)
			return;
		GUI.skin = guiSkin2;
		if ((!haveScreenShot && GUI.Button(new Rect(Screen.width - 200, Screen.height - 50, 200, 50), "Take screenshot")) || (haveScreenShot && GUI.Button(new Rect(Screen.width - 200, Screen.height - 50, 200, 50), "Paste screenshot")))
			takeScreenShot = true;
		GUI.Label(new Rect(0, 50, 9999999999, 50), "Screenshots used (infinite allowed): " + screenShots);
		if (GUI.Button(new Rect(0, Screen.height - 50, 200, 50), "Erase Saved Data"))
			PlayerPrefs.DeleteAll();
		if (PlayerPrefs.GetInt("Screenshots" + Application.loadedLevel, -1) == -1)
			GUI.Label(new Rect(0, 25, 9999999999, 50), "Least screenshots used to complete level: Not completed");
		else
			GUI.Label(new Rect(0, 25, 9999999999, 50), "Least screenshots used to complete level: " + PlayerPrefs.GetInt("Screenshots" + Application.loadedLevel, -1));
		GUI.skin = guiSkin;
		if (GameObject.Find("End") == null)
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 + 12, 750, 99999), "You have destroyed the door you need to beat the level. R to restart.");
			return;
		}
		if (showLookingAtRedNotification)
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 + 12, 750, 99999), "You cannot save or load a screenshot while viewing dark matter (the blue stuff)");
		if (showInFogNotification)
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 + 12, 750, 99999), "You cannot save or load a screenshot while in fog");
		if (showTip[0])// || showTip[1])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[0]);
			if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
				showTip[0] = false;
			if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
				showTip[1] = false;
		}
		else if (showTip[2])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[1]);
			if (Input.GetAxis("Jump") != 0)
				showTip[2] = false;
		}
		else if (showTip[3])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[2]);
		}
		else if (showTip[4])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[3]);
		}
		else if (showTip[5])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[4].Replace("Ω", "\n"));
		}
		else if (showTip[6])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[5]);
		}
		else if (showTip[7])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[6]);
		}
		else if (showTip[8])
		{
			GUI.Label(new Rect(Screen.width / 8, Screen.height / 2 - 12, 750, 99999), tips[7]);
		}
		GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
		for (int i = 1; i <= Application.levelCount; i ++)
			if (GUILayout.Button("Level " + i, GUILayout.Height(50)))
		{
			Application.LoadLevel(i - 1);
		}
		GUILayout.EndArea();
	}

	IEnumerator LookingAtRedNotification ()
	{
		showLookingAtRedNotification = true;
		yield return new WaitForSeconds(5);
		showLookingAtRedNotification = false;
	}

	IEnumerator InFogNotification ()
	{
		showInFogNotification = true;
		yield return new WaitForSeconds(5);
		showInFogNotification = false;
	}
}
