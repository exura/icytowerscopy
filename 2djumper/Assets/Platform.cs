using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform1 : MonoBehaviour {

    public float jumpF = 10f;
   


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();

        Vector2 relVel = collision.relativeVelocity;

        if(rb != null)
        {
            Vector2 vel = rb.velocity;
            vel.y = jumpF;
            rb.velocity = vel;    
        }
    }


    private void LateUpdate()
    {
        
    }
}
