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
    private float lifetimeSeconds = 6; //TODO fiddle with this

	// Use this for initialization
	void Start () {
		
	}

    private void Update()
    {
        lifetimeSeconds -= Time.deltaTime;
        //TODO fade or blink when ingredient nears the end of it's lifetime
        if (lifetimeSeconds < 0)
        {
            Destroy(this.gameObject);
        }
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
