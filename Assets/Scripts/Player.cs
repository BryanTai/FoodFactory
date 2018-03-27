using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Vector3[] positions;
    int currentPositionIndex;

    private int halfwayYPixel;

	// Use this for initialization
	void Start () {
        currentPositionIndex = 0;
	}

    public void MovePlayerClockwise()
    {
        currentPositionIndex++;
        if (currentPositionIndex > positions.Length - 1)
        {
            currentPositionIndex = 0;
        }
        gameObject.transform.position = positions[currentPositionIndex];
    }

    public void MovePlayerCounterClockwise()
    {
        currentPositionIndex--;
        if (currentPositionIndex < 0 )
        {
            currentPositionIndex = positions.Length - 1;
        }
        gameObject.transform.position = positions[currentPositionIndex];
    }
    
}
