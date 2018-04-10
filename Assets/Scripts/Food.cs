using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Parent Class of Ingredient and Meal
public abstract class Food : MonoBehaviour {

    public int pointAward { get; protected set; }
    private float lifetimeSeconds = 8; //TODO fiddle with this

    // Use this for initialization
    void Start () {
        OnStart();
	}

    abstract protected void OnStart();
	
	// Update is called once per frame
	void Update () {
        lifetimeSeconds -= Time.deltaTime;
        //TODO fade or blink when ingredient nears the end of it's lifetime
        if (lifetimeSeconds < 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision);
    }
    protected void OnCollision(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit the player!");
            Destroy(this.gameObject);
        }
    }
}
