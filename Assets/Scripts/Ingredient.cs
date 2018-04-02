using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {
        //TODO switch this to check collision with Floor Plane and make it shrink
        if(transform.position.y < 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit the player!");
        }
    }
}
