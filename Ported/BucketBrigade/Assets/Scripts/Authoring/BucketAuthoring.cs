using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Color = Components.Color;

public class BucketAuthoring : MonoBehaviour
{
    class Baker : Baker<BucketAuthoring>
    {
        public override void Bake(BucketAuthoring bucketAuthoring)
        {
            AddComponent<Bucket>();
            AddComponent<Volume>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}

public struct Bucket : IComponentData
{
    public bool isActive;
    public bool isFull;
}