using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

//Borrowed from Vuforia/DefaultTrackableEventHandler.cs

public class ImageTargetEventHandler : MonoBehaviour, Vuforia.ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;
    public GameController gameController;

    // Use this for initialization
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
             newStatus == TrackableBehaviour.Status.TRACKED ||
             newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }
    private void OnTrackingFound()
    {
        Debug.Log("Start shooting ingredients!");
        gameController.StartShooting();
    }

    private void OnTrackingLost()
    {
        Debug.Log("Stop shooting ingredients!");
        gameController.StopShooting();
    }

    
}
