// Porting the TileGenerator.js to c#
using UnityEngine;
using System.Collections;

public class TileGenerator : MonoBehaviour
{
	#region Fields

	public int numberOfTiles = 16;
	public RaycastHit hit;// This is used to get mouse click input the correct way.
	public GameObject matchOne;
	public GameObject matchTwo;

	public string tileName1;
	public string tileName2;

	public string[] tName1;
	public string[] tName2;

	public bool canClick = true;

	// Start and stop
	public Texture2D finishedTexture;
	public Texture2D timeUpTexture;
	public bool finished = false;
	public bool timeUp = false;


	// OnGui setup
	public int scoreInt = 0;
	public string scoreTxt;
	public GUISkin egyptSkin;


	// Timer
	private int startTime = 0; // Will not work unless initialized with a var....This is not inline with what is in the book....
	private int Seconds;
	private int roundedSeconds;
	private int txtSeconds;
	private int txtMinutes;
	public int countSeconds;
	private bool stopTimer = false;



	// var GameObject tileObjects : [];
	public GameObject[] tileObjects; // the [] means an array of that type

	// var tileLocations = new Array
	Vector3[] tileLocations = new Vector3[]
	{
		new Vector3 (0,0,0), new Vector3 (1.5f,0,0),
		new Vector3 (3,0,0), new Vector3 (4.5f,0,0),
		new Vector3 (0,1.5f,0), new Vector3 (1.5f,1.5f,0),
		new Vector3 (3,1.5f,0), new Vector3 (4.5f,1.5f,0),
		new Vector3 (0,3,0), new Vector3 (1.5f,3,0),
		new Vector3 (3,3,0), new Vector3 (4.5f,3,0),
		new Vector3 (0,4.5f,0), new Vector3 (1.5f,4.5f,0),
		new Vector3 (3,4.5f,0), new Vector3 (4.5f,4.5f,0)
	};


	#endregion


	#region Properties

	#endregion


	#region Functions


	// Runs before start does or as soon as the object the script is attached to is instantiated
	void Awake()
	{
		startTime = 60;
	}


	// Use this for initialization
	void Start()
	{
		Camera.main.transform.position = new Vector3(2.25f, 2.25f, -9.5f);
		for (var i = 0; i < numberOfTiles; i++)
		{
			Instantiate(tileObjects[i], tileLocations[i], Quaternion.identity);
		}
	}

	// Update is called once per frame
	void Update()
	{

		if (canClick == true)
		{

			if (Input.GetButtonDown("Fire1"))
			{

				var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray1, out hit, Mathf.Infinity)) // need to ad the keyword out before hit to get this to work.....
				{

					if (!matchOne)
					{
						revealCardOne();
					}
					else
					{
						revealCardTwo();
					}

				}


			}

		}

	}


	void OnGUI()
	{
		GUI.skin = egyptSkin;
		// FIXME: The rect height seems off........Also, GUI items are not staying where they should be in full screen play mode
		scoreTxt = scoreInt.ToString();
		GUI.Label(new Rect(400, 125, 100, 20), scoreTxt);
		// Timer code
		if (stopTimer == false)
		{
			var guiTime = Time.time - startTime;
			Seconds = countSeconds - (int)guiTime;
		}

		if (Seconds == 0)
		{
			print("The time is over");
			stopTimer = true;
			timeUp = true;

		}

		// Display Timer
		roundedSeconds = Mathf.CeilToInt(Seconds);
		txtSeconds = roundedSeconds % 60;
		txtMinutes = roundedSeconds / 60;

		var text = string.Format("{0:00}:{1:00}", txtMinutes, txtSeconds);// this works here?
		GUI.Label(new Rect(575, 125, 100, 30), text);

		// New screens
		if (finished == true) { GUI.Label(new Rect(270, 305, 512, 256), finishedTexture); }
		if (timeUp == true) { GUI.Label(new Rect(270, 305, 512, 256), timeUpTexture); }

	}

	void revealCardOne()
	{
		matchOne = hit.transform.gameObject;
		tileName1 = matchOne.transform.parent.name;
		print(tileName1);


		if (matchOne == null)
		{
			print("No object found!");
		}
		else
		{
			// Splits up the string at the _ and feeds it into an array.
			tName1 = tileName1.Split("_"[0]);
			// For debuging porpuses
			print(tName1[0]);
			//matchOne.transform.Rotate(Vector3(0,180,0));
			matchOne.transform.parent.animation.Play("tileReveal");

		}

	}

	// TODO: Find out what IEnumerable is and why it  may be needed here.
	//IEnumerable 
		void revealCardTwo()
	{
		matchTwo = hit.transform.gameObject;
		tileName2 = matchTwo.transform.parent.name;
		if (tileName1 != tileName2)
		{
			print(tileName2);
			if (matchTwo == null)
			{
				print("No object found!");
			}
			else
			{
				// Splits up the string at the _ and feeds it into an array.
				tName2 = tileName2.Split("_"[0]);
				// For debugging porpoises
				print(tName2[0]);
				matchTwo.transform.parent.animation.Play("tileReveal");
			}

			if (tName1[0] == tName2[0])
			{
				canClick = false;
				// yield return new WaitForSeconds (2); // FIXME: These change the type of method needed. Need to find out what is needed.
				Destroy(matchOne);
				Destroy(matchTwo);
				canClick = true;
				numberOfTiles = numberOfTiles - 2;

				if (numberOfTiles == 0)
				{
					print("End Game");
					finished = true;
					stopTimer = true;

				}

				scoreInt++;
			}
			else
			{
				canClick = false;
				// yield return new WaitForSeconds (2); // FIXME: These change the type of method needed. Need to find out what is needed.
				matchOne.transform.parent.animation.Play("tileHide");
				matchTwo.transform.parent.animation.Play("tileHide");

				canClick = true;
			}

			matchOne = null;
			matchTwo = null;
		}
	}




	#endregion
} 
