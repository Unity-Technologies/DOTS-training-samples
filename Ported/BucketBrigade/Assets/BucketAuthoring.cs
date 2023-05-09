using Unity.Entities;
using UnityEngine;

public class BucketAuthoring : MonoBehaviour
{
    class Baker : Baker<BucketAuthoring>
    {
        public override void Bake(BucketAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Bucket>(entity);
        }
    }
}

public struct Bucket : IComponentData
{

}