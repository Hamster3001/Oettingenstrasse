using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public Transform movementTarget;
	public Transform rotationTarget;
	
	Vector3 offset;
	
	void Start() {
		transform.position = new Vector3(movementTarget.position.x, 10.0f, movementTarget.position.z);
		offset = transform.position - movementTarget.position;
		transform.position = movementTarget.position + offset;
		
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3 (transform.rotation.eulerAngles.x, rotationTarget.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
		transform.rotation = q;
	}
	
	void Update() {
		transform.position = movementTarget.position + offset;
		
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3 (transform.rotation.eulerAngles.x, rotationTarget.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
		transform.rotation = q;
	}
}
