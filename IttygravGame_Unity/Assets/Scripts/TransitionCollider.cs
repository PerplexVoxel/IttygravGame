using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        Vector3 _Vector2 = new Vector3(20, 20);
        _Vector2 = _Vector2 + gameObject.transform.position;
        Debug.DrawRay(gameObject.transform.position, _Vector2, Color.red);

    }
    
}
