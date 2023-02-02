using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class PlatformAuthoring : MonoBehaviour
{
    public GameObject Stairs;
    public GameObject PlatformFloor;
    public float3 TrainStopPosition;

    class Baker : Baker<PlatformAuthoring>
    {
        public override void Bake(PlatformAuthoring authoring)
        {
            var childSteps = authoring.Stairs.GetComponentsInChildren<Renderer>();
            NativeList<Entity> steps = new NativeList<Entity>(childSteps.Length, Allocator.Persistent);
            foreach (var s in childSteps)
                steps.Add(GetEntity(s.gameObject));

            AddComponent(new Platform()
            {
                TrainStopPosition = authoring.TrainStopPosition,
                Stairs = GetEntity(authoring.Stairs),
                PlatformFloor = GetEntity(authoring.PlatformFloor),
                Steps = steps
            });
        }
    }
}

public struct Platform : IComponentData
{
    public float3 TrainStopPosition;
    public NativeList<Entity> Queues;
    public Entity Stairs;
    public Entity ParkedTrain;
    public Entity PlatformFloor;
    public Line Line;
    public NativeList<Entity> Steps;
    //public NativeArray<URPMaterialPropertyBaseColor> Renderers;
}
