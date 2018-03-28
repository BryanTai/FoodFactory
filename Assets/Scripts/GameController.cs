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
    System.Random rnd;

    public Vector3 cannonPosition;
    public GameObject IngredientPrefab;
    public Transform IngredientSpawnPosition;

    void Awake()
    {
        CreatePoints();
        halfwayXPixel = Screen.width / 2;
        rnd = new System.Random();
    }

    // Use this for initialization
    void Start () {
        SendPositionsToPlayerAndFactory();
        Debug.Log("HALFWAY " +halfwayXPixel);
        player.MovePlayerClockwise(); //TODO just to get the player onto the borderline
        camera = GetComponent<Camera>();
        cannonPosition = IngredientSpawnPosition.position; //TODO this is the top of the factory, find a better way to get this value

        ShootIngredient();
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
        //Print camera coordinates (for testing)
        //Vector3 worldPos = camera.transform.position;
        //Debug.Log("Camera : " + worldPos);


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

    //Ingredient launching code

    private void ShootIngredient()
    {
        Vector3 randomDestination = pickRandomPlayerPosition();
        Vector3 initVelocity = calculateBallisticVelocity(randomDestination);

        GameObject newIngredient = Instantiate(IngredientPrefab);
        newIngredient.transform.position = cannonPosition;
        newIngredient.GetComponent<Rigidbody>().velocity = initVelocity;
        Debug.Log("initVelocity: " + initVelocity);
    }

    private Vector3 calculateBallisticVelocity(Vector3 destination)
    {
        Vector3 direction = destination - cannonPosition;
        float heightDifference = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float angle = 80 * Mathf.Deg2Rad; //TODO Hardcoded 80 degree angle
        direction.y = distance * Mathf.Tan(angle); //set direction to the elevation angle
        distance += heightDifference / Mathf.Tan(angle); //Correction for height difference

        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * angle));
        return velocity * direction.normalized;
    }

    private Vector3 pickRandomPlayerPosition()
    {
        return playerPositions[0]; //TODO FOR TESTING
        //return playerPositions[rnd.Next(segments + 1)];
    }
}
