using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerScript : MonoBehaviour {
    public Transform PlayerRespawnPoint;

    private Player HitPlayer;
    private bool hasPlayer = false;

    public float FreezeTime = 1.0f;
    private float frozeTime = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (hasPlayer)
        {
            if(Time.fixedTime > frozeTime + FreezeTime)
            {
                HitPlayer.transform.position = PlayerRespawnPoint.position;
                HitPlayer.PositionFrozen = false;
                hasPlayer = false;
            }else if(Vector3.Distance(transform.position, HitPlayer.transform.position) < 0.3)
            {
                HitPlayer.PositionFrozen = true;
            }
        }
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(collision.name == "Center" && !hasPlayer)
            {
                HitPlayer = collision.gameObject.GetComponent<Player>();
                HitPlayer.PositionFrozen = true;
                hasPlayer = true;
                frozeTime = Time.fixedTime;
            }else if (collision.name == "Center")
            {
                HitPlayer.PositionFrozen = true;
            }
            else
            {
                HitPlayer = collision.gameObject.GetComponent<Player>();
                //HitPlayer.PositionFrozen = true;
                hasPlayer = true;
                frozeTime = Time.fixedTime;
            }
            

            //
        }
    }
}
