using UnityEngine;
using Vuforia;

public class FlashlightController : MonoBehaviour {

    private bool torchIsOn = false;

    public void ToggleFlashlight()
    {
        torchIsOn = !torchIsOn;
        CameraDevice.Instance.SetFlashTorchMode(torchIsOn);
    }
	
}
