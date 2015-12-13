using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PluginScript : MonoBehaviour {

	enum State {Overview, CardboardVR, Cardboard3D};

	public Text textLeft;
	public Text textRight;
	
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
	}
	
	// Update is called once per frame
	void Update () {
		Touch[] touches = Input.touches;
		if (touches.Length == 1) {
			if (touches[0].tapCount == 2) {
				textLeft.text = "Double Klick";
				bridge.Call ("findPosition");
			}
			/*
			else if (touches[0].phase.Equals (TouchPhase.Stationary)) {
				float radian = ((2.0f * Mathf.PI) / 360.0f) * head.rotation.eulerAngles.y;
				float xFactor = Mathf.Sin (radian);
				float yFactor = Mathf.Cos (radian);
				cardboard.transform.Translate (xFactor * Time.deltaTime, 0, yFactor * Time.deltaTime);
			}
			*/
			else if (state != State.Overview
			         && touches[0].phase.Equals (TouchPhase.Moved)
			         && touches[0].deltaPosition.y > 10.0f) {
				state = State.Overview;
				cardboardCam.SetActive(false);
				sphere.SetActive(true);
				overviewCam.gameObject.SetActive(true);
				ceiling.SetActive(false);
			}
			else if (state == State.Overview
			         && touches[0].phase.Equals (TouchPhase.Moved)
			         && touches[0].deltaPosition.y < -10.0f) {
				state = State.CardboardVR;
				ceiling.SetActive(true);
				overviewCam.gameObject.SetActive(false);
				sphere.SetActive(false);
				cardboardCam.SetActive(true);
				cardboard.VRModeEnabled = true;
			}
			else if (state == State.CardboardVR
			         && touches[0].phase.Equals (TouchPhase.Moved)
			         && touches[0].deltaPosition.x > 10.0f) {
				state = State.Cardboard3D;
				cardboard.VRModeEnabled = false;
			}
			else if (state == State.Cardboard3D
			         && touches[0].phase.Equals (TouchPhase.Moved)
			         && touches[0].deltaPosition.x < -10.0f) {
				state = State.CardboardVR;
				cardboard.VRModeEnabled = true;
			}
		}
		else if (touches.Length == 2) {
			if (touches[0].phase.Equals (TouchPhase.Began)
			    && touches[1].phase.Equals (TouchPhase.Began)) {
				bridge.Call ("recordFingerprint", cardboard.transform.position.x, cardboard.transform.position.z);
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

			textRight.text = "X: " + xPosition.ToString() + ", Z: " + zPosition.ToString() + "\n" + parameterArray[2];
		}
		else {
			textRight.text = "Wrong Parameter Size";
		}
	}
}
