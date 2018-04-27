using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {
    public GameController gameController;
    public Canvas canvas;

    //TODO support a dynamic # of icons for different # of ingredients
    public Image icon_0;
    public Image icon_1;
    public Image icon_2;
    public Image icon_3;

    private Image[] icons;
    private int iconCount = 4;

    //Intro Screen fields
    //public GameObject TitleScreen;
    //public GameObject IntroBackground;
    //public GameObject IntroPhone;
    public GameObject IntroScreens;

    private const float DIM_ALPHA = 0.5f;

    //Score Notification Fields
    public GameObject ScoreAlertPrefab;
    private Vector3 scoreAlertSpawnPoint;
    private const float FADE_SPEED = 4f;
    private const float WAIT_TIME = 0.5f;

    void Awake()
    {
        icons = new Image[iconCount];
        icons[0] = icon_0; //TODO refactor this hard code :I
        icons[1] = icon_1;
        icons[2] = icon_2;
        icons[3] = icon_3;

        scoreAlertSpawnPoint = new Vector3(100, -120, 0);
        
    }

    // Use this for initialization
    void Start () {
        ResetAllIcons();
	}

    public void ActivateIcon(int index)
    {
        Color oldColor = icons[index].color;
        icons[index].color = new Color(oldColor.r, oldColor.g, oldColor.b, 1);
    }

    public void ResetAllIcons()
    {
        foreach (Image icon in icons)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, DIM_ALPHA);
        }
    }

    //Borrowed from LineSort
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

    //internal void FadeIntroScreen()
    //{
    //    IntroScreens.GetComponent<Animator>().SetTrigger("FirstTouch");
    //}
    //internal void FadeInstructionScreen()
    //{
    //    IntroScreens.GetComponent<Animator>().SetTrigger("SecondTouch");
    //}

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

}
