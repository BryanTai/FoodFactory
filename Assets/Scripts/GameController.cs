using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 100)]
    public float radius = 100;

    public Vector3[] playerPositions { get; private set; }

    public Player player;
    public Borderline borderline;

    private int halfwayXPixel;

    Camera camera;

    void Awake()
    {
        CreatePoints();
        halfwayXPixel = Screen.width / 2;
    }

    // Use this for initialization
    void Start () {
        SendPositionsToPlayerAndFactory();
        Debug.Log("HALFWAY " +halfwayXPixel);
        player.MovePlayerClockwise(); //TODO just to get the player onto the borderline
        camera = GetComponent<Camera>();
        
    }

    private void SendPositionsToPlayerAndFactory()
    {
        player.positions = playerPositions;
        borderline.positions = playerPositions;
    }

    // Update is called once per frame
    void Update () {
        if(Input.touchCount > 0)
        {
            handleTouches();
        }
        Vector3 worldPos = camera.transform.position;
        Debug.Log("Camera : " + worldPos);
    }

    void handleTouches()
    {
        Touch newTouch = Input.GetTouch(0);

        if (newTouch.position.x < halfwayXPixel)
        {
            player.MovePlayerClockwise();
        } else
        {
            player.MovePlayerCounterClockwise();
        }

    }

    //Borrowed from https://gamedev.stackexchange.com/questions/126427/draw-circle-around-gameobject-to-indicate-radius
    void CreatePoints()
    {
        float x;
        float y = 1;
        float z;

        float angle = 20f;

        playerPositions = new Vector3[(segments + 1)];

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            playerPositions[i] = new Vector3(x, y, z);

            angle += (360f / segments);
        }
    }
}
