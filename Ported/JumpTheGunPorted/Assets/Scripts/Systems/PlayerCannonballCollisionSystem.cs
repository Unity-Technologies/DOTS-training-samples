using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerCannonballCollisionSystem : SystemBase
{
    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Player), typeof(Translation), typeof(NonUniformScale));
        RequireForUpdate(query);
    }

    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<Player>();
        var playerPos = GetComponent<Translation>(player);
        var playerScale = GetComponent<NonUniformScale>(player);

        // TODO:
        //var ballAabb = new AABB { Center = playerPos.Value.xy, Extents = playerScale.Value.xy / 2 };

        Entities
            .WithName("player_cannonball_collision_test")
            .WithStructuralChanges()
            .WithAll<Cannonball>()
            .ForEach((
                in Translation translation,
                in NonUniformScale scale) =>
            {
                // TODO:
                /*var cannonballAabb = new AABB { Center = translation.Value.xy, Extents = scale.Value.xy / 2 };

                var axisFlip = new float2();
                if (cannonballAabb.Intersects(ballAabb, ref axisFlip))
                {
                    EntityManager.SetComponentData(player, new Velocity2D { Value = ballVelocity.Value * axisFlip });
                }*/
            }).Run();
    }
}