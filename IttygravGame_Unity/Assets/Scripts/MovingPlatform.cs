﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public Transform[] Waypoints;
    private int index = 0;
    public float Speed = 10f;
    public float WaitTime = 2f;
    private float freezeTime = float.NegativeInfinity;
    private bool moving = true;

    private List<Transform> collidingObjects = new List<Transform>();
    private List<Transform> handledCollidingObjects = new List<Transform>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(moving){
            Vector3 displacement = Vector3.MoveTowards(transform.position, Waypoints[index].position, Speed * Time.deltaTime) - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[index].position, Speed * Time.deltaTime);
            checkCollidingObjects(collidingObjects, displacement);
            //for (int i = 0; i < collidingObjects.Count; i += 1){
            //    collidingObjects[i].position += displacement;
            //}
            if(Vector3.Distance(transform.position, Waypoints[index].position) < 0.1){
                moving = false;
                index = (index + 1) % Waypoints.Length;
                freezeTime = Time.fixedTime;
            }
        }else if(freezeTime + WaitTime < Time.fixedTime){
            moving = true;
        }

        collidingObjects.Clear();
        handledCollidingObjects.Clear();
	}

    private void checkCollidingObjects(List<Transform> collided, Vector3 displacement){
        for (int i = 0; i < collided.Count; i+= 1){
            if(!handledCollidingObjects.Contains(collided[i])){
                
                collided[i].position += displacement;
                handledCollidingObjects.Add(collided[i]);
                checkCollidingObjects(collided[i].GetComponent<CrateController2D>().GetCollidingObject(), displacement);
            }
        }
    }

	private void OnCollisionStay2D(Collision2D collision)
	{
        if(collision.transform.CompareTag("Box")){
            collidingObjects.Add(collision.transform);
        }
	}
}
