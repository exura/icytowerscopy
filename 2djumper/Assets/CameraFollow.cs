using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target; // create reference in inspector

    public float scrollSpeed =0.05f;
    public float newPositionY;

	// Use this for initialization
	void Start () {
        newPositionY = transform.position.y;
		
	}
	
	// Update is called once per frame
	void Update () {

      
		
	}

    private void FixedUpdate()
    {
        if (target.position.y > transform.position.y)
        {
            newPositionY = target.position.y + scrollSpeed;
            transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
        }
        else
        {
            newPositionY += scrollSpeed;
            transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
        }
       
    }
}
