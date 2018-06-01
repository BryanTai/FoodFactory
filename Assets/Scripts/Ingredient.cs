using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    bun,
    patty,
    lettuce,
    ketchup
}

[RequireComponent(typeof(Collider))]
public class Ingredient : Food {
    public IngredientType ingredientType { get; set; }

    protected override void OnStart()
    {
        pointAward = 1;
    }

    public override string ToString()
    {
        return ingredientType.ToString();
    }

    
}
