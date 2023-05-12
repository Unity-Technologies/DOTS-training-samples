using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class OmniBucketAuthoring : MonoBehaviour
{
    class Baker : Baker<OmniBucketAuthoring>
    {
        public override void Bake(OmniBucketAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            
            AddComponent(entity, new PostTransformMatrix()); 
            AddComponent(entity, new OmniBucket());
        }
    }
}

public struct OmniBucket : IComponentData
{
    public Entity TargetOmniBotEntity;
}