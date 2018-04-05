using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameController : MonoBehaviour {

    public Player player;
    private System.Random rnd;
    private int halfwayXPixel;

    //Camera fields
    private Camera playerCamera;
    public GameObject Cannon;
    float rotateSpeed = 0.5f;

    //Borderline fields
    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 100)]
    public float radius = 100;
    public Borderline borderline;
    public Vector3[] playerPositions { get; private set; }
    

    //Ingredient launching fields
    public Vector3 cannonPosition;
    private const float HEAVY_GRAVITY = -80f;
    private const float SPAWN_TIME = 2f;

    //Ingredient spawning fields
    public GameObject BunPrefab; //TODO there's gotta be a better way to load these
    public GameObject PattyPrefab; //otherwise just move it all to a factory
    public GameObject LettucePrefab;
    public GameObject TomatoPrefab;
    public Transform IngredientSpawnPosition;

    public enum IngredientType
    {
        bun,
        patty,
        lettuce,
        tomato
    }
    private int totalIngredientTypes;

    

    void Awake()
    {
        CreatePoints();
        halfwayXPixel = Screen.width / 2;
        rnd = new System.Random();
        totalIngredientTypes = Enum.GetNames(typeof(IngredientType)).Length;
        playerCamera = GetComponent<Camera>();
        Physics.gravity = new Vector3(0, HEAVY_GRAVITY, 0);
    }

    // Use this for initialization
    void Start () {
        SendPositionsToPlayerAndFactory();
        Debug.Log("HALFWAY " +halfwayXPixel);
        player.MovePlayerClockwise(); //TODO just to get the player onto the borderline
        cannonPosition = IngredientSpawnPosition.position; //TODO this is the top of the factory, find a better way to get this value

        StartCoroutine(shootIngredientAtIntervals(SPAWN_TIME));
    }

    private void SendPositionsToPlayerAndFactory()
    {
        player.positions = playerPositions;
        borderline.positions = playerPositions;
    }

    // Update is called once per frame
    void Update () {
        /* 
        if(Input.touchCount > 0)
        {
            handleTouches();
        }*/

        //Print camera coordinates (for testing)
        Vector3 targetPos = playerCamera.transform.position;
        Debug.Log("Camera : " + targetPos);
        Vector3 targetDir = targetPos - Cannon.transform.position;
        float step = rotateSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(Cannon.transform.forward, targetDir, step, 0.0f);
        Cannon.transform.rotation = Quaternion.LookRotation(newDir);


        //TODO THIS IS JUST FOR DEBUGGING IN THE EDITOR
        //handleKeys();
    }

    //TODO THIS IS JUST FOR DEBUGGING IN THE EDITOR
    private void handleKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            player.MovePlayerClockwise();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            player.MovePlayerCounterClockwise();
        }
    }

    private void handleTouches()
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

    private IEnumerator shootIngredientAtIntervals(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            shootIngredient();
        }
    }

    private void shootIngredient()
    {
        Vector3 randomDestination = pickRandomPlayerPosition();
        Vector3 initVelocity = calculateBallisticVelocity(randomDestination);

        GameObject newIngredient = pickRandomIngredient();
        newIngredient.transform.position = cannonPosition;
        newIngredient.GetComponent<Rigidbody>().velocity = initVelocity;
        //Debug.Log("initVelocity: " + initVelocity);
    }

    private GameObject pickRandomIngredient()
    {
        int index = rnd.Next(totalIngredientTypes);
        IngredientType nextType = (IngredientType)index;
        switch (nextType)
        {
            case IngredientType.bun:
                return Instantiate(BunPrefab);
            case IngredientType.patty:
                return Instantiate(PattyPrefab);
            case IngredientType.lettuce:
                return Instantiate(LettucePrefab);
            case IngredientType.tomato:
                return Instantiate(TomatoPrefab);
            default:
                throw new ArgumentException();
        }
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
        return playerPositions[rnd.Next(segments + 1)];
    }
}
