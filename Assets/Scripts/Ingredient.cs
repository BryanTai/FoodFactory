using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {
        if(transform.position.y < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
