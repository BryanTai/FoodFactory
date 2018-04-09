using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {

    //TODO support a dynamic # of icons for different # of ingredients
    public Image icon_0;
    public Image icon_1;
    public Image icon_2;
    public Image icon_3;

    private Image[] icons;
    private int iconCount = 4;

    float dimAlpha = 0.5f;

    void Awake()
    {
        icons = new Image[iconCount];
        icons[0] = icon_0; //TODO refactor this hard code :I
        icons[1] = icon_1;
        icons[2] = icon_2;
        icons[3] = icon_3;
    }

    // Use this for initialization
    void Start () {
		foreach(Image icon in icons)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, dimAlpha);
        }
	}

}
