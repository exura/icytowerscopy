using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI... probably not needed?
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro stuff

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
	//struct
	public struct CollisionInfo{
		public bool leftWall, rightWall, ground;

		public void reset(){
			leftWall = false;
			rightWall = false;
			ground = false;
		}
	}
	public CollisionInfo collisionInfo;
	//enumration class
	enum PlayerStatus {RunLeft, RunRight, Jumped, Idle};
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
	public float bounceTime = 1f;

	//private declarations
	private int boostCounter = 0;
	private int speedBoost = 100; // if counter above we will increase speed
	private int score = 0;
	private int life = 3;
	private float bounceCounter = 0;
	private bool bounce = false;

	private bool speedBoostActive = false;


	void Start (){
		//initialize the player and get its rigidbody
		player = GetComponent<Rigidbody2D>();
		collisionInfo.reset();
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
		bounceCounter+= Time.deltaTime;
		if (bounceCounter > bounceTime || collisionInfo.ground) { //stop bounce if player on platform
			bounce = false;
			bounceCounter = 0;
		}
	}
	
	void getPlayerEvent(){
		//movement left to right or idle
		previousPlayerStatus = playerStatus;
		if (Input.GetAxisRaw("Horizontal") > 0) {			
			playerStatus = PlayerStatus.RunRight;
		} else if (Input.GetAxisRaw("Horizontal") < 0) {
			playerStatus = PlayerStatus.RunLeft;
		} else{				
			playerStatus = PlayerStatus.Idle;
		}
		//jumping
		if (Input.GetKey(KeyCode.Space) && collisionInfo.ground){			
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
		if (collisionInfo.rightWall) {
			bounce = true;
			velocity.x = -1.3f * speed;
			//resolve player status (continuing the momentum)
			playerStatus = PlayerStatus.RunLeft;
			previousPlayerStatus = playerStatus;
		} else if (collisionInfo.leftWall) {
			bounce = true;
			velocity.x = 1.3f * speed;
			//resolve player status (continuing the momentum)
			playerStatus = PlayerStatus.RunRight;
			previousPlayerStatus = playerStatus;
		}
		//jump
		if(playerStatus == PlayerStatus.Jumped && collisionInfo.ground){
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
		print (pos);
		if (pos.y < 0) {
			life--;
			if (life <= 0) {
				gc.PlayerDied ();
			} else {
				print (life);
				Vector2 position = player.position;
				position.y = position.y + 10f;
				player.position = position;
			}
		}
	}

	//executes ONCE when entering a collision with another object
	private void OnCollisionEnter2D(Collision2D other){
		Rigidbody2D rb = other.collider.GetComponent<Rigidbody2D>();
		if ((other.gameObject.tag == "Platform" || other.gameObject.tag == "Checkpoint")  && other.relativeVelocity.y >= 0f){
			collisionInfo.ground = true; 
		}
		if(other.gameObject.tag =="Left Wall"){			
			collisionInfo.leftWall = true;
		}
		if(other.gameObject.tag =="Right Wall"){
			collisionInfo.rightWall = true;
		}
	}
		
	//executes ONCE when exiting a collision with another object
	private void OnCollisionExit2D(Collision2D other){
		if ((other.gameObject.tag == "Platform" || other.gameObject.tag == "Checkpoint") && other.relativeVelocity.y >= 0f){			
			gc.addScore(platformScore);
			collisionInfo.ground = false;
		}
		if (other.gameObject.tag == "Left Wall"){
			boostCounter += 40;
			gc.addScore(wallScore);
			gc.bonusTimer (true);
			collisionInfo.leftWall = false;
		}
		if (other.gameObject.tag == "Right Wall"){
			boostCounter += 40;
			gc.addScore(wallScore);
			gc.bonusTimer (true);
			collisionInfo.rightWall = false;
		}

	}
		
	// Need to do as trigger since it should be enough to just pass it, not land on it.
	private void OnTriggerEnter2D(Collider2D other) { 
		if (other.gameObject.tag == "Checkpoint") {
			gc.addScore (checkpointScore);
		}
	}

}