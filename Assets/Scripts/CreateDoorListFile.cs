using UnityEngine;
using System.Collections;

public class CreateDoorListFile : MonoBehaviour {

	private GameObject[] doors;
	private string namelist;
	// Use this for initialization
	void Start () {

		doors = GameObject.FindGameObjectsWithTag("NamedDoor");

		for(int i = 0; i < doors.Length; i++)
		{
			namelist +=doors[i].name;
			namelist +="\n";
			//Debug.Log(doors[i].name);

		

		}

		System.IO.File.WriteAllText("C:/Users/David/Desktop/Unity_Output/doorslist.txt", namelist);
		

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
