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
            AddComponent<BucketActive>();
            AddComponent<BucketFull>();
            AddComponent<Volume>();
            AddComponent<URPMaterialPropertyBaseColor>();

            SetComponentEnabled<BucketActive>(false);
            SetComponentEnabled<BucketFull>(false);
        }
    }
}

public struct Bucket : IComponentData
{
}

public struct BucketActive : IComponentData, IEnableableComponent
{
    
}

public struct BucketFull : IComponentData, IEnableableComponent
{
    
}
