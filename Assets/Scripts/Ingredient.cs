using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    bun,
    patty,
    lettuce,
    tomato
}

[RequireComponent(typeof(Collider))]
public class Ingredient : MonoBehaviour {

    public IngredientType ingredientType { get; set; }

	// Use this for initialization
	void Start () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit the player!");
            Destroy(this.gameObject);
        }
    }
}
