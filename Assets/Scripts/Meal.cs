using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MealType
{
    burger
}

public class Meal : Food {
    public MealType mealType { get; set; }

    protected override void OnStart()
    {
        pointAward = 10;
    }

    public override string ToString()
    {
        return mealType.ToString();
    }
}
