﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCollider : MonoBehaviour {
    public float collisionDistance;
    private float x; //These represent geometric calculations made on the object
    private float y; // ^^                                                  ^^
    private Vector3[,] CooridinateMatrix; //This holds the transformations for the collider
    private int[] positionOffset = new int[] { 0, 1, 2, 3 };
	// Use this for initialization
	void Start () {
        calcRayMatrix();
        transition(1);
    }

    private void FixedUpdate()
    {

        Debug.DrawRay(this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[positionOffset[0], 0], CooridinateMatrix[positionOffset[0], 1], Color.red);
        Debug.DrawRay(this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[positionOffset[1], 0], CooridinateMatrix[positionOffset[1], 1], Color.red);
        Debug.DrawRay(this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[positionOffset[2], 0], CooridinateMatrix[positionOffset[2], 1], Color.red);
        Debug.DrawRay(this.GetComponent<Rigidbody2D>().transform.position + CooridinateMatrix[positionOffset[3], 0], CooridinateMatrix[positionOffset[3], 1], Color.red);
        
    }

    private void calcRayMatrix()
    {
        x = this.GetComponent<BoxCollider2D>().size.y / (2 + Mathf.Sqrt(2));
        y = this.GetComponent<BoxCollider2D>().size.y - ((2 * this.GetComponent<BoxCollider2D>().size.y) / (2 + Mathf.Sqrt(2)));
        float xOneThird = (y / 2f) + (x * (1f / 3f));
        float xTwoThird = (y / 2f) + (x * (2f / 3f));
        float xHalfY = x + (y / 2f);
        float yOneSixth = (y / 6);
        Debug.Log(x.ToString());
        Debug.Log(y.ToString());
        float collisionAdjusted = collisionDistance * Mathf.Sqrt(1f / 2f);

        CooridinateMatrix = new Vector3[16, 2]; //[x,y] x = position, y:0 = origin of ray, y:1 = direction of ray

        CooridinateMatrix[0, 0] = new Vector3(-xOneThird, -xTwoThird);
        CooridinateMatrix[0, 1] = new Vector3(/*-xOneThird*/ -collisionAdjusted, /*-xTwoThird*/ -collisionAdjusted);

        CooridinateMatrix[1, 0] = new Vector3(-yOneSixth, -xHalfY);
        CooridinateMatrix[1, 1] = new Vector3(0, /*-xHalfY*/ -collisionDistance);

        CooridinateMatrix[2, 0] = new Vector3(yOneSixth, -xHalfY);
        CooridinateMatrix[2, 1] = new Vector3(0, /*-xHalfY*/ -collisionDistance);

        CooridinateMatrix[3, 0] = new Vector3(xOneThird, -xTwoThird);
        CooridinateMatrix[3, 1] = new Vector3(/*xOneThird + */ collisionAdjusted, /*-xTwoThird */ -collisionAdjusted);

        CooridinateMatrix[4, 0] = new Vector3(xTwoThird, -xOneThird);
        CooridinateMatrix[4, 1] = new Vector3(/*xTwoThird + */ collisionAdjusted, /*-xOneThird */ -collisionAdjusted);

        CooridinateMatrix[5, 0] = new Vector3(xHalfY, -yOneSixth);
        CooridinateMatrix[5, 1] = new Vector3(/*xHalfY + */ collisionDistance, 0);

        CooridinateMatrix[6, 0] = new Vector3(xHalfY, yOneSixth);
        CooridinateMatrix[6, 1] = new Vector3(/*xHalfY + */ collisionDistance, 0);

        CooridinateMatrix[7, 0] = new Vector3(xTwoThird, xOneThird);
        CooridinateMatrix[7, 1] = new Vector3(/*xTwoThird + */ collisionAdjusted, /*xOneThird + */ collisionAdjusted);

        CooridinateMatrix[8, 0] = new Vector3(xOneThird, xTwoThird);
        CooridinateMatrix[8, 1] = new Vector3(/*xOneThird + */ collisionAdjusted, /*xTwoThird + */ collisionAdjusted);

        CooridinateMatrix[9, 0] = new Vector3(yOneSixth, xHalfY);
        CooridinateMatrix[9, 1] = new Vector3(0, /*xHalfY + */ collisionDistance);

        CooridinateMatrix[10, 0] = new Vector3(-yOneSixth, xHalfY);
        CooridinateMatrix[10, 1] = new Vector3(0, /*xHalfY + */ collisionDistance);

        CooridinateMatrix[11, 0] = new Vector3(-xOneThird, xTwoThird);
        CooridinateMatrix[11, 1] = new Vector3(/*-xOneThird */ -collisionAdjusted, /*xTwoThird + */ collisionAdjusted);

        CooridinateMatrix[12, 0] = new Vector3(-xTwoThird, xOneThird);
        CooridinateMatrix[12, 1] = new Vector3(/*-xTwoThird */ -collisionAdjusted, /*xOneThird + */ collisionAdjusted);

        CooridinateMatrix[13, 0] = new Vector3(-xHalfY, yOneSixth);
        CooridinateMatrix[13, 1] = new Vector3(/*-xHalfY */ -collisionDistance, 0);

        CooridinateMatrix[14, 0] = new Vector3(-xHalfY, -yOneSixth);
        CooridinateMatrix[14, 1] = new Vector3(/*-xHalfY */ -collisionDistance, 0);

        CooridinateMatrix[15, 0] = new Vector3(-xTwoThird, -xOneThird);
        CooridinateMatrix[15, 1] = new Vector3(/*-xTwoThird */ -collisionAdjusted, /*-xOneThird */ -collisionAdjusted);
    }
    
    private void transition(int direction)
    {
        if (direction > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                positionOffset[i] += 2;
                Debug.Log(positionOffset[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                if (positionOffset[i] > 15)
                {
                    positionOffset[i] -= 15;
                   
                }
                Debug.Log(positionOffset[i]);
            }
        }
        else{
            for (int i = 0; i < 4; i++)
            {
                positionOffset[i] -= 2;
                Debug.Log(positionOffset[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                if (positionOffset[i] < 0)
                {
                    positionOffset[i] += 16;
                }
                Debug.Log(positionOffset[i]);
            }
        }
    }


}
