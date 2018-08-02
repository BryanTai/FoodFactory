using System;
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
    public TextMesh ScoreText;
    private GameStateHandler gameStateHandler;

    //Intro fields
    private const int TOTAL_INTRO_SCREENS = 2;
    private int currentIntroScreen;

    //Sound fields
    //Cannon sound SOURCE: https://freesound.org/people/baefild/sounds/91293/
    private AudioSource nomSound; //SOURCE: Team Fortress 2. //TODO NEED TO REPLACE
    private AudioSource bwipSound; //SOURCE: Made by me with Bfxr mixer

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
    public CanvasController canvasController;
    public FlatHUDController flatHUDController;

    //Game Logic fields

    //TODO make this a Dictionary with IngredientTypes and counts
    private Dictionary<IngredientType, bool> acquiredIngredientTypes;
    #endregion

    #region ENUMS

    enum Direction { up, down, left, right, none }
    private int totalDirections;
    private bool debugOnlyNoneDirectionFlag = false;
    #endregion

    #region UNITY_METHODS
    void Awake()
    {
        //Dynamically find Components
        playerCamera = GetComponent<Camera>();

        //Register Audio
        AudioSource[] allSounds = GetComponents<AudioSource>();
        nomSound = allSounds[0];
        bwipSound = allSounds[1];

        //Set global fields
        rnd = new System.Random();
        gameStateHandler = new GameStateHandler();
        totalIngredientTypes = Enum.GetNames(typeof(IngredientType)).Length;
        totalDirections = Enum.GetNames(typeof(Direction)).Length;
        resetAcquiredIngredientTypes();

        currentIntroScreen = 0;
        Physics.gravity = new Vector3(0, NO_GRAVITY, 0);
        targetOffsetDirection = chooseRandomCannonDirection();
        spawnFoodCoroutine = launchFoodAtIntervals(SPAWN_TIME);
    }

    private void resetAcquiredIngredientTypes()
    {
        acquiredIngredientTypes = new Dictionary<IngredientType, bool>(totalIngredientTypes);
        foreach (IngredientType ingredientType in Enum.GetValues(typeof(IngredientType)))
        {
            acquiredIngredientTypes.Add(ingredientType, false);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(GetGameState() == GameState.Intro)
        {
            handleIntroTouches();
        }
        
        if (GetGameState() == GameState.Detected)
        {
            aimCannon();
        }

        if (GetGameState() == GameState.GameOver)
        {
            handleGameOverTouches();
        }

        handleKeys(); //TODO THIS IS JUST FOR DEBUGGING
    }

    #endregion // UNITY_METHODS

    #region ON_EVENT_CODE

    private void handleIntroTouches()
    {
        if (Input.touchCount > 0)
        {
            canvasController.handleIntroTouches();
        }
    }

    //Called by CanvasController.cs
    public void StartGameplay()
    {
        gameStateHandler.SetCurrentStateToInGame();
        if (gameStateHandler.LastSavedStateBeforeIntro == GameState.Detected)
        {
            HandleImageTargetDetected();
        }
        else if (gameStateHandler.LastSavedStateBeforeIntro == GameState.NotDetected)
        {
            HandleImageTargetLost();
        }
    }

    //Called by ImageTargetEventHandler.cs
    public void HandleImageTargetDetected()
    {
        gameStateHandler.SetCurrentState(GameState.Detected);
        if (gameStateHandler.CurrentGameState.InGame()) {
            timer.StartTimer();
            StartCoroutine(spawnFoodCoroutine);
        }else
        {
            gameStateHandler.LastSavedStateBeforeIntro = GameState.Detected;
        }
    }

    //Called by ImageTargetEventHandler.cs
    public void HandleImageTargetLost()
    {
        gameStateHandler.SetCurrentState(GameState.NotDetected);
        if (gameStateHandler.CurrentGameState.InGame())
        {
            //TODO Encourage player to look at target card again
            timer.StopTimer();
            StopCoroutine(spawnFoodCoroutine);
        }else
        {
            gameStateHandler.LastSavedStateBeforeIntro = GameState.NotDetected;
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
        ScoreText.text = "Score " + playerScore.ToString();
    }

    private void handlePlayerIngredientCollision(Ingredient collidedIngredient)
    {
        IngredientType ingredientType = collidedIngredient.ingredientType;
        acquiredIngredientTypes[ingredientType] = true;

        //TODO have OPTION to swap between FlatHUD or Phonehud here
        //canvasController.ActivateScoringIcon(ingredientType); 
        flatHUDController.ActivateIngredientModel(ingredientType);

        bwipSound.Play();

        //TODO maybe move this to CanvasController?
        string scoreText = string.Format("+{0}", collidedIngredient.pointAward);
        canvasController.FlashScoreAlert(scoreText, Color.white);
        StartCoroutine(delayedCheckIfAllIngredientsAcquired());
    }

    private IEnumerator delayedCheckIfAllIngredientsAcquired()
    {
        yield return new WaitForSeconds(1);
        CheckIfAllIngredientsAcquired();
    }

    //Called by ScoringIcon.cs and delayedCheck
    internal void CheckIfAllIngredientsAcquired()
    {
        bool allIngredientsAcquired = true;
        foreach (bool b in acquiredIngredientTypes.Values)
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

        Debug.Log("Scored a meal!");
        nomSound.Play();
        canvasController.FadeMealIcon();
        string scoreText = string.Format("+{0}!", collidedMeal.pointAward);
        canvasController.FlashScoreAlert(scoreText, Color.green);
    }

    private void handleAllIngredientsAcquired()
    {
        Debug.Log("All Ingredients Acquired!");
        //TODO PLAY FULL MEAL SOUND EFFECT HERE
        isMealReady = true;

        //TODO have OPTION to swap between FlatHUD or Phonehud here
        //canvasController.ResetAllIcons();
        canvasController.ActivateMealIcon();

        flatHUDController.ShrinkAllModels();

        resetAcquiredIngredientTypes();
    }

    internal void HandleTimeOut()
    {
        //TODO
        // New scene?
        //UnityEngine.SceneManagement.SceneManager.LoadScene("LevelComplete");
        gameStateHandler.SetCurrentState(GameState.GameOver);
        canvasController.showGameOverText();
    }

    private void handleGameOverTouches()
    {
        if (Input.touchCount > 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("main");
        }
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
            case (Direction.none):
                return playerCamera.transform.position;
            default:
                throw new ArgumentException();
        }
    }

    private Direction chooseRandomCannonDirection()
    {
        //TODO this is only for DEBUGGING. Remove before production
        if (debugOnlyNoneDirectionFlag) { return Direction.none; }

        return (Direction)rnd.Next(totalDirections);
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
                nextFood = createMeal(MealType.burger);
                isMealReady = false;
            }
            else
            {
                nextFood = createRandomIngredient();
            }
            Cannon.GetComponent<AudioSource>().Play();
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

    const int ACQUIRED_TYPE_WEIGHT = 1;
    const int UNACQUIRED_TYPE_WEIGHT = 4;
    private GameObject createRandomIngredient()
    {
        IngredientType nextType = findWeightedRandomIngredientType();
        return createIngredient(nextType);
    }

    private IngredientType findWeightedRandomIngredientType()
    {
        int totalWeight = 0;
        int[] typeWeights = new int[totalIngredientTypes];
        foreach (int typeIndex in acquiredIngredientTypes.Keys)
        {
            if (acquiredIngredientTypes[(IngredientType)typeIndex] == true)
            {
                typeWeights[typeIndex] = ACQUIRED_TYPE_WEIGHT;
                totalWeight += ACQUIRED_TYPE_WEIGHT;
            }
            else
            {
                typeWeights[typeIndex] = UNACQUIRED_TYPE_WEIGHT;
                totalWeight += UNACQUIRED_TYPE_WEIGHT;
            }
        }

        int randomGuess = rnd.Next(totalWeight);

        IngredientType nextType = 0;

        for (int typeIndex = 0; typeIndex < totalIngredientTypes; typeIndex++)
        {
            if (randomGuess < typeWeights[typeIndex])
            {
                nextType = (IngredientType)typeIndex;
                break;
            }

            randomGuess = randomGuess - typeWeights[typeIndex];
        }

        return nextType;
    }

    //TODO Instead of Instantiating new Objects, load them in advance and hide them by disabling
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
            case IngredientType.ketchup:
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
    //BE SURE TO DISABLE ALL THIS FOR RELEASE
    private void handleKeys()
    {
        if (Input.GetKeyDown("1"))
        {
            canvasController.handleIntroTouches();
        }

        if (Input.GetKeyDown("2"))
        {
            debugOnlyNoneDirectionFlag = true;
        }
        
        if (Input.GetKeyDown("3"))
        {
            canvasController.showGameOverText();
        }

        if (Input.GetKeyDown("4"))
        {
            canvasController.DEBUGHideMainMenuAndStartGamePlay();
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
