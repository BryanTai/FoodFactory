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
	
    internal void handleTouches()
    {
        if (!introAnim.GetBool("TouchedAtLeastOnce"))
        {
            introAnim.SetTrigger("FirstTouch");
            introAnim.SetBool("TouchedAtLeastOnce", true);
        }else if (introAnim.GetBool("InTutorial"))
        {
            introAnim.SetTrigger("TutorialScreenPressed");
            introAnim.SetBool("InTutorial", false);
        }
    }

    //Called by the Main Menu Start Button
    public void HideMainMenuAndStartGamePlay()
    {
        Debug.Log("Start button pressed!");
        introAnim.SetTrigger("StartGameButtonPressed");
    }

    public void HideMainMenuAndShowTutorial()
    {
        Debug.Log("How To button pressed!");
        introAnim.SetTrigger("TutorialButtonPressed");
    }

    public void HideMainMenuAndShowOptions()
    {
        Debug.Log("Options button pressed!");
        //TODO
    }

    //Called by the MainMenuToGameplay Animation Event
    public void StartGameplay()
    {
        canvasController.StartGameplay();
    }

    internal void showGameOverText()
    {
        GameOverText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        Animator gameOverAnim = GameOverText.GetComponent<Animator>();
        gameOverAnim.SetBool("IsGameOver", true);
    }
}
