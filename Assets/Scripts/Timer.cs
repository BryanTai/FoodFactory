using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public GameController gameController;
    private TextMesh TimerText;
    private MeshRenderer TimerRenderer;
    public Material RedMaterial;

    private float startTime;
    public float timeLeftSeconds { get; set; }
    private bool stopTimer = true;
    private const int FLASHING_TIME = 10;
    private bool timerIsStillWhite = true;

    void Start()
    {
        TimerText = GetComponent<TextMesh>();
        TimerRenderer = GetComponent<MeshRenderer>();
        //TimerRenderer.material = TimerRenderer.materials[0];
        startTime = Time.time;
        timeLeftSeconds = 60; //TODO Load this from GameController or something
    }

    public void StartTimer()
    {
        stopTimer = false;
    }

    public void StopTimer()
    {
        stopTimer = true;
    }

    void Update()
    {
        if (stopTimer)
        {
            //TODO Blink the timer
            return;
        }

        timeLeftSeconds -= Time.deltaTime;
        float timeToPrint = Mathf.Round(timeLeftSeconds);
        string minutes = ((int)timeToPrint / 60).ToString();
        string seconds = (timeToPrint % 60).ToString("f0"); //set to "f2" for 2 decimal places

        if (timeToPrint % 60 < 10)
        {
            seconds = "0" + seconds;
        }

        TimerText.text = minutes + ":" + seconds;

        if (timerIsStillWhite && timeLeftSeconds <= FLASHING_TIME)
        {
            TimerRenderer.material = RedMaterial;
            timerIsStillWhite = false;
        }

        if (timeLeftSeconds < 0)
        {
            Debug.Log("Timer is DONE!!");
            stopTimer = true;
            
            gameController.HandleTimeOut();
        }
    }

    
}
