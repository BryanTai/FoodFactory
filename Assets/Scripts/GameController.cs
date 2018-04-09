using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

[RequireComponent(typeof(Camera))]
public class GameController : MonoBehaviour
{
    #region FIELDS
    //General game fields
    private System.Random rnd;
    private int halfwayXPixel;

    //Vuforia Player Camera fields
    private Camera playerCamera;
    private GameObject playerCollider;

    //Cannon fields
    public GameObject Cannon;
    float rotateSpeed = 0.5f;
    Direction targetOffsetDirection;
    float verticalOffset = 15; //TODO adjust these
    float horizontalOffset = 50;
    
    //Ingredient launching fields
    private const float HEAVY_GRAVITY = -80f;
    private const float NO_GRAVITY = 0;
    private const float SPAWN_TIME = 2f;
    float ingredientLaunchSpeed = 20;

    //Ingredient spawning fields
    private IEnumerator spawnIngredientCoroutine;
    public GameObject BunPrefab; //TODO there's gotta be a better way to load these
    public GameObject PattyPrefab; //otherwise just move it all to a factory
    public GameObject LettucePrefab;
    public GameObject TomatoPrefab;
    public Transform IngredientSpawnPoint;
    private int totalIngredientTypes;

    //Canvas UI fields
    private CanvasController canvasController;
    //Game Logic fields
    private bool[] acquiredIngredients; //TODO make this a Dictionary with IngredientTypes and counts
    #endregion

    #region ENUMS
    
    enum Direction { up, down, left, right }
    #endregion

    #region UNITY_METHODS
    void Awake()
    {
        playerCamera = GetComponent<Camera>();
        canvasController = GetComponent<CanvasController>();

        halfwayXPixel = Screen.width / 2;
        rnd = new System.Random();
        totalIngredientTypes = Enum.GetNames(typeof(IngredientType)).Length;
        acquiredIngredients = new bool[totalIngredientTypes];
        
        Physics.gravity = new Vector3(0, NO_GRAVITY, 0);
        targetOffsetDirection = pickRandomCannonDirection();
        spawnIngredientCoroutine = shootIngredientAtIntervals(SPAWN_TIME);
    }

    // Use this for initialization
    void Start () {
        Debug.Log("HALFWAY PIXEL" +halfwayXPixel);
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
    #endregion // UNITY_METHODS

    public void HandlePlayerIngredientCollision(IngredientType ingredientType)
    {
        int iconIndex = (int)ingredientType;
        acquiredIngredients[iconIndex] = true;
        canvasController.ActivateIcon(iconIndex);

        bool allIngredientsAcquired = true;
        foreach(bool b in acquiredIngredients)
        {
            allIngredientsAcquired = allIngredientsAcquired && b;
        }

        if (allIngredientsAcquired)
        {
            handleAllIngredientsAcquired();
        }
        //TODO count score here
    }

    private void handleAllIngredientsAcquired()
    {
        Debug.Log("All Ingredients Acquired!");
        canvasController.ResetAllIcons();
        acquiredIngredients = new bool[totalIngredientTypes];
    }

    #region AIM_CANNON_CODE
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
    #endregion // AIM_CANNON_CODE

    #region INGREDIENT_LAUNCH_CODE

    public void StartShooting()
    {
        StartCoroutine(spawnIngredientCoroutine);
    }

    public void StopShooting()
    {
        StopCoroutine(spawnIngredientCoroutine);
    }

    private IEnumerator shootIngredientAtIntervals(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime/2);
            shootIngredient(pickRandomIngredient());
            yield return new WaitForSeconds(waitTime/2);
            targetOffsetDirection = pickRandomCannonDirection();
            //Debug.Log(targetOffsetDirection);
        }
    }

    private void shootIngredient(GameObject newIngredient)
    {
        newIngredient.transform.position = IngredientSpawnPoint.position;
        //Just need to get the position once, this ingredient is going in a straight line
        Vector3 targetDestination = getCannonTargetAroundCamera(targetOffsetDirection); 
        newIngredient.transform.LookAt(targetDestination);

        Vector3 initVelocity = newIngredient.transform.forward * ingredientLaunchSpeed;
        newIngredient.GetComponent<Rigidbody>().velocity = initVelocity;
        //Debug.Log("initVelocity: " + initVelocity);
    }

    private GameObject pickRandomIngredient()
    {
        int index = rnd.Next(totalIngredientTypes);
        IngredientType nextType = (IngredientType)index;

        GameObject newIngredient;
        switch (nextType)
        {
            case IngredientType.bun:
                newIngredient = Instantiate(BunPrefab); break;
            case IngredientType.patty:
                newIngredient = Instantiate(PattyPrefab); break;
            case IngredientType.lettuce:
                newIngredient = Instantiate(LettucePrefab); break;
            case IngredientType.tomato:
                newIngredient = Instantiate(TomatoPrefab); break;
            default:
                throw new ArgumentException();
        }
        newIngredient.GetComponent<Ingredient>().ingredientType = nextType;
        return newIngredient;
    }
    #endregion // INGREDIENT_LAUNCH_CODE

    #region DEBUG_CODE
    //TODO THIS IS JUST FOR DEBUGGING IN THE EDITOR
    private void handleKeys()
    {
    }

    private void handleTouches()
    {
        Touch newTouch = Input.GetTouch(0);
    }
    #endregion // DEBUG_CODE
}
