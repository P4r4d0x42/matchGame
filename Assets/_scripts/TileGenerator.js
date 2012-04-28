#pragma strict


var numberOfTiles = 16;
var hit : RaycastHit; // This is used to get mouse click input the correct way.
var matchOne : GameObject;
var matchTwo : GameObject;
var tileName1 : String;
var tileName2 : String;
var tName1 : Array;
var tName2 : Array;
var canClick = true;


var tileObjects : GameObject[];


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





function Start () {
	Camera.main.transform.position = Vector3(2.25,2.25,-8);
	for ( var i=0; i < numberOfTiles; i++){
	Instantiate (tileObjects[i], tileLocations[i],Quaternion.identity);
	}
	

}

function Update () {
	
	if (canClick == true){
	
		if (Input.GetButtonDown("Fire1")){
			print ("Mouse clicked");
		
			var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
		
			if (Physics.Raycast (ray1, hit, Mathf.Infinity)){
			
				if (!matchOne)
				{
					revealCardOne();
				}
				else{
					revealCardTwo();
				}
			
			}
	
		}	
	}
}




function revealCardOne(){
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

function revealCardTwo(){
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
			}
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


