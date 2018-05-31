using UnityEngine;

public class ScoringIcon : MonoBehaviour {
    public CanvasController canvasController;
    public int currentIconIndex { get; set; }

    public void ActivateTopIcon()
    {
        canvasController.ActivateTopIcon(currentIconIndex);
    }
}
