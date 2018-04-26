﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

[RequireComponent(typeof(Camera))]
public class GameController : MonoBehaviour
{
    #region FIELDS
    //General game fields
    private System.Random rnd;
    public Timer timer;
    public Text ScoreText;
    private GameStateHandler gameStateHandler;

    //Intro fields
    private const int TOTAL_INTRO_SCREENS = 2;
    private int currentIntroScreen;

    //Player fields
    private Camera playerCamera;
    private GameObject playerCollider;
    int playerScore;

    //Cannon fields
    public GameObject Cannon;
    float rotateSpeed = 0.5f;
    Direction targetOffsetDirection;
    float verticalOffset = 15; //TODO adjust these
    float horizontalOffset = 50;
    
    //Ingredient launching fields
    private const float HEAVY_GRAVITY = -80f;
    private const float NO_GRAVITY = 0; //TODO just disable gravity entirely :I
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
    private bool isMealReady;

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

        rnd = new System.Random();
        gameStateHandler = new GameStateHandler();
        totalIngredientTypes = Enum.GetNames(typeof(IngredientType)).Length;
        acquiredIngredients = new bool[totalIngredientTypes];
        currentIntroScreen = 0;

        Physics.gravity = new Vector3(0, NO_GRAVITY, 0);
        targetOffsetDirection = chooseRandomCannonDirection();
        spawnFoodCoroutine = launchFoodAtIntervals(SPAWN_TIME);
    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update ()
    {
        if(GetGameState() == GameState.Intro)
        {
            handleIntroTouches();
            handleKeys(); //TODO THIS IS JUST FOR DEBUGGING
        }
        
        if (GetGameState() == GameState.Detected)
        {
            aimCannon();
        }

        //TODO For Debugging Purposes
        //handleTouches();
    }


    #endregion // UNITY_METHODS

    #region ON_EVENT_CODE

    private void handleIntroTouches()
    {
        //TODO SEE DEBUG CODE handleKeys
    }

    //Called by ImageTargetEventHandler.cs
    public void HandleImageTargetDetected()
    {
        gameStateHandler.SetCurrentState(GameState.Detected);
        if (gameStateHandler.CurrentGameState.InGame()) {
            timer.StartTimer();
            StartCoroutine(spawnFoodCoroutine);
        }
    }

    public void HandleImageTargetLost()
    {
        gameStateHandler.SetCurrentState(GameState.NotDetected);
        if (gameStateHandler.CurrentGameState.InGame())
        {
            StopCoroutine(spawnFoodCoroutine);
        }
    }

    //Called by Player.cs
    public void HandlePlayerFoodCollision(GameObject foodObject)
    {
        Ingredient collidedIngredient = foodObject.GetComponent<Ingredient>();
        if (collidedIngredient != null)
        {
            handlePlayerIngredientCollision(collidedIngredient);
        }
        //TODO is detecting Meal collisions necessary?
        Meal collidedMeal = foodObject.GetComponent<Meal>();
        if(collidedMeal != null)
        {
            handlePlayerMealCollision(collidedMeal);
        }
        Food collidedFood = foodObject.GetComponent<Food>();
        updatePlayerScore(collidedFood.pointAward);
    }

    private void updatePlayerScore(int pointsAwarded)
    {
        playerScore += pointsAwarded;
        ScoreText.text = playerScore.ToString();
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
        //TODO maybe move this to CanvasController?
        string scoreText = string.Format("+{0}", collidedIngredient.pointAward);
        canvasController.FlashScoreAlert(scoreText, Color.white);
    }

    private void handlePlayerMealCollision(Meal collidedMeal)
    {
        //TODO might not need this step
        Debug.Log("Scored a meal!");
        string scoreText = string.Format("+{0}!", collidedMeal.pointAward);
        canvasController.FlashScoreAlert(scoreText, Color.green);
    }

    private void handleAllIngredientsAcquired()
    {
        Debug.Log("All Ingredients Acquired!");
        isMealReady = true;
        canvasController.ResetAllIcons();
        acquiredIngredients = new bool[totalIngredientTypes];
    }

    public GameState GetGameState()
    {
        return gameStateHandler.CurrentGameState;
    }
    #endregion //ON_EVENT_CODE

    #region AIM_CANNON_CODE
    private void aimCannon()
    {
        Vector3 targetPos = getCannonTargetAroundCamera(targetOffsetDirection);
        Vector3 targetDir = targetPos - Cannon.transform.position;
        float step = rotateSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(Cannon.transform.forward, targetDir, step, 0.0f);
        Cannon.transform.rotation = Quaternion.LookRotation(newDir);
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
            if (isMealReady)
            {
                //TODO Add some kind of aura or particle effect 
                nextFood = createMeal(MealType.burger);
                isMealReady = false;
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
        if (Input.GetKeyDown("1"))
        {
            canvasController.FadeIntroScreen();
        }
        if (Input.GetKeyDown("2"))
        {
            canvasController.FadeInstructionScreen();

        }
        if (Input.GetKeyDown("3"))
        {
            gameStateHandler.SetStateToInGame();
        }
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
