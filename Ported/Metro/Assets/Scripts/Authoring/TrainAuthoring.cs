using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class TrainAuthoring : MonoBehaviour
{
    public int carriageAmount = 5;
    public GameObject carriagePrefab;
}

class TrainAuthoringBaker : Baker<TrainAuthoring>
{
    public override void Bake(TrainAuthoring authoring)
    {
        AddComponent<SpeedComponent>();
        AddComponent<URPMaterialPropertyBaseColor>();
        AddComponent(new Train
        {
            State = TrainState.EnRoute,
            Destination = new float2(0,1)
        });
        AddComponent(new TrainSpawn
        {
            CarriageSpawn = GetEntity(authoring.carriagePrefab),
            CarriageCount = authoring.carriageAmount
        });
    }
}
 