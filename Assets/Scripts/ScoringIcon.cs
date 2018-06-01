using UnityEngine;

public class ScoringIcon : MonoBehaviour {
    public GameController gameController;
    public CanvasController canvasController;
    public int currentIconIndex { get; set; }

    //Animation Event functions triggered by ScoringIcon.anim

    //Called when ScoringIcon reaches the Top Bar
    public void ActivateTopIcon()
    {
        canvasController.ActivateTopIcon(currentIconIndex);
    }

    //Called when animation completes
    public void CheckIfAllIngredientsAcquired()
    {
        gameController.CheckIfAllIngredientsAcquired();
    }
}
