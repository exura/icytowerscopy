using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI... probably not needed?
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro stuff

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterExperimental : MonoBehaviour {

    public GameObject camera;

    enum Event {Left, Right, LeftWall, RightWall, Idle};

    Event playerAction;
    Event previous;
    Event previousWallJump;

    public Rigidbody2D player;
    public float jumpF = 12f;
    public int boostCeil = 100; // if counter above we will increase speed
    public float maxSpeed = 12f;
    public float baseSpeed = 6f;
    public float currentSpeed = 0f;

	public int platformScore = 5;
	public int wallScore = 10;
	public int checkpointScore = 50;

	public GameControl gc;

    private int boostCounter = 0;
  
    private int airBourneCounter = 0;

	private Camera cam;
    

    

    bool allowJump;
   
    bool touchRightWall =false;
    bool touchLeftWall = false;
   
    
    bool grounded = false;
    

	// Use this for initialization
	void Start () {
        player = GetComponent<Rigidbody2D>();
        allowJump = false;       
        playerAction = Event.Idle;
                             
	}

    // Update is called once per frame
    private void Update()
    {
		Vector3 pos = Camera.main.WorldToViewportPoint (player.position);

		if (pos.y < 0) {
			gc.PlayerDied();
		}
    }

    void FixedUpdate()
    {

        getPlayerEvent();             
        movePlayer();
//        checkGameOver();        
    }
        
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        Rigidbody2D rb = other.collider.GetComponent<Rigidbody2D>();

        if (other.gameObject.tag == "Platform"  && other.relativeVelocity.y >= 0f)
        {
			gc.addScore (platformScore);
            grounded = true; 
            allowJump = true;
        }

        if(other.gameObject.tag =="Left Wall")
        {
		   gc.addScore (wallScore);	
           touchLeftWall = true;
           
        }

        if(other.gameObject.tag =="Right Wall")
        {
		   gc.addScore (wallScore);
           touchRightWall = true;
        }

		if (other.gameObject.tag == "Checkpoint" && other.relativeVelocity.y >= 0f) {
			grounded = true;
			allowJump = true;
		}

    }

	private void OnTriggerEnter2D(Collider2D other) { // Need to do as trigger since it should be enough to just pass it, not land on it.
		if (other.gameObject.tag == "Checkpoint") {
			gc.addScore (checkpointScore);
		}
	}

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Platform" && other.relativeVelocity.y >= 0f)
        {
            grounded = false;
            allowJump = false;
        }
        if (other.gameObject.tag == "Left Wall")
        {
			
            touchLeftWall = false;
            
        }

        if (other.gameObject.tag == "Right Wall")
        {
            touchRightWall = false;
            
        }
    }

    void getPlayerEvent()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (touchLeftWall)
            {
                previous = playerAction;
                playerAction = Event.LeftWall;
            }
            else
            {
                previous = playerAction;
                playerAction = Event.Left;
            }
        }

        //moving right
        else if (Input.GetKey(KeyCode.D))
        {
                if (touchRightWall)
                {
                    previous = playerAction;
                    playerAction = Event.RightWall;
                }
                else
                {
                    previous = playerAction;
                    playerAction = Event.Right;
                }
            }

        //standing still
        else
        {
            if (touchRightWall)
            {
                previous = playerAction;
                playerAction = Event.RightWall;
            }
            else if (touchLeftWall)
            {
                previous = playerAction;
                playerAction = Event.LeftWall;
            }
            else if (grounded)
            {
                previous = playerAction;
                playerAction = Event.Idle;
            }

        }
    }  

    void movePlayer()
    {
        Vector2 velocity = player.velocity;

        if (playerAction == Event.Idle)
        {
            velocity.x = 0;
        }

        if (playerAction == Event.Right)
        {
            if (previous == Event.LeftWall)
            {
                //do not reset boostCounter
                currentSpeed = -currentSpeed;
            }
            else if (previous == Event.Left || previous == Event.Idle)
            {
                //reset boostCounter. We fkd up the momentum
                currentSpeed = baseSpeed;
                boostCounter = 0;
            }
            else
            {
                //we are continuing in the right direction for momentum
                boostCounter++;
                if(boostCounter > boostCeil)
                {
                    currentSpeed = maxSpeed;
                }
                else
                {
                    currentSpeed = baseSpeed;
                }
                
            }
          
             velocity.x = currentSpeed;
  
        }

        if (playerAction == Event.Left)
        {
            if (previous == Event.RightWall)
            {
                //do not reset boostCounter
                currentSpeed = -currentSpeed;
            }
            else if (previous == Event.Right || previous == Event.Idle)
            {
                //reset boostCounter. We fkd up the momentum
                boostCounter = 0;
                currentSpeed = -baseSpeed;
            }
            else
            {
                //we are continuing in the right direction for momentum
                boostCounter++;
                if(boostCounter > boostCeil)
                {
                    currentSpeed = -maxSpeed;
                }
                else
                {
                    currentSpeed = -baseSpeed;
                }               
            }

             velocity.x = currentSpeed;
  
        }

        if (playerAction == Event.RightWall)
        {
            velocity.x = 0;

        }

        if (playerAction == Event.LeftWall)
        {
            velocity.x = 0;

        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (allowJump)
            {
                velocity.y = jumpF + Mathf.Abs(velocity.x * 1.5f);
            }                              
        }
        player.velocity = velocity;

    }

//    void checkGameOver()
//    {
//        if(camera.transform.position.y > player.position.y + 9f)
//        {            
//            //Destroy(gameObject); //destroys the game object this script is attached to
//            SceneManager.LoadScene(0);
//        }
//    }
}

   


    
    
        
    

