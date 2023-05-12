using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TeamBucketAuthoring : MonoBehaviour
{
    class Baker : Baker<TeamBucketAuthoring>
    {
        public override void Bake(TeamBucketAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            
            AddComponent(entity, new PostTransformMatrix()); 
            AddComponent(entity, new TeamBucket());
        }
    }
}

public struct TeamBucket : IComponentData
{
    public Entity TargetTeambotEntity;
}