using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Borderline borderline;
    public Vector3[] positions;
    int currentPositionIndex;

	// Use this for initialization
	void Start () {
        borderline = GameObject.FindGameObjectWithTag("Borderline").GetComponent<Borderline>();
        currentPositionIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {

        gameObject.transform.position = positions[currentPositionIndex];
        currentPositionIndex++;
        if(currentPositionIndex > positions.Length - 1)
        {
            currentPositionIndex = 0;
        }

    }
}
