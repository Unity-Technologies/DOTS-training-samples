using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class BucketAuthoring : MonoBehaviour
{
    class Baker : Baker<BucketAuthoring>
    {
        public override void Bake(BucketAuthoring authoring)
        {
            AddComponent<Position>();
            AddComponent<PickedUpTag>();
            SetComponentEnabled<PickedUpTag>(GetEntity(), false);
            AddComponent<TargetedTag>();
            SetComponentEnabled<TargetedTag>(GetEntity(), false);
            AddComponent<WaterAmount>();
            AddComponent<BucketTag>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}
