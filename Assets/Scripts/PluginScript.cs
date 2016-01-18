using UnityEngine;
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

	public Camera overviewCam;

	public GameObject moveButton;
	public Cardboard cardboard;
	public Transform head;
	public GameObject cardboardCam;
	public GameObject sphere;
	
	private AndroidJavaObject bridge;
	private State state;
	private GameObject[] doors;

	// Use this for initialization
	void Start () {
		cardboard.VRModeEnabled = Menuscript.vrEnabled;
		if (cardboard.VRModeEnabled)
			state = State.CardboardVR;
		else
			state = State.Cardboard3D;

		if (!Menuscript.movementEnabled)
			moveButton.SetActive(true);

		if (!Menuscript.locationEnabled) {
			doors = GameObject.FindGameObjectsWithTag("NamedDoor");
			foreach (GameObject d in doors) {
				if (d.name.Equals(Menuscript.locationstring)) {
					cardboard.transform.position = new Vector3(d.transform.position.x,
					                                           cardboard.transform.position.y,
					                                           d.transform.position.z);
					cardboard.transform.rotation = d.transform.rotation;

					Transform child = d.transform.FindChild("Frame");
					if (child == null) {
						child = d.transform.FindChild("Cube");
					}
					if (child != null) {
						float size = child.GetComponent<MeshRenderer>().bounds.size.z;
						cardboard.transform.Translate((-size/2.0f)*Vector3.forward);
						if (size < 1.0f)
							cardboard.transform.Translate(Vector3.right);
						else
							cardboard.transform.Translate(size*Vector3.right);
						Debug.Log("Size: " + size);
					}
					else {
						cardboard.transform.Translate(Vector3.right);
					}

					cardboard.transform.Rotate(new Vector3(0, -90, 0));
					cardboard.transform.eulerAngles = new Vector3(0, cardboard.transform.eulerAngles.y, 0);
					break;
				}
			}
		}

		bridge = new AndroidJavaObject ("com.example.player.Bridge");
		bridge.Call ("initializeFingerprints", textAsset.text);
	}
	
	// Update is called once per frame
	void Update () {
		textMiddle.text = cardboard.transform.position.x + " , " + cardboard.transform.position.z;

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel(0);
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
			} else if (touches [0].phase.Equals (TouchPhase.Began)
				&& touches [0].position.x < Screen.width / 2
				&& touches [0].position.y > Screen.height-200) {
				textLeft.text = "Record Fingerprint";
				bridge.Call ("recordFingerprint", cardboard.transform.position.x, cardboard.transform.position.z);
			} else if (touches [0].phase.Equals (TouchPhase.Began)
				&& touches [0].position.x < Screen.width / 2
				&& touches [0].position.y < 200) {
				textLeft.text = "Find Position";
				bridge.Call ("findPosition");
			}
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
		textLeft.text = text;
	}
	
	void SetRightText(string text) {
		textRight.text = text;
	}

	void AddLeftText(string text) {
		textLeft.text = textLeft.text + "\n" + text;
	}

	void AddRightText(string text) {
		textRight.text = textRight.text + "\n" + text;
	}

	void SetPosition(string text) {
		string[] parameterArray = text.Split (new char[]{'#'});
		if (parameterArray.Length == 3) {
			float xPosition = float.Parse(parameterArray[0]);
			float zPosition = float.Parse(parameterArray[1]);
			cardboard.transform.position = new Vector3(xPosition, cardboard.transform.position.y, zPosition);

			textLeft.text = "Position: X=" + xPosition.ToString() + ", Z=" + zPosition.ToString() + "\n" + parameterArray[2];
		}
		else {
			textLeft.text = "Wrong Parameter Size";
		}
	}
}
