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
    public Timer timer;

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
    float ingredientLaunchSpeed = 30;

    //Ingredient spawning fields
    private IEnumerator spawnFoodCoroutine;
    public GameObject BunPrefab; //TODO there's gotta be a better way to load these
    public GameObject PattyPrefab; //otherwise just move it all to a factory
    public GameObject LettucePrefab;
    public GameObject TomatoPrefab;
    public GameObject BurgerPrefab;
    public Transform FoodSpawnPoint;
    private int totalIngredientTypes;
    private bool launchMealNext;

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
        targetOffsetDirection = chooseRandomCannonDirection();
        spawnFoodCoroutine = launchFoodAtIntervals(SPAWN_TIME);
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

        //TODO For Debugging Purposes
        //handleTouches();
    }
    #endregion // UNITY_METHODS

    #region ON_EVENT_CODE
    public void HandleImageTargetDetected()
    {
        timer.StartTimer();
        StartCoroutine(spawnFoodCoroutine);
    }

    public void HandleImageTargetLost()
    {
        StopCoroutine(spawnFoodCoroutine);
    }

    public void HandlePlayerFoodCollision(GameObject foodObject)
    {
        Ingredient collidedIngredient = foodObject.GetComponent<Ingredient>();
        if (collidedIngredient != null)
        {
            handlePlayerIngredientCollision(collidedIngredient);
        }
        Meal collidedMeal = foodObject.GetComponent<Meal>();
        if(collidedMeal != null)
        {
            handlePlayerMealCollision(collidedMeal);
        }
        
        //TODO count score here
    }

    private void handlePlayerIngredientCollision(Ingredient collidedIngredient)
    {
        int iconIndex = (int)collidedIngredient.ingredientType;
        acquiredIngredients[iconIndex] = true;
        canvasController.ActivateIcon(iconIndex);

        bool allIngredientsAcquired = true;
        foreach (bool b in acquiredIngredients)
        {
            allIngredientsAcquired = allIngredientsAcquired && b;
        }

        if (allIngredientsAcquired)
        {
            handleAllIngredientsAcquired();
        }
    }

    private void handlePlayerMealCollision(Meal collidedMeal)
    {
        throw new NotImplementedException();
    }

    private void handleAllIngredientsAcquired()
    {
        Debug.Log("All Ingredients Acquired!");
        canvasController.ResetAllIcons();
        acquiredIngredients = new bool[totalIngredientTypes];
    }
    #endregion //ON_EVENT_CODE

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

    private Direction chooseRandomCannonDirection()
    {
        return (Direction)rnd.Next(4);
    }
    #endregion // AIM_CANNON_CODE

    #region FOOD_LAUNCH_CODE

    private IEnumerator launchFoodAtIntervals(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime/2);

            GameObject nextFood;
            if (launchMealNext)
            {
                nextFood = createMeal(MealType.burger);
            }
            else
            {
                nextFood = createRandomIngredient();
            }

            launchFood(nextFood);
            yield return new WaitForSeconds(waitTime/2);
            targetOffsetDirection = chooseRandomCannonDirection();
            //Debug.Log(targetOffsetDirection);
        }
    }

    private void launchFood(GameObject newFood)
    {
        newFood.transform.position = FoodSpawnPoint.position;
        //Just need to get the position once, this food is going in a straight line
        Vector3 targetDestination = getCannonTargetAroundCamera(targetOffsetDirection); 
        newFood.transform.LookAt(targetDestination);

        Vector3 initVelocity = newFood.transform.forward * ingredientLaunchSpeed;
        newFood.GetComponent<Rigidbody>().velocity = initVelocity;
        //Debug.Log("initVelocity: " + initVelocity);
    }

    private GameObject createRandomIngredient()
    {
        int index = rnd.Next(totalIngredientTypes);
        IngredientType nextType = (IngredientType)index;
        return createIngredient(nextType);
    }

    private GameObject createIngredient(IngredientType ingredientType)
    {
        GameObject newIngredient;
        switch (ingredientType)
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
        newIngredient.GetComponent<Ingredient>().ingredientType = ingredientType;
        return newIngredient;
    }

    private GameObject createMeal(MealType mealType)
    {
        GameObject newMeal;
        switch (mealType)
        {
            case MealType.burger:
                newMeal = Instantiate(BurgerPrefab); break;
            default:
                throw new ArgumentException();
        }
        newMeal.GetComponent<Meal>().mealType = mealType;
        return newMeal;
    }

    #endregion // FOOD_LAUNCH_CODE

    #region DEBUG_CODE
    //TODO THIS IS JUST FOR DEBUGGING IN THE EDITOR
    private void handleKeys()
    {
    }

    private void handleTouches()
    {
        //Touch newTouch = Input.GetTouch(0);
        if(Input.touchCount > 0)
        {
            Debug.Log(getDistanceFromCannonToCamera());
        }
    }

    private float getDistanceFromCannonToCamera()
    {
        return Vector3.Distance(playerCamera.transform.position, FoodSpawnPoint.transform.position);
    }
    #endregion // DEBUG_CODE


}
