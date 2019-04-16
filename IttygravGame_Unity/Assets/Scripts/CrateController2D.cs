using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateController2D : MonoBehaviour {

    [SerializeField]
    private CrateParameters2D DefaultParameters;

    public CrateParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }

    private CrateParameters2D _overrideParameters;

    public float DeltaMoveCoefficient = 1.5f;

    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody2D>().mass = Parameters.Mass;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().AddForce(Parameters.GravityCoefficient* Parameters.Mass *Parameters.Gravity );
    }

    public Vector2 Move(Vector2 deltaMovement, float massOfImpact, Vector2 normalForce)
    {
        //Do momentum calculation and check for box or wall collisions
        //Return the deltaMovement to set displacement of all touching objects.
        //If touching a wall, return 0
        transform.position += (Vector3)deltaMovement;
        //Vector2 myDeltaMovement = deltaMovement * DeltaMoveCoefficient / Time.deltaTime;
        //Debug.Log(deltaMovement.x);

        //GetComponent<Rigidbody2D>().velocity = myDeltaMovement;
        GetComponent<Rigidbody2D>().AddForce(normalForce );

        return deltaMovement;
    }
}
