using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial struct FollowSystem : ISystem
{
    ComponentDataFromEntity<LocalToWorld> localToWorldFromEntity;

    public void OnCreate(ref SystemState state)
    {
        localToWorldFromEntity = state.GetComponentDataFromEntity<LocalToWorld>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        localToWorldFromEntity.Update(ref state);
        foreach (var follower in SystemAPI.Query<FollowAspect>())
        {
            follower.Position = localToWorldFromEntity.GetRefRO(follower.ThingToFollow).ValueRO.Position + follower.Offset;
        }
    }
}
