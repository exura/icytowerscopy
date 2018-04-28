using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will create raycasts given the boxcollider attached to the object
// Therefore it requires a box-collider

[RequireComponent (typeof(BoxCollider2D))]
public class RayCastController : MonoBehaviour {

	// What objects it can collide with
	public LayerMask collisionMask;

	// skindwidth of the sprite
	public const float skinWidth = .015f;

	// how many rays will shoot out of the left/right of the box
	public int horizontalRayCount = 4;

	// how many rays will shoot out of the top/bottom of the box
	public int verticalRayCount = 4;

	// how the rays are spaced
	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	// referemce tp the collider
	[HideInInspector]
	public BoxCollider2D collider;

	// reference to the origins of the raycasts, see struct below
	public RaycastOrigins rayCastOrigins;

	// Use this for initialization which means that other classes can use this start and their own start as well (virtual void)
	public virtual void Start () {

		// get collider component when box is created
		collider = GetComponent<BoxCollider2D> ();

		// calculate the spacing of the rays
		CalculateRaySpacing ();
	}


	public void UpdateRaycastOrigins() {
		// get the bounds of the box collider
		Bounds bounds = collider.bounds;
		// start shooting out the rays one skinwidth inside the box (therefore subtract two skinwidths, one for the actual skinwidth and one to be inside)
		bounds.Expand (skinWidth * -2);

		// make sure to place the bottom left ray in the bottom left corner (min x and min y)
		rayCastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		// place the bottom right ray to the right, but still at the bottom (max x and min y)
		rayCastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		// and so on ...
		rayCastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		// and so on ...
		rayCastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing() {
		// get the bounds of the box collider
		Bounds bounds = collider.bounds;
		// start shooting out the rays one skinwidth inside the box (therefore subtract two skinwidths, one for the actual skinwidth and one to be inside)
		bounds.Expand (skinWidth * -2);

		// make sure we don't have more rays than int can store
		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		// place the rays equidistant from the boundsizes
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}