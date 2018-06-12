using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Animator))]
public class ScreenCanvasController : MonoBehaviour {
    Animator introAnim;
    CanvasController canvasController;
    //GameOver Screen fields
    public GameObject GameOverText;

    void Start () {
        introAnim = GetComponent<Animator>();
        canvasController = transform.parent.GetComponent<CanvasController>();
    }
	
    internal void showNextIntroScreen()
    { //TODO CLEAN UP UNUSED TRIGGERS
        if (!introAnim.GetBool("TouchedAtLeastOnce"))
        {
            introAnim.SetTrigger("FirstTouch");
            introAnim.SetBool("TouchedAtLeastOnce", true);
        }
        if (introAnim.GetBool("FadeTitleComplete"))
        {
            introAnim.SetTrigger("SecondTouch");
            //gameController.StartGameplay();
            //ShowHUD();
        }
        if (introAnim.GetBool("FadeInstructionsComplete"))
        {
            introAnim.SetTrigger("GameStarted");

            //introAnim.enabled = false;
        }
    }

    //Called by the Main Menu Start Button
    public void HideMainMenuAndStartGamePlay()
    {
        introAnim.SetTrigger("StartGameButtonPressed");
    }

    public void HideMainMenuAndShowTutorial()
    {
        //TODO
    }

    public void HideMainMenuAndShowOptions()
    {
        //TODO
    }

    //Called by the MainMenuToGameplay Animation Event
    public void StartGameplay()
    {
        canvasController.StartGameplay();
    }

    internal void showGameOverText()
    {
        Animator gameOverAnim = GameOverText.GetComponent<Animator>();
        gameOverAnim.SetBool("IsGameOver", true);
    }
}
