using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI... probably not needed?
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro stuff

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
	//enumration class
	enum PlayerStatus {RunLeft, RunRight, LeftWall, RightWall, Jumped, Idle};
	private PlayerStatus playerStatus;
	private PlayerStatus previousPlayerStatus;

	//public declarations
	public GameObject gameCamera;
	public Rigidbody2D player;
	public float jumpF = 12f; //jump force
	public float groundSpeed = 6f; 
	public float airSpeed = 3f; //PREPARED FOR  WHEN WE IMPLEMENT AIR MOVEMENT
	public float friction = 0.7f;
	public TextMeshProUGUI scoreText;
	public GameControl gc;
	public int platformScore = 5;
	public int wallScore = 10;
	public int checkpointScore = 50;
	public int bounceTime = 20;

	//private declarations
	private int boostCounter = 0;
	private int speedBoost = 100; // if counter above we will increase speed
	private int score = 0;
	private int bounceCounter = 0;
	private bool touchRightWall =false;
	private bool touchLeftWall = false;
	private bool grounded = false;
	private bool speedBoostActive = false;
	private bool bounce = false;

	void Start (){
		//initialize the player and get its rigidbody
		player = GetComponent<Rigidbody2D>();		       
		playerStatus = PlayerStatus.Idle;
		previousPlayerStatus = playerStatus;
	}

	private void Update(){	}

	void FixedUpdate(){		
		//Handle user inputs
		if (!bounce) {
			getPlayerEvent (); 
			checkPlayerSpeedBoost ();
		 	movePlayer (groundSpeed);
			checkGameOver (); 
		} else if (bounce) {
			resolveBounce ();
		}

	}

	void resolveBounce(){		
		bounceCounter++;
		if (bounceCounter > bounceTime || grounded) { //stop bounce if player on platform
			bounce = false;
			bounceCounter = 0;
		}
	}
	
	void getPlayerEvent(){
		//movement left to right or idle
		previousPlayerStatus = playerStatus;
		if (Input.GetAxis ("Horizontal") > 0) {
			if (touchRightWall) {
				playerStatus = PlayerStatus.RightWall;
			} else {
				playerStatus = PlayerStatus.RunRight;
			}
		} else if (Input.GetAxis ("Horizontal") < 0) {
			if (touchLeftWall) {
				playerStatus = PlayerStatus.LeftWall;
			} else  {
				playerStatus = PlayerStatus.RunLeft;
			}
		} else{				
			playerStatus = PlayerStatus.Idle;
		}
		//jumping
		if (Input.GetKey(KeyCode.Space) && grounded){			
			playerStatus = PlayerStatus.Jumped;
		}
	}

	void movePlayer(float speed){
		//read player velocity
		Vector2 velocity = player.velocity;
		//horizontal movement
		if(playerStatus == PlayerStatus.Idle){			
			velocity.x = 0; //handles slowing down
		}
		if(playerStatus == PlayerStatus.RunRight && !bounce){			
			if (speedBoostActive) {
				velocity.x = 2 * speed;
			} else {
				velocity.x = speed;
			}
		}else if(playerStatus == PlayerStatus.RunLeft && !bounce){
			if (speedBoostActive) {
				velocity.x = -2 * speed;
			} else {
				velocity.x = -speed;
			}
		}
		//bounce
		if (playerStatus == PlayerStatus.RightWall) {
			bounce = true;
			velocity.x = -1.3f * speed;
			//resolve player status (continuing the momentum)
			previousPlayerStatus = playerStatus;
			playerStatus = PlayerStatus.RunLeft;
		} else if (playerStatus == PlayerStatus.LeftWall) {
			bounce = true;
			velocity.x = 1.3f * speed;
			//resolve player status (continuing the momentum)
			previousPlayerStatus = playerStatus;
			playerStatus = PlayerStatus.RunRight;
		}
		//jump
		if(playerStatus == PlayerStatus.Jumped && grounded){
			float jumpMultiplier = Mathf.Abs(velocity.x) * 1.5f;
			velocity.y = jumpF + jumpMultiplier;
		}  
		//update player velocity
		player.velocity = velocity;      
	}

	void checkPlayerSpeedBoost(){		
		if ((playerStatus == PlayerStatus.RunRight && previousPlayerStatus == PlayerStatus.RunLeft) ||
			(playerStatus == PlayerStatus.RunLeft && previousPlayerStatus == PlayerStatus.RunRight) ||
			playerStatus == PlayerStatus.Idle) {			
			boostCounter = 0;
		} 	
		boostCounter++;	
		speedBoostActive = (boostCounter > speedBoost);
	}

	void checkGameOver(){
		Vector3 pos = Camera.main.WorldToViewportPoint (player.position);
		if (pos.y < 0) {
			gc.PlayerDied();
		}
	}

	//executes ONCE when entering a collision with another object
	private void OnCollisionEnter2D(Collision2D other){
		Rigidbody2D rb = other.collider.GetComponent<Rigidbody2D>();
		if ((other.gameObject.tag == "Platform" || other.gameObject.tag == "Checkpoint")  && other.relativeVelocity.y >= 0f){
			grounded = true; 
		}
		if(other.gameObject.tag =="Left Wall"){			
			touchLeftWall = true;
		}
		if(other.gameObject.tag =="Right Wall"){
			touchRightWall = true;
		}
	}
		
	//executes ONCE when exiting a collision with another object
	private void OnCollisionExit2D(Collision2D other){
		if ((other.gameObject.tag == "Platform" || other.gameObject.tag == "Checkpoint") && other.relativeVelocity.y >= 0f){
			grounded = false;
			gc.addScore(platformScore);
		}
		if (other.gameObject.tag == "Left Wall"){
			touchLeftWall = false;
			gc.addScore(wallScore);
		}
		if (other.gameObject.tag == "Right Wall"){
			touchRightWall = false;
			gc.addScore (wallScore);
		}
	}
		
	// Need to do as trigger since it should be enough to just pass it, not land on it.
	private void OnTriggerEnter2D(Collider2D other) { 
		if (other.gameObject.tag == "Checkpoint") {
			gc.addScore (checkpointScore);
		}
	}
}