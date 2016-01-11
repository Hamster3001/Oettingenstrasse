using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Navigation_build : MonoBehaviour {

	private GameObject[] doors;
	private GameObject player;
	public string targetstring;
	private Transform target;
	private GameObject[] horizontalwaypoints;
	private GameObject[] verticalwaypoints;
	private GameObject[] allwaypoints;
	private List<Transform> donepoints;
	private List<Transform> path;
	private List<GameObject> allwaypoints2; 
	private int while_counter = 0;
	private Transform startpoint;
	private Transform endpoint;
	private bool rotateonce;



	void Start () {


		doors = GameObject.FindGameObjectsWithTag("NamedDoor");
		horizontalwaypoints = GameObject.FindGameObjectsWithTag("HorizontalWaypoint");
		verticalwaypoints = GameObject.FindGameObjectsWithTag("VerticalWaypoint");
		allwaypoints = new GameObject[horizontalwaypoints.Length+verticalwaypoints.Length];
		horizontalwaypoints.CopyTo (allwaypoints, 0);
		verticalwaypoints.CopyTo (allwaypoints, horizontalwaypoints.Length);
		allwaypoints2 = new List<GameObject>();  

		foreach (GameObject g in allwaypoints) {
			allwaypoints2.Add(g);
		}


		player = GameObject.FindGameObjectWithTag ("Player");

		if (Menuscript.searchstring != null) {
		
			targetstring = Menuscript.searchstring;

		} else if (targetstring == "") {
			targetstring = "B001";
		}

		startPath ();
	
	}
	
    Transform[] getStartPoints(Transform source,Transform target){

		List<GameObject> twoNN = getNearestNeighbors (source, 2,allwaypoints2);
		Transform startObject = getNearestNeighbor(target, twoNN);
		twoNN.Remove(startObject.gameObject);
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
		donepoints = new List<Transform> ();
		startpoint = startpoints [0];
		path = new List<Transform> ();

//		Debug.Log ("END: "+endpoint);
//		Debug.Log ("START: "+startpoint);

		if (startpoint != endpoint) {
			path = buildPath (startpoint, endpoint);
		} else
			path.Add (endpoint);

		if (path!=null) {
//			Debug.Log("PATHLENGTH" +path.Length);
			foreach (Transform p in path) {

				activateWaypoint (p);

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
				endpoint.transform.GetChild (0).transform.eulerAngles = new Vector3 (90f, -180f, 0f);
			}


		}	  
	
		Transform sprite = startpoint.gameObject.transform.GetChild(0);
		SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
		Sprite mysprite = Resources.Load("startsprite",typeof(Sprite)) as Sprite;
		renderer.sprite = mysprite;

		Transform esprite = endpoint.gameObject.transform.GetChild(0);
		SpriteRenderer erenderer = esprite.GetComponent<SpriteRenderer>();
		Sprite emysprite = Resources.Load("endsprite",typeof(Sprite)) as Sprite;
		erenderer.sprite = emysprite;

	}


	 List<Transform> buildPath(Transform startpoint, Transform endpoint){

		ArrayList resultpathes = new ArrayList ();
		List<Transform> currentpoints = new List<Transform> ();

		currentpoints.Add (startpoint);
		List<Transform> resultpath = new List<Transform>();
		resultpath.Add (startpoint);
		resultpathes.Add (resultpath);

		while (true) {
		
			while_counter++;
			if(while_counter>10000)break;

			for (int i=0;i<currentpoints.Count;i++){

				Transform p = currentpoints[i];
				if(p==null){
					continue;
				}

				List<Transform> nextpoints = getOneStepPoints(p);

				bool first = true;
				int idx = currentpoints.IndexOf(p);

				if(nextpoints.Count==0){
					currentpoints[idx]=null;
					resultpathes[idx]=null;
					continue;
				}

				List<Transform> currentresultpath = new List<Transform>((List<Transform>)resultpathes[idx]);

				foreach (Transform np in nextpoints){
					if(first){

						currentresultpath = (List<Transform>)resultpathes[idx];
						List<Transform> tempcurrentresultpath = new List<Transform>((List<Transform>)resultpathes[idx]);
						tempcurrentresultpath.Add(np);
						resultpathes[idx] = tempcurrentresultpath;
						currentpoints[idx] = np;
					
						first = false;
					}

					else{

						currentpoints.Add(np);
						List<Transform> newresultpath = new List<Transform>(currentresultpath);
						newresultpath.Add (np);
						resultpathes.Add(newresultpath);

					}

					if(np==endpoint){
						currentresultpath.Add (np);	
						return currentresultpath;
					}


				}
				if(!donepoints.Contains(p)){
					donepoints.Add(p);
				}
			} //for each

		}
		return null;
	}

	
	List<Transform> getOneStepPoints(Transform p){

		List<Transform> candidates = new List<Transform> ();
		List<Transform> result = new List<Transform> ();
		string name = p.gameObject.name;

		if (name.Contains ("split")) {
		List<GameObject>  tempresult = getNearestNeighbors (p, 3, allwaypoints2);

			foreach(GameObject g in tempresult){

				candidates.Add(g.transform);
	        }

		} else if (name.Contains ("fourway")) {

			List<GameObject>  tempresult = getNearestNeighbors (p, 4, allwaypoints2);
			
			foreach(GameObject g in tempresult){
				
				candidates.Add(g.transform);
			}
		} else {

			List<GameObject>  tempresult = getNearestNeighbors (p, 2, allwaypoints2);
			
			foreach(GameObject g in tempresult){
				
				candidates.Add(g.transform);
			}
		}
			
		foreach(Transform can in candidates){

				if(!donepoints.Contains(can)){

				result.Add(can);

				}

		}

		return result;
	}

	

	Transform getNearestNeighbor (Transform source, List<GameObject>allwaypoints)
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




	List<GameObject> getNearestNeighbors (Transform source, int number, List<GameObject>waypoints)
	{	
	
	
		List<GameObject> result = new List<GameObject> ();

	    Vector3 currentPosition = source.position;
				var distanceToObject = new Dictionary <float, GameObject>(waypoints.Count);  
	
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
				result.Add(item.Value);
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
		
		for (int i = 0; i < path.Count; i++) {

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

		List<GameObject> List = getNearestNeighbors (crossing.transform, 3,allwaypoints2);

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

		foreach(Transform child in target.transform)
		{
			if(child.gameObject.name.Contains("Door")){

			Renderer rend = child.gameObject.GetComponent<Renderer>();
				rend.material.color = Color.cyan;
			}
		}


		return target.transform;
	}



	void Update () {

	}
}
