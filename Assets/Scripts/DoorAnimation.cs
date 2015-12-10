using UnityEngine;
using System.Collections;

public class DoorAnimation : MonoBehaviour {

    private Animator anim;
    private GameObject player;
    private int count;
    private Transform door;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Door")
                {
                    door = child;
                }
            }
            count++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        { 
            count = Mathf.Max(0, count-1);
        }
    }


	void Update()
	{  
        anim.SetBool("Open",count>0);
    }
}
