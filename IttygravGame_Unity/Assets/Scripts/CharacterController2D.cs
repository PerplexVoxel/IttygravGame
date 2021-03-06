﻿using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour 
{
	private const float SkinWidth = 0.02f;
	private const int TotalHorizontalRays = 8;
	private const int TotalVerticalRays = 4;

	private static readonly float SlopeLimitTangent = Mathf.Tan (75f * Mathf.Deg2Rad);

	public LayerMask PlatformMask; 
	public ControllerParameters2D DefaultParameters;

	public ControllerState2D State { get; private set; }
	public Vector2 Velocity { get{ return _velocity; } }
	public bool CanJump 
	{ 
		get 
		{ 
			if(Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpAnywhere)
				return _jumpIn <= 0;

			if(Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpOnGround)
				return State.IsGrounded;

			return false;
		} 
	}

	public bool HandleCollisions { get; set; }
	public ControllerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }
	public GameObject StandingOn{ get; private set; }
	public Vector3 PlatformVelocity { get; private set; }

	private Vector2 _velocity;
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private ControllerParameters2D _overrideParameters;
	private float _jumpIn;
	private GameObject _lastStandingOn;

    public bool MotionFrozen = false;

	private Vector3
		_activeGlobalPlatformPoint,
		_activeLocalPlatformPoint;

	private Vector3
		_raycastTopLeft,
		_raycastBottomRight,
		_raycastBottomLeft;

	private float
		_verticalDistanceBetweenRays,
		_horizontalDistanceBetweenRays;

	public void Awake()
	{
		HandleCollisions = true;
		State = new ControllerState2D ();
		_transform = transform;
		_localScale = transform.localScale;
		_boxCollider = GetComponent<BoxCollider2D> ();

		var colliderWidth = _boxCollider.size.x * Mathf.Abs (transform.localScale.x) - (2 * SkinWidth);
		_horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

		var colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);

	}
	public void AddForce(Vector2 force)
	{
		_velocity += force;
	}
	public void SetForce(Vector2 force)
	{
		_velocity = force;
	}
	public void SetHorizontalForce(Vector2 right)
	{
		_velocity.x = right.x;
        
    }
	public void SetVerticalForce(Vector2 up)
	{
		_velocity.y = up.y;
        
    }
	public void Jump()
	{
        AddForce ( new Vector2 (0, Parameters.JumpMagnitude));
		_jumpIn = Parameters.JumpFrequency;
	}
	public void LateUpdate()
	{
		_jumpIn -= Time.deltaTime;

        //float gravity = 2.5f * Parameters.Mass * mapToPlayerOrientation(Parameters.Gravity).y * Time.deltaTime;
        float gravity = Parameters.GravityCoefficient * Parameters.Mass * Parameters.Gravity.y * Time.deltaTime;
        _velocity.y += gravity;
        //Debug.Log("Gravity: " + gravity);
        //_velocity.y += -25 * Time.deltaTime;

        if(!MotionFrozen) Move (Velocity * Time.deltaTime);
	}

    float xx = 1;
    float xy = 0;
    float yx = 0;
    float yy = 1;
    float lastPlayerAngle = 0;
    public void RotatePlayer(float rotationAngle)
    {
        transform.eulerAngles = new Vector3(0, 0, rotationAngle);
    }

    private Vector2 mapToPlayerOrientation(Vector3 adjustingVector)
    {
        if (transform.eulerAngles.z != lastPlayerAngle)
        {
            float angle = transform.eulerAngles.z;
            float angle2 = Mathf.PI * (angle + 90) / 180;
            angle = angle * Mathf.PI / 180;

            xx = Mathf.Cos(angle);
            xy = Mathf.Sin(angle);
            yx = Mathf.Cos(angle2);
            yy = Mathf.Sin(angle2);

            lastPlayerAngle = transform.eulerAngles.z;
        }


        return new Vector2(adjustingVector.x * xx + adjustingVector.y * yx, adjustingVector.x * xy + adjustingVector.y * yy);

    }

    public Vector2 Move(Vector2 deltaMovement, float massOfImpact)
    {
        Move(deltaMovement);
        return deltaMovement; //needs to be updated with momentum calculations
    }

    private void Move(Vector2 deltaMovement)
	{
		var wasGrounded = State.IsCollidingBelow;
		State.Reset ();

		if (HandleCollisions) {
			
			CalculateRayOrigins();
 
            if (Mathf.Abs(deltaMovement.x) > 0.001f)
            {
                MoveHorizontally(ref deltaMovement);
            }

            
            MoveVertically(ref deltaMovement);

            //CorrectHorizontalPlacement(ref deltaMovement, true);
            //CorrectHorizontalPlacement(ref deltaMovement, false);
        }

        
        _transform.Translate( mapToPlayerOrientation(deltaMovement), Space.World);
      

        if (Time.deltaTime > 0)
			_velocity = deltaMovement / Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);



  
		if (StandingOn != null) {
			_activeGlobalPlatformPoint = transform.position;
			_activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);

//			Debug.DrawLine(transform.position, _activeGlobalPlatformPoint);
//			Debug.DrawLine(transform.position, _activeLocalPlatformPoint);

			if(_lastStandingOn != StandingOn){
				if(_lastStandingOn != null)
					_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

				StandingOn.SendMessage ("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = StandingOn;
			}
			else if(StandingOn != null){
				StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);

			}else if(_lastStandingOn != null){
				_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = null;
			}
		}


	}
	

	private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight){
		var halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
		var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

		if (isRight) {
			
            rayOrigin = rayOrigin - (Vector3) mapToPlayerOrientation(new Vector2(halfWidth - SkinWidth, 0));
        }
        else
        {
           
            rayOrigin = rayOrigin + (Vector3)mapToPlayerOrientation(new Vector2(halfWidth - SkinWidth, 0));
        }
			

		var rayDirection = isRight ? transform.right : -transform.right;
		var offset = 0f;
        

		for (var i = 1; i < TotalHorizontalRays -1; i++){
            
            var rayVector = (Vector2)rayOrigin + mapToPlayerOrientation(deltaMovement) + mapToPlayerOrientation(new Vector2(0, i * _verticalDistanceBetweenRays));

            //Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
			if(!raycastHit){
				continue;
			}

			offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
            
        }

		deltaMovement.x += offset;
	}

	private void CalculateRayOrigins()
	{
		var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		var center = new Vector2 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);							 //center = offset

		
        _raycastTopLeft = _transform.position + (Vector3)mapToPlayerOrientation( new Vector2(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth));
        _raycastBottomRight = _transform.position + (Vector3)mapToPlayerOrientation(new Vector2(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth));
        _raycastBottomLeft = _transform.position + (Vector3)mapToPlayerOrientation(new Vector2(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth));
    }

    

	private void MoveHorizontally(ref Vector2 deltaMovement)
	{
		var isGoingRight = deltaMovement.x > 0;

        var rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidth;
        //var rayDistance = deltaMovement
		var rayDirection = isGoingRight ? transform.right : - transform.right;
		var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

		for (var i = 0; i< TotalHorizontalRays; i++) {
			//var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y) + mapToPlayerOrientation(new Vector2(0, (i * _verticalDistanceBetweenRays)));

			Debug.DrawRay (rayVector, rayDirection*rayDistance, Color.red);

			var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);

			if(!rayCastHit)
				continue;

            
			
            Vector2 collisionDistance = new Vector2( Mathf.Sign(deltaMovement.x)*Vector3.Distance( new Vector2(rayCastHit.point.x, rayCastHit.point.y) , rayVector), 0);

            //Check for box collision
            if (rayCastHit.transform.gameObject.layer == LayerMask.NameToLayer("Box"))
            {
                //Debug.Log("Box collision");
                CrateController2D crate = rayCastHit.transform.gameObject.GetComponent<CrateController2D>();
                Vector2 playerNormal = -transform.up; // mapToPlayerOrientation(-transform.up);
                //Debug.Log(playerNormal.ToString());
                collisionDistance.x = Mathf.Sign(collisionDistance.x)*Vector3.Magnitude( crate.Move(mapToPlayerOrientation(new Vector2(deltaMovement.x, 0)), Parameters.Mass, Parameters.BoxNormalForce * playerNormal));

            }

            deltaMovement.x = collisionDistance.x;
            rayDistance = Mathf.Abs( deltaMovement.x);

			if(isGoingRight){
				deltaMovement.x -= SkinWidth;
				State.IsCollidingRight = true;
			}else{
				deltaMovement.x += SkinWidth;
				State.IsCollidingLeft = true;
			}

            

            if (rayDistance < SkinWidth + 0.0001f)
				break;
		}
	}
	private void MoveVertically(ref Vector2 deltaMovement)
	{
		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs (deltaMovement.y) + SkinWidth;
        //var rayDistance = Vector3.Magnitude(deltaMovement + (Vector2)(SkinWidth * transform.up));
        var rayDirection = isGoingUp ? transform.up : -transform.up;
		var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;

		

		
		for (var i = 0; i < TotalVerticalRays; i++) {
            //var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Vector2 rayVector = new Vector2(rayOrigin.x, rayOrigin.y) + mapToPlayerOrientation(new Vector2( (i * _horizontalDistanceBetweenRays), 0));// + (Vector2)_transform.position;
            Debug.DrawRay (rayVector, rayDirection * rayDistance, Color.green);

			var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			if (!raycastHit)
				continue;

            if(raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Platform") && !isGoingUp) CheckPlatform(raycastHit.transform);
            //CheckPlatform(raycastHit.transform);




            deltaMovement.y =  Mathf.Sign(deltaMovement.y)*Vector3.Distance(new Vector2(raycastHit.point.x, raycastHit.point.y) , rayVector);

            

			rayDistance = Mathf.Abs ( deltaMovement.y);

			if (isGoingUp){
				deltaMovement.y -= SkinWidth;
				State.IsCollidingAbove = true;
			}else{
				deltaMovement.y += SkinWidth;
				State.IsCollidingBelow = true;
			}

			

			if (rayDistance < SkinWidth + 0.002f) {
				break;
			}
		}
	}
	
    private void CheckPlatform(Transform platform)
    {
        //if (platform.eulerAngles.z != transform.eulerAngles.z)
        //{
        //    RotatePlayer(platform.eulerAngles.z);
        //}

        if (platform.tag == "Platform090") RotatePlayer(90);
        else if (platform.tag == "Platform135") RotatePlayer(135);
        else if (platform.tag == "Platform180") RotatePlayer(180);
        else if (platform.tag == "Platform225") RotatePlayer(225);
        else if (platform.tag == "Platform270") RotatePlayer(270);
        else if (platform.tag == "Platform315") RotatePlayer(315);
        else if (platform.tag == "Platform000") RotatePlayer(0);
        else if (platform.tag == "Platform045") RotatePlayer(45);
    }

	public void OnTriggerEnter2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D> ();
		if (parameters == null) {
			return;
		}

		_overrideParameters = parameters.Parameters;
	}
	public void OnTriggerExit2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D> ();
		if (parameters == null) {
			return;
		}

		_overrideParameters = null;
	}
}
