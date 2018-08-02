using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FlatHUDController handles the visibility of the IngredientModels, the Timer, and the Score
//Timer is handled by its own script
public class FlatHUDController : MonoBehaviour {

    public GameController gameController;

    //TODO replace with array
    public GameObject BunModel;
    public GameObject PattyModel;
    public GameObject LettuceModel;
    public GameObject KetchupModel;

    private Dictionary<IngredientType, GameObject> ingredientModels;

    // Use this for initialization
    void Awake ()
    {
        //TODO Replace with LevelParams
        ingredientModels = new Dictionary<IngredientType, GameObject>();
        ingredientModels.Add(IngredientType.bun, BunModel);
        ingredientModels.Add(IngredientType.patty, PattyModel);
        ingredientModels.Add(IngredientType.lettuce, LettuceModel);
        ingredientModels.Add(IngredientType.ketchup, KetchupModel);
    }

    void Start()
    {
        hideAllModels();
    }

    private void hideAllModels()
    {
        foreach(GameObject model in ingredientModels.Values)
        {
            hideModel(model);
        }
    }

    private void hideModel(GameObject model)
    {
        model.SetActive(false);
    }

    //TODO Shrink model til it disappears
    private void shrinkModel(GameObject model)
    {
        model.SetActive(false);
    }

    private void showModel(GameObject model)
    {
        model.SetActive(true);
    }


    public void ActivateIngredientModel(IngredientType ingredientType)
    {
        showModel(ingredientModels[ingredientType]);
    }
}
