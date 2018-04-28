using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script uses the controller2d script, therefore requires it automatically

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {

	// the move speed
	public float moveSpeed = 6;
	// the jumpHeight
	float jumpHeight = 4;
	// how long time it should take to reach the jump heigh
	public float timeToJumpApex = 0.4f;
	// how long time it takes to reach the moveSpeed in air
	float accelerationTimeAirborne = 0.2f;
	// how long time it takes to reach the moveSpeed if on ground
	float accelerationTimeGrounded = 0.2f;


	//public float wallSlideSpeedMax = 3;

	// the gracity (is calculated below using jumpHeight and timeToJumpApex
	float gravity;
	// the velocity (is calculated below using the gracity and the timeToJumpApex
	float jumpVelocity;
	// the velocity
	Vector3 velocity;
	// the smoother velocity (causes some acceleration to moveSpeed)
	float velocityXSmoothing;

	// reference to the controller
	Controller2D controller;

	// Use this for initialization
	void Start () {
		// set the controller to the component (that is required)
		controller = GetComponent<Controller2D> ();

		// calculate the gravity and timeToJumpApex
		gravity = -(2* jumpHeight) / Mathf.Pow(timeToJumpApex, 2);

		// calculate the needer jumpvelocity given the gravity and timeToJumpApex
		jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);

		// debug so it works
		print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);

	}

	// Update is called once per frame
	void Update () {

		/*bool wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;
			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}
		}*/


		// get input from the the keys
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		// if jump button is pressed and the player is on the ground
		if (Input.GetButtonDown ("Jump") && controller.collisions.below) {
			// apply the jumpvelocity
			velocity.y = jumpVelocity;
		}

		// since there should be some acceleration up to movespeed we have to smoothen the run
		// first check what the targetvelocity should be
		float targetVelocityX = input.x * moveSpeed;
		// apply the velocity using a damp-function (also check if grounded or airborne, so we can apply two different accelerations
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)? accelerationTimeGrounded : accelerationTimeAirborne);
		// apply the gravity
		velocity.y += gravity * Time.deltaTime;
		// and apply the velocity
		controller.Move (velocity * Time.deltaTime, input);
		// if player hit the ceiling or floor set velocity.y to 0
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}
}