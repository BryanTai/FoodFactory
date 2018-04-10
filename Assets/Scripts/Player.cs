using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour {

    private GameController gameController;


	// Use this for initialization
	void Start () {
        gameController = transform.parent.gameObject.GetComponent<GameController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameController.HandlePlayerFoodCollision(collision.gameObject);

        Debug.Log("Player collided with " + collision.gameObject.name);
    }
}
