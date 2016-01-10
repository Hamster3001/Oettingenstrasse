using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
	

	private GameObject player;
	public bool hitbyplayer;
	private SpriteRenderer renderer;
	private Transform sprite;
	
	// Use this for initialization
	void Start()
	{	
		hitbyplayer = false;
		player = GameObject.FindGameObjectWithTag("Player");
		sprite = this.gameObject.transform.GetChild(0);
		renderer = sprite.GetComponent<SpriteRenderer>();


	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == player)
		{
			hitbyplayer = true;
			renderer.color =  new Color(0f,255f,255f,0.5f);
			//	renderer.enabled = false;
		}

	

	}

}
