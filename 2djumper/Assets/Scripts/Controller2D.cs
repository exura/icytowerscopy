using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script inherits from RayCastController

public class Controller2D : RayCastController {

	// define a maximum slope to climb up
	float maxClimbAngle = 80f;
	// define a maximum slope to climb down
	float maxDescendAngle = 80f;

	// holds a reference to the collisions, see struct below
	public CollisionInfo collisions;

	// holds information about player input
	Vector2 playerInput;

	// use RayCastController start
	public override void Start() {
		base.Start ();
	}

	// since other classes might want to use this script but not with playerinput the player input is zeroed
	public void Move(Vector3 velocity, bool standingOnPlatform) {
		Move (velocity, Vector2.zero, standingOnPlatform);
	}

	// controls move and uses player input
	public void Move (Vector3 velocity, Vector2 input, bool standingOnPlatform = false) {
		// first update the raycastorigins to the position of the box
		UpdateRaycastOrigins ();
		// reset all previous collision info
		collisions.Reset ();
		// update the "old" velocity
		collisions.velocityOld = velocity;
		// assign the player input
		playerInput = input;

		// if the velocity is negative the box might descend a slope
		if (velocity.y < 0) {
			DescendSlope (ref velocity); // refer to the velocity being passed in
		}
		// if moving left or right the box might collide into something horizontally
		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		// if moving up or down the box might collide into something vertically
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}
		// move the box with the velocity
		transform.Translate (velocity);

		// if standing on a moving platform the platform will tell the controller that the player is standing on it, 
		// but since it might be moving down, then it's not always sure that jump will work, unless it is specified
		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	// handles horizontal collisions, take in the velocity
	void HorizontalCollisions(ref Vector3 velocity) {
		// find the direction of the velocity
		float directionX = Mathf.Sign (velocity.x);
		// check only as far out as the velocity is indicating we are moving, plus one skinWidth so we at least look outside of the box
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		// then for as many rays as specified
		for (int i = 0; i < horizontalRayCount; i++) {
			// set the rayorigin either at the left side, if going left (directionX == -1), or right side, if going right
			Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight;
			// make sure the rays are placed up vertically by the spacing and i
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			// save the hit object from the raycast, going from origin, shooting in the direction, given the rayLength, and only give hits for objects with our collisionmask
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			// draw the rays to see that they are working (can be deleted)
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			// if we have it something we can collide with
			if (hit) {

				// if already inside the other object, skip the collision)
				if (hit.distance == 0) {
					continue;
				}

				// calculate the angle to what we have hit by using the normal of the object we have hit, and the global up
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				// a slope will always be found looking at the lowest raycast (bottomleft or bottomright), otherwise it's not a slope
				// if the angle is higher than maxClimbAngle the box cannot climb it
				if (i == 0 && slopeAngle <= maxClimbAngle) {
					// if we were on a descending slope last frame
					if (collisions.descendingSlope) {
						// set the descending slope to false, it will be reset to true in the next move if we are still on the slope
						collisions.descendingSlope = false;
						// cornercase if we meet an ascending slope
						velocity = collisions.velocityOld;
					}

					// handle if we have less than velocity until slope starts
					float distanceToSlopeStart = 0f;
					// if we have a slope with different angles
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					// climb the new slope
					ClimbSlope (ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				// if not climbing a slope or we meet a slope higher with too high angle, we move next to it
				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;

				}
			}
		}
	}

	// handles vertical collisions, takes in velocity
	void VerticalCollisions(ref Vector3 velocity) {
		// get direction of the new velocity
		float directionY = Mathf.Sign (velocity.y);
		// how far do we need to shoot the rays
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		// for each ray
		for (int i = 0; i < verticalRayCount; i++) {
			// takes in the direction of the velocity and places rays either on up or bottom of the box
			Vector2 rayOrigin = (directionY == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.topLeft;
			// place them out given the new position (velocity.x)
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			// save the result in hit
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			// draw them (can be deleted later)
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			// if hit something
			if (hit) {

				// if we want to be able to move through a platform, it should have the tag "Through"
				if (hit.collider.tag == "Through") {
					// if we move up or are inside the other object already, skip the collision
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}
					// if we are "falling" down through the platfrom
					if (collisions.fallingThroughPlatform) {
						continue;
					}
					// if player wants to fall down through the platform
					if (playerInput.y == -1) {
						// then we have set the collosions to fall through to true
						collisions.fallingThroughPlatform = true;
						// it's reset after 0.25 seconds because otherwise the player will have a problem falling through
						Invoke ("ResetFallingThroughPlatform", 0.25f);
						continue;
					}
				}

				// make sure we can't move into the object
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				// if climbing slope use this
				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
				}

				// mark that we have collided with something
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		// if climbing a slope
		if (collisions.climbingSlope) {
			// get direction and shoot a new raycast
			float directionX = Mathf.Sign (velocity.x);
			rayLength = Mathf.Abs (velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight) + Vector2.up * velocity.y;

			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			// if hit something make sure we can move up
			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				// cornercase for new slope
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	// if climbing a slope want to make sure tha twe move with the real velocity and not slower velocity
	void ClimbSlope (ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;

		}
	}

	// if descending slope we want to keep the player attached to the ground
	void DescendSlope (ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.bottomRight : rayCastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign (hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x)) {
						float moveDistance = Mathf.Abs (velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;
						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	// resets falling through platform
	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	// struct for collisions
	public struct CollisionInfo {

		// if collided with something above or below
		public bool above, below;
		// if collided with something left or right
		public bool left, right;
		// if climbing or descending a slope
		public bool climbingSlope, descendingSlope;
		// holding the angle of the slope, and the old angle of the slope (in case we meet a slope with different angles)
		public float slopeAngle, slopeAngleOld;
		// holds the old velocity
		public Vector3 velocityOld;
		// if we are currently falling through a platform
		public bool fallingThroughPlatform;
		// resets the values
		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}