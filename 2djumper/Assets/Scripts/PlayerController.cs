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
	public float baseSpeed = 6f; //maximum 
	public float friction = 0.7f;
	public TextMeshProUGUI scoreText;
	public GameControl gc;
	public int platformScore = 5;
	public int wallScore = 10;
	public int checkpointScore = 50;

	//private declarations
	private int boostCounter = 0;
	private int speedBoost = 100; // if counter above we will increase speed
	private int score = 0;
	private bool touchRightWall =false;
	private bool touchLeftWall = false;
	private bool grounded = false;
	private bool speedBoostActive = false;

	void Start (){
		//initialize the player and get its rigidbody
		player = GetComponent<Rigidbody2D>();		       
		playerStatus = PlayerStatus.Idle;
		previousPlayerStatus = playerStatus;
	}

	private void Update(){	}

	void FixedUpdate(){		
		//Handle user inputs
		getPlayerEvent();    
		//Update character based on inputs
		movePlayer();
		//Check for if player fell off the screen
		checkGameOver();    
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
			} else {
				playerStatus = PlayerStatus.RunLeft;
			}
		} 

		if(Input.GetAxis ("Horizontal") == 0) {
			playerStatus = PlayerStatus.Idle;
		}

		//jumping
		if (Input.GetKey(KeyCode.Space) && grounded)
		{
			print ("jumped");
			playerStatus = PlayerStatus.Jumped;
		}
	}

	void movePlayer(){
		//read the player velocity
		Vector2 velocity = player.velocity; 
		checkPlayerSpeedBoost ();

		if(playerStatus == PlayerStatus.Idle){			
			velocity.x = 0; //handles slowing down
		}
		if(playerStatus == PlayerStatus.RunRight){			
			if (speedBoostActive) {
				velocity.x = 2 * baseSpeed;
			} else {
				velocity.x = baseSpeed;
			}
		}else if(playerStatus == PlayerStatus.RunLeft){
			if (speedBoostActive) {
				velocity.x = -2 * baseSpeed;
			} else {
				velocity.x = -baseSpeed;
			}
		}
		if(playerStatus == PlayerStatus.Jumped){
			float jumpMultiplier = Mathf.Abs(velocity.x);
			velocity.y = jumpF + jumpMultiplier;
		}  
		//update the player velocity
		player.velocity = velocity;      
	}

	void checkPlayerSpeedBoost(){		
		if ((playerStatus == PlayerStatus.RunRight && previousPlayerStatus == PlayerStatus.RunLeft) || (playerStatus == PlayerStatus.RunLeft && previousPlayerStatus == PlayerStatus.RunRight)) {			
			boostCounter = 0;
		} 
		print (speedBoostActive);
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



/*unused functions
	void updateScore()	{
		Vector2 position = player.position;
		float platformDistance = 2.5f; //distance between platforms as found in LevelGenerator
		float nextPlatformPosition = (score + 1) * platformDistance;
		if(position.y > nextPlatformPosition){ 
			score++;
		}
		scoreText.SetText("Score: " + score.ToString());      
	}
*/