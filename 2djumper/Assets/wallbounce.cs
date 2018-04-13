using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallbounce : MonoBehaviour {

    public float bounceF= 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        Rigidbody2D rb = other.collider.GetComponent<Rigidbody2D>();
        Vector2 velocity = rb.velocity;
        
        
        if(rb != null)
        {
            if(velocity.x > 0)
            {
                print("bounce1 " + velocity.x);

              
                velocity.x = -bounceF;
                print("bounce2" + velocity.x);
            }
            else
            {
                
                velocity.x = bounceF;
            }
            rb.velocity = velocity;
        }

    }
}
