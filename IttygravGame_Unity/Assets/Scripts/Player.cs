using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private bool _isFacingRight;
	private CharacterController2D _controller;
	private float _normalizedHorizontalSpeed;

	public float MaxSpeed = 8;
	public float SpeedAccelerationOnGround = 10f;
	public float SpeedAccelerationInAir = 5f;

	public bool IsDead { get; private set; }

    private CrateController2D pushingCrate;

	public void Awake()
	{
		_controller = GetComponent<CharacterController2D> ();
		_isFacingRight = transform.localScale.x > 0; 			//assuming right is default
	}

	public void Update()
	{
		if(!IsDead)
			HandleInput ();

		var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

		// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
		if (IsDead)
			_controller.SetHorizontalForce (new Vector2(0,0));
		else
			_controller.SetHorizontalForce (new Vector2(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor), 0));
	}

	public void Kill(){
		_controller.HandleCollisions = false;
		GetComponent<Collider2D>().enabled = false;
		IsDead = true;

		_controller.SetForce(new Vector2(0, 20f));
	}

	public void RespawnAt(Transform spawnPoint){
		if (!_isFacingRight)
			Flip ();

		IsDead = false;
		_controller.HandleCollisions = true;
		GetComponent<Collider2D>().enabled = true;

		transform.position = spawnPoint.position;
	}

	private void HandleInput(){
		if (Input.GetKey (KeyCode.D)) {
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight) {
				Flip ();
			}

		} else if (Input.GetKey (KeyCode.A)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight) {
				Flip ();
			}
		} else {
			_normalizedHorizontalSpeed = 0;
		}

		if (_controller.CanJump && Input.GetKey (KeyCode.Space)) {
			_controller.Jump();
		}
	}

	private void Flip(){
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}

    private void HandlePlatformRotation()
    {

    }
}
