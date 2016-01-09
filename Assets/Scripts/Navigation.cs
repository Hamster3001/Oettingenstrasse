using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class Navigation : MonoBehaviour {

	private GameObject[] doors;
	private GameObject player;
	public string targetstring;
	private GameObject[] horizontalwaypoints;
	private GameObject[] verticalwaypoints;
	private GameObject[] allwaypoints;
	private GameObject[] allwaypoints2;
	Transform[] donepoints;
	private Transform target;
	private Transform[] path;
	private int while_counter = 0;
	private Transform startpoint;
	private Transform endpoint;
	private bool rotateonce;


	void Start () {

		doors = GameObject.FindGameObjectsWithTag("NamedDoor");
		horizontalwaypoints = GameObject.FindGameObjectsWithTag("HorizontalWaypoint");
		verticalwaypoints = GameObject.FindGameObjectsWithTag("VerticalWaypoint");
		allwaypoints = new GameObject[horizontalwaypoints.Length+verticalwaypoints.Length];
		allwaypoints2 = new GameObject[horizontalwaypoints.Length+verticalwaypoints.Length];
		horizontalwaypoints.CopyTo (allwaypoints, 0);
		verticalwaypoints.CopyTo (allwaypoints, horizontalwaypoints.Length);
		horizontalwaypoints.CopyTo (allwaypoints2, 0);
		verticalwaypoints.CopyTo (allwaypoints2, horizontalwaypoints.Length);

		player = GameObject.FindGameObjectWithTag ("Player");

		if (targetstring == "") {
		
			targetstring = "B001";
		
		}

		startPath ();
	
	}
	
    Transform[] getStartPoints(Transform source,Transform target){

		GameObject[] twoNN = getNearestNeighbors (source, 2,allwaypoints2);
		Transform startObject = getNearestNeighbor(target, twoNN);
		ArrayUtility.Remove(ref twoNN,startObject.gameObject);
		Transform[] result = new Transform [2];
		result [0] = startObject;
		result [1] = twoNN [0].transform;
		return result;

   }
	

	void startPath(){

		hideAllWaypoints ();
		target = findTargetDoor (targetstring);
		endpoint = getNearestNeighbor (target, allwaypoints2);
		Transform[] startpoints = getStartPoints (player.transform,target);
		donepoints = new Transform[0];
		startpoint = startpoints [0];

//		Debug.Log ("END: "+endpoint);
//		Debug.Log ("START: "+startpoint);


		path = buildPath(startpoint, endpoint);

		if (path!=null) {
//			Debug.Log("PATHLENGTH" +path.Length);
			foreach (Transform p in path) {

				activateWaypoint (p);
				Waypoint waypointscript = p.GetComponent<Waypoint>();
				if(p==startpoint)waypointscript.startpoint=true;
				if(p==endpoint)waypointscript.endpoint=true;
			}

			rotateStartAndEndpoint();
			InvokeRepeating("rotateWaypoints", 0, 0.5f);


		} else
			Debug.Log ("No Path found");

	}


	void rotateStartAndEndpoint(){

	if (startpoint.gameObject.tag == "HorizontalWaypoint") {
			if (startpoint.transform.position.x < player.transform.position.x) {
				startpoint.transform.GetChild (0).transform.eulerAngles = new Vector3 (90f, 270f, 0f);
			}

			if (endpoint.transform.position.x < player.transform.position.x) {
				endpoint.transform.GetChild (0).transform.eulerAngles = new Vector3 (90f, 270f, 0f);
			}

	 }	

		if (startpoint.gameObject.tag == "VerticalWaypoint") {

			if (startpoint.transform.position.z < player.transform.position.z) {
				startpoint.transform.GetChild (0).transform.eulerAngles = new Vector3 (90f, -180f, 0f);
			}

			if (endpoint.transform.position.z < player.transform.position.z) {
				Debug.Log ("IF");
				endpoint.transform.GetChild (0).transform.eulerAngles = new Vector3 (90f, -180f, 0f);
			}


		}	  
	
		//startpoint.transform.GetChild(0).transform.rotation.eulerAngles.x !=90;

	}


	Transform[] buildPath(Transform startpoint, Transform endpoint){

		ArrayList resultpathes = new ArrayList ();
		Transform[] currentpoints = new Transform[0];

		ArrayUtility.Add (ref currentpoints, startpoint);
		Transform[] resultpath = new Transform[0];
		ArrayUtility.Add (ref resultpath, startpoint);
		resultpathes.Add (resultpath);
		bool breaknew = false;

		while (true) {				

			while_counter++;
			if(while_counter>10000)break;

			foreach (Transform p in currentpoints){

				Transform[]nextpoints = getOneStepPoints(p);

				bool first = true;
				int idx = ArrayUtility.IndexOf(currentpoints,p);

				if(nextpoints==null ||nextpoints.Length==0){

					ArrayUtility.Remove(ref currentpoints,p);
					resultpathes.Remove(resultpathes[idx]);
					continue;
				}

				Transform[] currentresultpath = null;

				foreach (Transform np in nextpoints){

					if(first){


						currentresultpath = (Transform[])resultpathes[idx];
						currentpoints[idx] = np;
						ArrayUtility.Add (ref currentresultpath,np);
						resultpathes[idx] = currentresultpath;
						ArrayUtility.Remove (ref currentresultpath,np);
						first = false;
					}

					else{

						ArrayUtility.Add (ref currentpoints,np);
						Transform[] newresultpath = (Transform[]) currentresultpath.Clone();	
						ArrayUtility.Add (ref newresultpath,np);
						resultpathes.Add(newresultpath);

					}

					if(np==endpoint){
						ArrayUtility.Add(ref currentresultpath,np);		
						return currentresultpath;
					}


				}
				if(!donepoints.Contains(p)){
				ArrayUtility.Add(ref donepoints,p);
				}
			}

		}
		return null;
	}

	
	Transform[]getOneStepPoints(Transform p){

		Transform[] result = new Transform[0];
		string name = p.gameObject.name;

		if (name.Contains ("split")) {
		GameObject[] tempresult = getNearestNeighbors (p, 3, allwaypoints2);

			foreach(GameObject g in tempresult){

				ArrayUtility.Add(ref result,g.transform);
	        }

		} else if (name.Contains ("fourway")) {

			GameObject[] tempresult = getNearestNeighbors (p, 4, allwaypoints2);
			
			foreach(GameObject g in tempresult){
				
				ArrayUtility.Add(ref result,g.transform);
				
			}
		} else {

			GameObject[] tempresult = getNearestNeighbors (p, 2, allwaypoints2);
			
			foreach(GameObject g in tempresult){
				
				ArrayUtility.Add(ref result,g.transform);
				
			}
		}

		if (donepoints.Length > 0) {
		
		foreach(Transform pt in donepoints){
			
			foreach(Transform r in result){

				if(donepoints.Contains(r)){

					ArrayUtility.Remove(ref result,r);

				}
			}
			
		}

	}


		return result;
	}

	

	Transform getNearestNeighbor (Transform source, GameObject[]allwaypoints)
	{
		Transform bestTarget = null;
		float closestDist = Mathf.Infinity;
		Vector3 currentPosition = source.position;
		foreach(GameObject potentialTarget in allwaypoints)
		{
			//float dist =  Vector3.Distance(potentialTarget.transform.position,currentPosition);
			Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
			directionToTarget.y =0.001f;
			float dist = directionToTarget.sqrMagnitude;
			if(dist < closestDist)
			{
				closestDist = dist;
				bestTarget = potentialTarget.transform;
			}
		}
	
		return bestTarget;
	}




    GameObject[] getNearestNeighbors (Transform source, int number, GameObject[]waypoints)
	{	
	
	
		GameObject[] result = new GameObject[number];
	    Vector3 currentPosition = source.position;
				var distanceToObject = new Dictionary <float, GameObject>(waypoints.Length);  
	
				foreach(GameObject waypoint in waypoints)
		{
			//float dist =  Vector3.Distance(waypoint.transform.position,currentPosition);
		
			Vector3 directionToTarget = waypoint.transform.position - currentPosition;
			directionToTarget.y =0.001f;
			float dist = directionToTarget.sqrMagnitude;

			if(distanceToObject.ContainsKey(dist)){

				float repairvalue = Random.Range(0.01F, 0.02F);
				dist += repairvalue;
			
			}
		
			distanceToObject.Add(dist,waypoint);
		
		}

		int counter = 0;

		foreach (var item in distanceToObject.OrderBy(i => i.Key))
		{   

			if(counter<number &&item.Value.name!=source.gameObject.name){
			result[counter]=item.Value; 
			counter ++;
			}
		}

	
		return result;
	}




	void activateWaypoint(Transform waypoint){

		Transform sprite = waypoint.GetChild(0);
		SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
		renderer.enabled = true;
	
	}
	
	void deactivateWaypoint(Transform waypoint){
		
		Transform sprite = waypoint.GetChild(0);
		SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
		renderer.enabled = false;
		
	}


	void hideAllWaypoints(){
		
		for (int i = 0; i < horizontalwaypoints.Length; i++) {
			
			Transform sprite = horizontalwaypoints[i].transform.GetChild(0);
			SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
			renderer.enabled = false;
		}
		
		for (int i = 0; i < verticalwaypoints.Length; i++) {
			
			Transform sprite = verticalwaypoints[i].transform.GetChild(0);
			SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
			renderer.enabled = false;
			
		}
		
	}
	

	void rotateWaypoints(){
		
		for (int i = 0; i < path.Length; i++) {

			Transform waypoint = path[i];
			Waypoint waypointscript = waypoint.GetComponent<Waypoint>();

			if(waypoint!=endpoint){

			if(waypoint!=startpoint){

			if(!waypointscript.hitbyplayer){
				
			if(waypoint.gameObject.tag == "HorizontalWaypoint"){

			if(waypoint.transform.position.x > player.transform.position.x){
				
				if(waypoint.transform.GetChild(0).transform.rotation.eulerAngles.x !=90){
					waypoint.transform.GetChild(0).transform.Rotate(180,0,0);}
			}
			
			else{
				if(waypoint.transform.GetChild(0).transform.rotation.eulerAngles.x !=270f){
					waypoint.transform.GetChild(0).transform.Rotate(-180,0,0);
				}

			}


		  }

			if(waypoint.gameObject.tag == "VerticalWaypoint"){

				if(waypoint.transform.position.z > player.transform.position.z){
					
					if(waypoint.transform.GetChild(0).transform.rotation.eulerAngles.x !=90){
						waypoint.transform.GetChild(0).transform.Rotate(180,0,0);}
					
				}
				
				else{
					if(waypoint.transform.GetChild(0).transform.rotation.eulerAngles.x !=270f){
						waypoint.transform.GetChild(0).transform.Rotate(-180,0,0);
					}
				}


			}	

			}//hit

			}

		  }
			
		}//for
		
	}


	void testCrossing(GameObject crossing){

		GameObject[] List = getNearestNeighbors (crossing.transform, 3,allwaypoints);

		foreach (GameObject t in List) {

			Debug.Log(t);
		}


	}

	
	Transform findTargetDoor(string _string){
		
		GameObject target = null;
		for(int i = 0; i < doors.Length; i++)
		{
			string name = (doors[i].name);
			
			if(name == _string){
				
				target = doors[i];
			}
		}
		return target.transform;
	}



	void Update () {

	}
}
