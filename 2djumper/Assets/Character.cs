using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI... probably not needed?
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro stuff

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour {

    public GameObject camera;

    enum Event {Left, Right, LeftWall, RightWall, Idle};

    Event playerAction;
    Event previous;

    public Rigidbody2D player;
    public float jumpF = 12f;
    public float playerAccel = 0.01f;
   
   
    public float maxSpeed = 20f;
    public float baseSpeed = 5f;
    public float currentSpeed = 0f;

    private int score;
    
   
    public TextMeshProUGUI scoreText;
    
   
    

    bool allowJump;
    bool wallJump;
    bool touchRightWall =false;
    bool touchLeftWall = false;
    
    bool grounded = false;
    

	// Use this for initialization
	void Start () {
        player = GetComponent<Rigidbody2D>();
        allowJump = false;
        wallJump = false;
        playerAction = Event.Idle;
        score = 0;
                       
        
	}

    // Update is called once per frame
    private void Update()
    {
        
    }

    void FixedUpdate () { 
        updateScore();
        getPlayerEvent();
        movePlayer();
        checkGameOver();
	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        Rigidbody2D rb = other.collider.GetComponent<Rigidbody2D>();

        if (other.gameObject.tag == "Platform"  && other.relativeVelocity.y >= 0f)
        {
            grounded = true; 
            allowJump = true;
        }

        if(other.gameObject.tag =="Left Wall")
        {
           touchLeftWall = true;
           wallJump = true;
        }

        if(other.gameObject.tag =="Right Wall")
        {
           touchRightWall = true;
           wallJump = true;
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
            wallJump = false;
        }

        if (other.gameObject.tag == "Right Wall")
        {
            touchRightWall = false;
            wallJump = false;
        }
    }

    void updateScore()
    {
        Vector2 position = player.position;
        

        if(position.y > (score + 1) * 2.5f)
        {
            score++;
        }
        
        scoreText.SetText("Score: " + score.ToString()); // can also use scoreText.text = "<msg>"
        
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
                currentSpeed = -currentSpeed;
            }
            else if (previous == Event.Left || previous == Event.Idle)
            {
                currentSpeed = baseSpeed;
            }
            else
            {
                currentSpeed += playerAccel;
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
            }
            velocity.x = currentSpeed;

        }

        if (playerAction == Event.Left)
        {
            if (previous == Event.RightWall)
            {

                currentSpeed = -currentSpeed;
            }
            else if (previous == Event.Right || previous == Event.Idle)
            {
                currentSpeed = -baseSpeed;
            }
            else
            {
                currentSpeed -= playerAccel;
                currentSpeed = Mathf.Max(currentSpeed, -maxSpeed);

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

    void checkGameOver()
    {
        if(camera.transform.position.y > player.position.y + 9f)
        {            
            //Destroy(gameObject); //destroys the game object this script is attached to
            SceneManager.LoadScene(0);
        }
    }
}

   


    
    
        
    

