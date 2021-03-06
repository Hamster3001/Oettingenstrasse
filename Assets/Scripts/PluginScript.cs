﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class PluginScript : MonoBehaviour {

	enum State {Overview, CardboardVR, Cardboard3D};

	public TextAsset textAsset;

	public Text textLeft;
	public Text textRight;
	public Text textMiddle;
	
	public GameObject[] ceiling;
	public GameObject staircubes;
	public GameObject entrance;

	public Camera overviewCam;

	public Button moveButton;
	public Button pauseButton;
	public Navigation_build navigation;
	public Cardboard cardboard;
	public CardboardHead head;
	public GameObject cardboardCam;
	public GameObject sphere;
	
	private AndroidJavaObject bridge;
	private GameObject[] doors;
	public static bool pause;
	private bool pressedPause;
	private State state;
	private Sprite playsprite;
	private Sprite stopsprite;
	
	// Use this for initialization
	void Start () {
		pauseButton.transform.position = new Vector3 (Screen.width/2, 50, 0);
		moveButton.transform.position = new Vector3 (Screen.width-100, 100, 0);
		textLeft.transform.position = new Vector3 (Screen.width/4, Screen.height/2);

		// Für Editor folgende vier Zeilen auskommentieren
		bridge = new AndroidJavaObject ("com.example.player.Bridge");
		bridge.Call ("trackMovement", false);
		if (textAsset != null)
			bridge.Call ("initializeFingerprints", textAsset.text);

		doors = GameObject.FindGameObjectsWithTag("NamedDoor");

		pause = true;
		pressedPause = false;
		moveButton.interactable = false;

		playsprite = Resources.Load("play",typeof(Sprite)) as Sprite;
		stopsprite = Resources.Load("stop",typeof(Sprite)) as Sprite;

		// VR Mode
		cardboard.VRModeEnabled = Menuscript.vrEnabled;
		if (cardboard.VRModeEnabled)
			state = State.CardboardVR;
		else
			state = State.Cardboard3D;

		// Movement
		if (!Menuscript.vrEnabled)
			moveButton.gameObject.SetActive(true);

		// Location
		if (Menuscript.locationEnabled) {
			bridge.Call ("findPosition");
		}
		else if (!Menuscript.locationEnabled) {
			foreach (GameObject d in doors) {
				if (d.name.Equals(Menuscript.locationstring)) {
					InitPosition(d);
					return;
				}
			}
			InitPosition(entrance);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//textMiddle.text = cardboard.transform.position.x + " , " + cardboard.transform.position.z;

		if (Input.GetButtonDown ("Pause")) {
			if (pause) {
				pauseButton.interactable = false;
				pauseButton.GetComponent<Image> ().sprite = stopsprite;
				SetRightText ("Start Tracking");
				pause = false;
				cardboard.enabled = true;
				head.trackRotation = true;
				textMiddle.text = "";
				pressedPause = false;
				pauseButton.interactable = true;
				moveButton.interactable = true;
				bridge.Call ("trackMovement", Menuscript.movementEnabled);
				cardboard.transform.eulerAngles = new Vector3(cardboard.transform.eulerAngles.x,
				                                              head.transform.eulerAngles.y,
				                                              cardboard.transform.eulerAngles.z);
			}
			else {
				SetRightText ("Stop Tracking");
				pause = true;
				pauseButton.GetComponent<Image> ().sprite = playsprite;
				cardboard.enabled = false;
				head.trackRotation = false;
				pressedPause = false;
				moveButton.interactable = false;
				bridge.Call ("trackMovement", false);
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel (0);
		} else if (Input.GetKeyDown (KeyCode.O) && !Menuscript.vrEnabled) {
			if (state != State.Overview) {
				state = State.Overview;
				cardboardCam.SetActive (false);
				sphere.SetActive (true);
				overviewCam.gameObject.SetActive (true);
				staircubes.SetActive (false);
				for (int i=0; i<ceiling.Length; i++) {
					ceiling [i].SetActive (false);
				}
			} else if (state == State.Overview) {
				if (cardboard.VRModeEnabled)
					state = State.CardboardVR;
				else
					state = State.Cardboard3D;
				staircubes.SetActive (true);
				for (int i=0; i<ceiling.Length; i++) {
					ceiling [i].SetActive (true);
				}
				overviewCam.gameObject.SetActive (false);
				sphere.SetActive (false);
				cardboardCam.SetActive (true);
			}
		} else if (Input.GetKey (KeyCode.Plus)) {
			if (state == State.Overview) {
				float zoom = -1.0f;
				
				overviewCam.orthographicSize += zoom * 0.5f;
				if (overviewCam.orthographicSize < 2)
					overviewCam.orthographicSize = 2;
				else if (overviewCam.orthographicSize > 40)
					overviewCam.orthographicSize = 40;
			}
		} else if (Input.GetKey (KeyCode.Minus)) {
			if (state == State.Overview) {
				float zoom = 1.0f;
				
				overviewCam.orthographicSize += zoom * 0.5f;
				if (overviewCam.orthographicSize < 2)
					overviewCam.orthographicSize = 2;
				else if (overviewCam.orthographicSize > 40)
					overviewCam.orthographicSize = 40;
			}
		}

		Touch[] touches = Input.touches;
		if (touches.Length == 1) {
			if (touches [0].phase.Equals (TouchPhase.Moved)) {
				if (state != State.Overview
					&& touches [0].deltaPosition.y > 5.0f) {
					state = State.Overview;
					cardboardCam.SetActive (false);
					sphere.SetActive (true);
					overviewCam.gameObject.SetActive (true);
					staircubes.SetActive (false);
					for (int i=0; i<ceiling.Length; i++) {
						ceiling [i].SetActive (false);
					}
				} else if (state == State.Overview
					&& touches [0].deltaPosition.y < -5.0f) {
					if (cardboard.VRModeEnabled)
						state = State.CardboardVR;
					else
						state = State.Cardboard3D;
					staircubes.SetActive (true);
					for (int i=0; i<ceiling.Length; i++) {
						ceiling [i].SetActive (true);
					}
					overviewCam.gameObject.SetActive (false);
					sphere.SetActive (false);
					cardboardCam.SetActive (true);
				}
			}
//			else if (touches [0].phase.Equals (TouchPhase.Began)
//			           && touches [0].position.x < 200
//			           && touches [0].position.y > Screen.height-100) {
//				SetLeftText("Record Fingerprint");
//				bridge.Call ("recordFingerprint", cardboard.transform.position.x, cardboard.transform.position.z);
//			} else if (touches [0].phase.Equals (TouchPhase.Began)
//			           && touches [0].position.x < 200
//			           && touches [0].position.y < 100) {
//				SetLeftText("Find Position");
//				bridge.Call ("findPosition");
//			}
		}
		else if (touches.Length == 2) {
			if (state == State.Overview) {
				float zoom = 0.0f;
				if (touches[0].position.x < touches[1].position.x) {
					zoom = touches[0].deltaPosition.x - touches[1].deltaPosition.x;
				}
				else if (touches[0].position.x > touches[1].position.x) {
					zoom = touches[1].deltaPosition.x - touches[0].deltaPosition.x;
				}

				overviewCam.orthographicSize += zoom * 0.5f;
				if (overviewCam.orthographicSize < 2)
					overviewCam.orthographicSize = 2;
				else if (overviewCam.orthographicSize > 40)
					overviewCam.orthographicSize = 40;
			}
		}
	}

	void SetLeftText(string text) {
//		textLeft.text = text;
	}
	
	void SetRightText(string text) {
//		textRight.text = text;
	}

	void AddLeftText(string text) {
//		textLeft.text = textLeft.text + "\n" + text;
	}

	void AddRightText(string text) {
//		textRight.text = textRight.text + "\n" + text;
	}

	IEnumerator Wait() {
		for (int i=15; i>=0; i--) {
			yield return new WaitForSeconds (1);
			textMiddle.text = i.ToString();
		}
		yield return new WaitForSeconds (1);

		SetRightText ("Start Tracking");
		pause = false;
		cardboard.enabled = true;
		head.trackRotation = true;
		textMiddle.text = "";
		pressedPause = false;
		pauseButton.interactable = true;
		moveButton.interactable = true;
		bridge.Call ("trackMovement", Menuscript.movementEnabled);
		cardboard.transform.eulerAngles = new Vector3(cardboard.transform.eulerAngles.x,
		                                              head.transform.eulerAngles.y,
		                                              cardboard.transform.eulerAngles.z);
		StartCoroutine(Go());
	}

	IEnumerator Go() {
		textLeft.text = "Go!";
		yield return new WaitForSeconds (5);
		textLeft.text = "";
	}
	
	public void Pause() {
		if (!pressedPause) {
			pressedPause = true;
			if (pause) {
				pauseButton.interactable = false;
				pauseButton.GetComponent<Image> ().sprite = stopsprite;
				if (Menuscript.vrEnabled) {
					StartCoroutine(Wait());
				}
				else {
					SetRightText ("Start Tracking");
					pause = false;
					cardboard.enabled = true;
					head.trackRotation = true;
					textMiddle.text = "";
					pressedPause = false;
					pauseButton.interactable = true;
					moveButton.interactable = true;
					bridge.Call ("trackMovement", Menuscript.movementEnabled);
					cardboard.transform.eulerAngles = new Vector3(cardboard.transform.eulerAngles.x,
					                                              head.transform.eulerAngles.y,
					                                              cardboard.transform.eulerAngles.z);
				}
			} else {
				SetRightText ("Stop Tracking");
				pause = true;
				pauseButton.GetComponent<Image> ().sprite = playsprite;
				cardboard.enabled = false;
				head.trackRotation = false;
				pressedPause = false;
				moveButton.interactable = false;
				bridge.Call ("trackMovement", false);
			}
		}
	}
	
	void InitPosition(GameObject door) {
		cardboard.transform.position = new Vector3(door.transform.position.x,
		                                           cardboard.transform.position.y,
		                                           door.transform.position.z);
		cardboard.transform.rotation = door.transform.rotation;
		
		Transform child = door.transform.FindChild("Frame");
		if (child == null) {
			child = door.transform.FindChild("Cube");
		}
		if (child != null) {
			if (child.name.Equals("Frame")) {
				float size = child.GetComponent<MeshRenderer>().bounds.size.z;
				if (size < 1.0f)
					cardboard.transform.Translate((-1.0f/2.0f)*Vector3.forward);
				else
					cardboard.transform.Translate((-size/2.0f)*Vector3.forward);
				if (size < 1.5f)
					cardboard.transform.Translate(1.5f*Vector3.right);
				else
					cardboard.transform.Translate(size*Vector3.right);

				cardboard.transform.Rotate(new Vector3(0, -90, 0));
			}
			else {
				float size = child.GetComponent<MeshRenderer>().bounds.size.z;
				if (size < 1.5f)
					cardboard.transform.Translate(-1.5f*Vector3.right);
				else
					cardboard.transform.Translate(-size*Vector3.right);

				cardboard.transform.Rotate(new Vector3(0, 90, 0));
			}
		}
		else {
			cardboard.transform.Translate(1.5f*Vector3.right);
			cardboard.transform.Rotate(new Vector3(0, -90, 0));
		}

		cardboard.transform.eulerAngles = new Vector3(0, cardboard.transform.eulerAngles.y, 0);

		navigation.enabled = true;
	}

	void SetPosition(string text) {
		string[] parameterArray = text.Split (new char[]{'#'});
		if (parameterArray.Length == 3) {
			float xPosition = float.Parse(parameterArray[0]);
			float zPosition = float.Parse(parameterArray[1]);

			cardboard.transform.position = new Vector3(xPosition, cardboard.transform.position.y, zPosition);
			Transform nn = getNearestNeighbor(cardboard.transform, doors);
			if (nn != null) {
				GameObject door = nn.gameObject;
				SetRightText("HitDoor: " + door.name);

				InitPosition(door);
			}
			else {
				InitPosition(entrance);
			}

			SetLeftText("Position: X=" + xPosition.ToString() + ", Z=" + zPosition.ToString() + "\n" + parameterArray[2]);
		}
		else {
			SetLeftText("Wrong Parameter Size");
		}
	}

	Transform getNearestNeighbor(Transform source, GameObject[] allwaypoints) {
		Transform bestTarget = null;
		float closestDist = Mathf.Infinity;
		Vector3 currentPosition = source.position;
		RaycastHit hit;

		foreach (GameObject potentialTarget in allwaypoints) {
			Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
			directionToTarget.y =0.001f;
			if (Physics.Raycast(currentPosition, directionToTarget, out hit)) {
				if (hit.transform.gameObject.tag.Equals("NamedDoor")) {
					float dist = directionToTarget.sqrMagnitude;
					if(dist < closestDist) {
						closestDist = dist;
						bestTarget = potentialTarget.transform;
					}
				}
			}
		}
		return bestTarget;
	}
}
