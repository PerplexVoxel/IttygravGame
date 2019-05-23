using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public Transform[] Waypoints;
    private int index = 0;
    public float Speed = 10f;
    public float WaitTime = 2f;
    private float freezeTime = float.NegativeInfinity;
    private bool moving = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(moving){
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[index].position, Speed * Time.deltaTime);

            if(Vector3.Distance(transform.position, Waypoints[index].position) < 0.1){
                moving = false;
                index = (index + 1) % Waypoints.Length;
                freezeTime = Time.fixedTime;
            }
        }else if(freezeTime + WaitTime < Time.fixedTime){
            moving = true;
        }
	}
}
