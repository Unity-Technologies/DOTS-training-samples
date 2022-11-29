using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FoodAuthor : MonoBehaviour
{

}

class FoodBaker : Baker<FoodAuthor>
{
    public override void Bake(FoodAuthor author)
    {
        AddComponent(new Food());
    }
}
