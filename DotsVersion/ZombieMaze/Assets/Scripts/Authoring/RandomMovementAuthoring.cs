using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RandomMovementAuthoring : MonoBehaviour
{
    
}

class RandomMovementAuthoringBaker : Baker<RandomMovementAuthoring>
{
    public override void Bake(RandomMovementAuthoring authoring)
    {
        AddComponent<RandomMovement>();
    }
}
