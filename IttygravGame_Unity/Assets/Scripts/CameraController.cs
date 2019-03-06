using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject Player;
    public float RotationSmoothing = 10;
    public float FollowSmoothing = 10;
    public Vector2 FollowOffset;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //move towards player
        float cameraAngle = transform.eulerAngles.z * Mathf.PI /180;
        float xOffset = Mathf.Cos(cameraAngle) * FollowOffset.x + Mathf.Cos(cameraAngle + Mathf.PI / 2) * FollowOffset.y;
        float yOffset = Mathf.Sin(cameraAngle) * FollowOffset.x + Mathf.Sin(cameraAngle + Mathf.PI / 2) * FollowOffset.y;
        Vector3 newCameraPosition = new Vector3(Player.transform.position.x + xOffset, Player.transform.position.y + yOffset, transform.position.z);
        transform.position = Vector3.Lerp(newCameraPosition, transform.position, FollowSmoothing * Time.deltaTime);

        //rotate camera
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Player.transform.rotation, RotationSmoothing * Time.deltaTime);
        
        
	}
}
