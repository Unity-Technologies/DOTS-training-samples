using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TrainAuthoring : MonoBehaviour
{
}

public class TrainBaker : Baker<TrainAuthoring>
{
    public override void Bake(TrainAuthoring authoring)
    {
        AddComponent<Train>();
    }
}
