using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCollider : MonoBehaviour {
    public float collisionDistance;
    private float x; //These represent geometric calculations made on the object
    private float y; // ^^                                                  ^^
    private Vector3[,] CooridinateMatrix; //This holds the transformations for the collider

	// Use this for initialization
	void Start () {
        x = this.GetComponent<BoxCollider2D>().size.y / (2 + Mathf.Sqrt(2));
        y = this.GetComponent<BoxCollider2D>().size.y - ((2 * this.GetComponent<BoxCollider2D>().size.y) / (2 + Mathf.Sqrt(2)));

        CooridinateMatrix = new Vector3[16,2]; //[x,y] x = position, y:0 = origin of ray, y:1 = direction of ray
        
        CooridinateMatrix[0, 0] = new Vector3(-(y / 2) - (x / 3), -(y / 2) - (x * (2 / 3)));
        CooridinateMatrix[0, 1] = new Vector3(-(y / 2) - (x / 3) - collisionDistance, -(y / 2) - (x * (2 / 3)) - collisionDistance);

        CooridinateMatrix[1, 0] = new Vector3(-(y / 6), -(y / 2) - x);
        CooridinateMatrix[1, 1] = new Vector3(-(y / 6), -(y / 2) - x - collisionDistance);

        CooridinateMatrix[2, 0] = new Vector3((y / 6), -(y / 2) - x);
        CooridinateMatrix[2, 1] = new Vector3((y / 6), (-(y / 2) - x  -collisionDistance));

        CooridinateMatrix[3, 0] = new Vector3((y / 2) + (x / 3), -(y / 2) - (x * (2 / 3)));
        CooridinateMatrix[3, 1] = new Vector3((y / 2) + (x / 3) + collisionDistance, -(y / 2) - (x * (2 / 3)) - collisionDistance);

        CooridinateMatrix[4, 0] = new Vector3((y / 2) + (x * (2 / 3)), -(y / 2) - (x / 3));
        CooridinateMatrix[4, 1] = new Vector3((y / 2) + (x * (2 / 3)) + collisionDistance, -(y / 2) - (x / 3) - collisionDistance);

        CooridinateMatrix[5, 0] = new Vector3((y / 2) + x, -(y / 6));
        CooridinateMatrix[5, 1] = new Vector3((y / 2) + x + collisionDistance, -(y / 6));

        CooridinateMatrix[6, 0] = new Vector3((y / 2) + x, (y / 6));
        CooridinateMatrix[6, 1] = new Vector3((y / 2) + x + collisionDistance, (y / 6));

        CooridinateMatrix[7, 0] = new Vector3((y / 2) + (x * (2 / 3)), (y / 2) + (x / 3));
        CooridinateMatrix[7, 1] = new Vector3((y / 2) + (x * (2 / 3)) + collisionDistance, (y / 2) + (x / 3) + collisionDistance);

        CooridinateMatrix[8, 0] = new Vector3((y / 2) + (x / 3), (y / 2) + (x * (2 / 3)));
        CooridinateMatrix[8, 1] = new Vector3((y / 2) + (x / 3) + collisionDistance, (y / 2) + (x * (2 / 3)) + collisionDistance);

        CooridinateMatrix[9, 0] = new Vector3((y / 6), (y / 2) + x);
        CooridinateMatrix[9, 1] = new Vector3((y / 6), (y / 2) + x + collisionDistance);

        CooridinateMatrix[10, 0] = new Vector3(-(y / 6), (y / 2) + x);
        CooridinateMatrix[10, 1] = new Vector3(-(y / 6), (y / 2) + x + collisionDistance);

        CooridinateMatrix[11, 0] = new Vector3(-(y / 2) - (x / 3), (y / 2) + (x * (2 / 3)));
        CooridinateMatrix[11, 1] = new Vector3(-(y / 2) - (x / 3) - collisionDistance, (y / 2) + (x * (2 / 3)) + collisionDistance);

        CooridinateMatrix[12, 0] = new Vector3(-(y / 2) - (x * (2 / 3)), (y / 2) + (x / 3));
        CooridinateMatrix[12, 1] = new Vector3(-(y / 2) - (x * (2 / 3)) - collisionDistance, (y / 2) + (x / 3) + collisionDistance);

        CooridinateMatrix[13, 0] = new Vector3(-(y / 2) - x, (y / 6));
        CooridinateMatrix[13, 1] = new Vector3(-(y / 2) - x - collisionDistance, (y / 6));

        CooridinateMatrix[14, 0] = new Vector3(-(y / 2) - x, -(y / 6));
        CooridinateMatrix[14, 1] = new Vector3(-(y / 2) - x - collisionDistance, -(y / 6));

        CooridinateMatrix[15, 0] = new Vector3(-(y / 2) - (x * (2 / 3)), -(y / 2) - (x / 3));
        CooridinateMatrix[15, 1] = new Vector3(-(y / 2) - (x * (2 / 3)) - collisionDistance, -(y / 2) - (x / 3) - collisionDistance);

    }

    // Update is called once per frame
    private void FixedUpdate()
    {



        //Debug.DrawRay(this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[2, 0], this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[2, 1], Color.blue);
        Debug.DrawRay(this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[0, 0], this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[0, 1], Color.red);



    }

}
