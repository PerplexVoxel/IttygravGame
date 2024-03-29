﻿using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ControllerParameters2D : Parameters2D
{

	public enum JumpBehavior{
		CanJumpOnGround,
		CanJumpAnywhere,
		CantJump
	}

	public Vector2 MaxVelocity = new Vector2 (float.MaxValue, float.MaxValue);
    public float BoxNormalForce = 10f;

	[Range(0,90)]
	public float SlopeLimit = 30;

	//public float Gravity = -25f;

	public JumpBehavior JumpRestrictions;

	public float JumpFrequency = 0.25f;

	public float JumpMagnitude = 12;

    //public float Mass = 10;
}
