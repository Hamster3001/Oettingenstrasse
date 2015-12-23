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
	
	public GameObject ceiling;

	public Camera overviewCam;

	public Cardboard cardboard;
	public Transform head;
	public GameObject cardboardCam;
	public GameObject sphere;

	private AndroidJavaObject bridge;

	private State state;

	// Use this for initialization
	void Start () {
		bridge = new AndroidJavaObject ("com.example.player.Bridge");

		state = State.CardboardVR;

		bridge.Call ("initializeFingerprints", textAsset.text);
	}
	
	// Update is called once per frame
	void Update () {
		textMiddle.text = cardboard.transform.position.x + " , " + cardboard.transform.position.z;

		Touch[] touches = Input.touches;
		if (touches.Length == 1) {
			if (touches[0].phase.Equals (TouchPhase.Moved)) {
				if (state != State.Overview
				    && touches[0].deltaPosition.y > 5.0f) {
					state = State.Overview;
					cardboardCam.SetActive(false);
					sphere.SetActive(true);
					overviewCam.gameObject.SetActive(true);
					ceiling.SetActive(false);
				}
				else if (state == State.Overview
				    && touches[0].deltaPosition.y < -5.0f) {
					state = State.CardboardVR;
					ceiling.SetActive(true);
					overviewCam.gameObject.SetActive(false);
					sphere.SetActive(false);
					cardboardCam.SetActive(true);
					cardboard.VRModeEnabled = true;
				}
				else if (state == State.CardboardVR
				    && touches[0].deltaPosition.x > 5.0f) {
					state = State.Cardboard3D;
					cardboard.VRModeEnabled = false;
				}
				else if (state == State.Cardboard3D
				    && touches[0].deltaPosition.x < -5.0f) {
					state = State.CardboardVR;
					cardboard.VRModeEnabled = true;
				}
			}
			else if (touches[0].phase.Equals (TouchPhase.Began)
			         && touches[0].position.x < Screen.width/2
			         && touches[0].position.y > Screen.height/2) {
				textLeft.text = "Record Fingerprint";
				bridge.Call ("recordFingerprint", cardboard.transform.position.x, cardboard.transform.position.z);
			}
			else if (touches[0].phase.Equals (TouchPhase.Began)
			         && touches[0].position.x < Screen.width/2
			         && touches[0].position.y < Screen.height/2) {
				textLeft.text = "Find Position";
				bridge.Call ("findPosition");
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
