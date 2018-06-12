using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvasController : MonoBehaviour {
    
    //Main Menu buttons
    public Button StartButton;
    public Button HowToButton;
    public Button OptionsButton;

    //GameOver Screen fields
    public GameObject GameOverText;

    // Use this for initialization
    void Awake () {
		
	}


    internal void showGameOverText()
    {
        Animator gameOverAnim = GameOverText.GetComponent<Animator>();
        gameOverAnim.SetBool("IsGameOver", true);
    }
}
