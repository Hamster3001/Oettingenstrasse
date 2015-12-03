using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PluginScript : MonoBehaviour {

	public Text textLeft;
	public Text textRight;
	public Transform cardboard;
	public Transform head;

	private AndroidJavaObject bridge;

	// Use this for initialization
	void Start () {
		bridge = new AndroidJavaObject ("com.example.player.Bridge");
	}
	
	// Update is called once per frame
	void Update () {
		Touch[] touches = Input.touches;
		if (touches.Length == 1) {
			if (touches[0].phase.Equals(TouchPhase.Stationary)) {
				float radian = ((2.0f * Mathf.PI)/360.0f) * head.rotation.eulerAngles.y;
				float xFactor = Mathf.Sin(radian);
				float yFactor = Mathf.Cos(radian);
				cardboard.position = new Vector3(cardboard.position.x + (xFactor * 0.1f), cardboard.position.y,
				                                 cardboard.position.z + (yFactor * 0.1f));
			}
		}
		else if (touches.Length == 2) {
			if (touches[0].phase.Equals(TouchPhase.Began) && touches[1].phase.Equals(TouchPhase.Began)) {
				bridge.Call ("recordFingerprint", cardboard.position.x, cardboard.position.z);
				float[] rec = bridge.Call<float[]> ("getRecord");
				textLeft.text = "X: " + rec[0] + ", Y: " + rec[1];
			}
		}
	}

	void SetLeftText(string text) {
		textLeft.text = text;
	}
	
	void SetRightText(string text) {
		textRight.text = text;
	}
}
