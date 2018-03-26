using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Borrowed from https://gamedev.stackexchange.com/questions/126427/draw-circle-around-gameobject-to-indicate-radius

[RequireComponent(typeof(LineRenderer))]
public class Borderline : MonoBehaviour {

    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 100)]
    public float xradius = 100;
    [Range(0, 100)]
    public float yradius = 100;

    public float width = 1;
    LineRenderer line;

    // Use this for initialization
    void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.useWorldSpace = true;

        CreatePoints();
    }
	
	void CreatePoints()
    {
        float x;
        float y;
        float z;

        float angle = 20f;
        line.widthMultiplier = width;
        line.positionCount = (segments+1);


        Vector3[] positions = new Vector3[(segments + 1)];

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            //line.SetPosition(i, new Vector3(x, 0, z));
            positions[i] = new Vector3(x, 0, z);

            angle += (360f / segments);
        }

        line.SetPositions(positions);
    }
}
