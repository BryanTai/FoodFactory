using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameController : MonoBehaviour {
    private System.Random rnd;
    private int halfwayXPixel;

    //Camera fields
    private Camera playerCamera;

    //Cannon fields
    public GameObject Cannon;
    float rotateSpeed = 0.5f;
    enum Direction { up, down, left, right }
    Direction targetOffsetDirection;
    float verticalOffset = 15; //TODO adjust these
    float horizontalOffset = 50;

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
    private const float NO_GRAVITY = 0;
    private const float SPAWN_TIME = 2f;
    float ingredientLaunchSpeed = 20;

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
        halfwayXPixel = Screen.width / 2;
        rnd = new System.Random();
        totalIngredientTypes = Enum.GetNames(typeof(IngredientType)).Length;
        playerCamera = GetComponent<Camera>();
        Physics.gravity = new Vector3(0, NO_GRAVITY, 0);
        targetOffsetDirection = pickRandomCannonDirection();
    }

    // Use this for initialization
    void Start () {
        Debug.Log("HALFWAY " +halfwayXPixel);
        cannonPosition = IngredientSpawnPosition.position; //TODO this is the top of the factory, find a better way to get this value

        StartCoroutine(shootIngredientAtIntervals(SPAWN_TIME));
    }

    // Update is called once per frame
    void Update () {
        Vector3 targetPos = getCannonTargetAroundCamera(targetOffsetDirection);
        Vector3 targetDir = targetPos - Cannon.transform.position;
        float step = rotateSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(Cannon.transform.forward, targetDir, step, 0.0f);
        Cannon.transform.rotation = Quaternion.LookRotation(newDir);
        //Debug.DrawRay(Cannon.transform.position, newDir, Color.red, 5.0f);
    }

    

    private Vector3 getCannonTargetAroundCamera(Direction direction)
    {
        switch (direction)
        {
            case (Direction.up): 
                return playerCamera.transform.position + (transform.up * verticalOffset);
            case (Direction.down):
                return playerCamera.transform.position + (-transform.up * verticalOffset);
            case (Direction.left):
                return playerCamera.transform.position + (-transform.right * horizontalOffset);
            case (Direction.right):
                return playerCamera.transform.position + (transform.right * horizontalOffset);
            default:
                throw new ArgumentException();
        }
    }

    private Direction pickRandomCannonDirection()
    {
        return (Direction)rnd.Next(4);
    }

    //TODO THIS IS JUST FOR DEBUGGING IN THE EDITOR
    private void handleKeys()
    {
    }

    private void handleTouches()
    {
        Touch newTouch = Input.GetTouch(0);
    }

    //Ingredient launching code

    private IEnumerator shootIngredientAtIntervals(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            shootIngredient();
            targetOffsetDirection = pickRandomCannonDirection();
            Debug.Log(targetOffsetDirection);
        }
    }

    private void shootIngredient()
    {
        GameObject newIngredient = pickRandomIngredient();
        newIngredient.transform.position = cannonPosition;
        Vector3 targetDestination = getCannonTargetAroundCamera(targetOffsetDirection); //Just need to get the position once, this ingredient is going in a straight line
        newIngredient.transform.LookAt(targetDestination);

        
        Vector3 initVelocity = newIngredient.transform.forward * ingredientLaunchSpeed;

        newIngredient.GetComponent<Rigidbody>().velocity = initVelocity;

        Debug.Log("initVelocity: " + initVelocity);
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

}
