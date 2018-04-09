using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour {

    private GameController gameContoller;


	// Use this for initialization
	void Start () {
        gameContoller = transform.parent.gameObject.GetComponent<GameController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        IngredientType scoredType = collision.gameObject.GetComponent<Ingredient>().ingredientType;

        Debug.Log("Player collided with " + scoredType.ToString());
    }
}
