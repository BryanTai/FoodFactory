using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class Borderline : MonoBehaviour {
    public float width = 1;
    LineRenderer line;

    public Vector3[] positions { get; set; }

    // Use this for initialization
    void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.widthMultiplier = width;
        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }
}
