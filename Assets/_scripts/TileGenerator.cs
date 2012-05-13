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
	
	public ArrayList tName1;
	public ArrayList tName2;
	
	public bool canClick = true;

	// Start and stop
	public Texture2D finishedTexture;
	public Texture2D  timeUpTexture;
	public bool finished = false;
	public bool timeUp = false;
	
	
	// OnGui setup
	public int scoreInt = 0;
	public string scoreTxt;
	public GUISkin egyptSkin;
	

	// Timer
	private int startTime = 0; // Will not work unless initilized with a var....This is not inline with what is in the book....
	private int Seconds;
	private int roundedSeconds;
	private int txtSeconds;
	private int txtMinutes;
	public int countSeconds;
	private bool stopTimer = false;
	

	
	var GameObject tileObjects : [];
	
	
	var tileLocations = new Array
	(
		Vector3 (0,0,0), Vector3 (1.5,0,0),
		Vector3 (3,0,0), Vector3 (4.5,0,0),
		Vector3 (0,1.5,0), Vector3 (1.5,1.5,0),
		Vector3 (3,1.5,0), Vector3 (4.5,1.5,0),
		Vector3 (0,3,0), Vector3 (1.5,3,0),
		Vector3 (3,3,0), Vector3 (4.5,3,0),
		Vector3 (0,4.5,0), Vector3 (1.5,4.5,0),
		Vector3 (3,4.5,0), Vector3 (4.5,4.5,0)
	);
	
	
	#endregion


	#region Properties

	#endregion


	#region Functions
	

	// Runs before start does or as soon as the object the script is attached to is instanciated
	void Awake ()
	{
	startTime = 60;
	}


	// Use this for initialization
	void Start()
	{
		Camera.main.transform.position = Vector3(2.25,2.25,-9.5);
		for ( var i=0; i < numberOfTiles; i++)
		{
			Instantiate (tileObjects[i], tileLocations[i],Quaternion.identity);
		}
	}

	// Update is called once per frame
	void Update()
	{
	
		if (canClick == true){
	
		if (Input.GetButtonDown("Fire1"))
		{
		
			var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
		
			if (Physics.Raycast (ray1, hit, Mathf.Infinity))
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
	// FIXME: The rect heigt seems off........Also, GUI items are not staying where they should be in full screen play mode
	scoreTxt = scoreInt.ToString();
	GUI.Label(Rect(400,125,100,20),scoreTxt);
	// Timer code
	if(stopTimer == false){
		var guiTime = Time.time - startTime;
		Seconds = countSeconds - (guiTime);
	}
	
	if (Seconds == 0){
		print("The time is over");
		stopTimer = true;
		timeUp = true;
		
	}
	
	// Display Timer
	roundedSeconds = Mathf.CeilToInt(Seconds);
	txtSeconds = roundedSeconds % 60;
	txtMinutes = roundedSeconds / 60;
	
	var text = String.Format("{0:00}:{1:00}", txtMinutes, txtSeconds);
	GUI.Label(Rect(575,125,100,30), text);
	
	// New screens
	if (finished == true){
		GUI.Label(Rect(270,305,512,256), finishedTexture);
	}
	
	if (timeUp == true){
		GUI.Label(Rect(270,305,512,256), timeUpTexture);
	}
	
}


	void revealCardOne(){
	matchOne = hit.transform.gameObject;
	tileName1 = matchOne.transform.parent.name;
	print(tileName1);
	
	
	if(matchOne == null)
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

	void revealCardTwo(){
	matchTwo = hit.transform.gameObject;
	tileName2 = matchTwo.transform.parent.name;
	if (tileName1 != tileName2){
		print(tileName2);
		if(matchTwo == null)
		{
			print("No object found!");
		}
		else
		{
			// Splits up the string at the _ and feeds it into an array.
			tName2 = tileName2.Split("_"[0]);
			// For debuging porpuses
			print(tName2[0]);
			//matchTwo.transform.Rotate(Vector3(0,180,0));	
			matchTwo.transform.parent.animation.Play("tileReveal");
		}
	
		if (tName1[0] == tName2[0] )
		{
			canClick = false;
			yield new WaitForSeconds (2);
			Destroy (matchOne);
			Destroy (matchTwo);
			canClick = true;
			numberOfTiles = numberOfTiles-2;
			
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
			yield new WaitForSeconds (2);
			//matchOne.transform.Rotate(Vector3(0,180,0));
			//matchTwo.transform.Rotate(Vector3(0,180,0));
			matchOne.transform.parent.animation.Play("tileHide");
			matchTwo.transform.parent.animation.Play("tileHide");

			canClick = true;
		}
	
		matchOne = null;
		matchTwo = null;

	}




	
}
	#endregion