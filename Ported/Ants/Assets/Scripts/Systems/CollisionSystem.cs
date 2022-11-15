using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(AntMovementSystem))]
[BurstCompile]
public partial struct CollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((TransformAspect antTransform, RefRW<Ant> ant) in SystemAPI.Query<TransformAspect, RefRW<Ant>>())
        {
            foreach ((LocalToWorldTransform wallTransform, Wall wall ) in SystemAPI.Query<LocalToWorldTransform,Wall>())
            {
                var sqrDistance =math.distancesq(wallTransform.Value.Position,antTransform.Position) ;
                if (sqrDistance <= wallTransform.Value.Scale) //COLLIDED
                {
                    ant.ValueRW.Angle += 90f;
                }
            }
        }
    }
}