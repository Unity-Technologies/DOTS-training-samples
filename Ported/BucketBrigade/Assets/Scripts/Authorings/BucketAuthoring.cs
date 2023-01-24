using Unity.Entities;
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
            AddComponent<TargetedTag>();
            AddComponent<WaterAmount>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}
