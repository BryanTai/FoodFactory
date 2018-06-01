using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {
    public GameController gameController;
    public Canvas canvas;

    //TODO support a dynamic # of icons for different # of ingredients
    public Image BunIcon;
    public Image PattyIcon;
    public Image LettuceIcon;
    public Image KetchupIcon;

    public ScoringIcon ScoringIcon;

    private Dictionary<IngredientType, Image> ingredientIcons;
    private int iconCount = 4;

    //Intro Screen fields
    public GameObject IntroScreens;

    //GameOver Screen fields
    public GameObject GameOverText;

    private const float DIM_ALPHA = 0.5f;
    private const float NO_ALPHA = 0;

    //Score Notification Fields
    public GameObject ScoreAlertPrefab;
    private Vector3 scoreAlertSpawnPoint;
    private const float FADE_SPEED = 4f;
    private const float WAIT_TIME = 0.5f;

    void Awake()
    {
        ingredientIcons = new Dictionary<IngredientType, Image>();
        ingredientIcons.Add(IngredientType.bun, BunIcon);
        ingredientIcons.Add(IngredientType.patty, PattyIcon);
        ingredientIcons.Add(IngredientType.lettuce, LettuceIcon);
        ingredientIcons.Add(IngredientType.ketchup, KetchupIcon);

        scoreAlertSpawnPoint = new Vector3(100, -120, 0);
    }

    // Use this for initialization
    void Start () {
        ResetAllIcons();
	}

    #region ICON_CODE

    public void ResetAllIcons()
    {
        foreach (Image icon in ingredientIcons.Values)
        {
            hideIcon(icon);
        }
        hideIcon(ScoringIcon.GetComponent<Image>());
    }

    private void hideIcon(Image icon)
    {
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, NO_ALPHA);
    }

    public void ActivateScoringIcon(IngredientType ingredientType)
    {
        setScoringIconSprite(ingredientType);
        animateScoringIcon(ingredientType);
    }
    
    private void setScoringIconSprite(IngredientType ingredientType)
    {
        ScoringIcon.currentIngredientIcon = ingredientType;
        ScoringIcon.GetComponent<Image>().sprite = ingredientIcons[ingredientType].sprite;
    }

    private void animateScoringIcon(IngredientType ingredientType)
    {
        //Animation scoreAnim = ScoringIcon.GetComponent<Animation>();
        //scoreAnim.Play();
        Animator scoreAnim = ScoringIcon.GetComponent<Animator>();
        scoreAnim.SetTrigger(ingredientType.ToString());
    }

    //This gets called when ScoringIcon reaches the Top bar
    public void ActivateTopIcon(IngredientType ingredientType)
    {
        Color oldColor = ingredientIcons[ingredientType].color;
        ingredientIcons[ingredientType].color = new Color(oldColor.r, oldColor.g, oldColor.b, 1);
    }

    #endregion //ICON_CODE

    #region SCORE_CODE
    public void FlashScoreAlert(string text, Color color)
    {
        StartCoroutine(displayScoreAlert(text, color));
    }

    private IEnumerator displayScoreAlert(string text, Color color)
    {
        //Create new invisible
        GameObject newAlert = Instantiate(ScoreAlertPrefab, scoreAlertSpawnPoint, Quaternion.identity);
        newAlert.transform.SetParent(canvas.transform, false);

        Text alertText = newAlert.GetComponent<Text>();
        alertText.text = text;
        alertText.color = color;

        //Fade in
        alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, 0);
        while (alertText.color.a < 1.0f)
        {
            alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, alertText.color.a + (Time.deltaTime * FADE_SPEED));
            yield return null;
        }
        //hold it
        yield return new WaitForSeconds(WAIT_TIME);

        //Fade out
        alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, 1);
        while (alertText.color.a > 0.0f)
        {
            alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, alertText.color.a - (Time.deltaTime * FADE_SPEED));
            yield return null;
        }

        Destroy(newAlert);
    }
    #endregion //SCORE_CODE

    #region SCREEN_CODE
    internal void showNextIntroScreen()
    {
        Animator introAnim = IntroScreens.GetComponent<Animator>();

        introAnim.SetTrigger("FirstTouch");
        if (introAnim.GetBool("FadeTitleComplete"))
        {
            introAnim.SetTrigger("SecondTouch");
            gameController.StartGameplay();
        }
        if (introAnim.GetBool("FadeInstructionsComplete"))
        {
            introAnim.SetTrigger("GameStarted");
            //introAnim.enabled = false;
        }
    }

    internal void showGameOverScreen()
    {
        Animator gameOverAnim = GameOverText.GetComponent<Animator>();
        gameOverAnim.SetBool("IsGameOver", true);
    }
    #endregion //SCREEN_CODE
}
