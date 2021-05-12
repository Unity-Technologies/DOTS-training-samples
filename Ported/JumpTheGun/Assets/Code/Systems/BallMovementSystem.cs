using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SpawnerSystem))]
public class BallMovementSystem : SystemBase
{


    public static int CreateRandomStartPoint(int range)
    {

        var random = new System.Random();

        int value = random.Next(0, range);


        return value;
    }


    protected override void OnUpdate()
    {

        /*var player = GetSingletonEntity<Player>();
        var startX = CreateRandomStartPoint(8);
        var starty = CreateRandomStartPoint(8);

        float3 targetPos = new float3(0.0f, 0.0f, 0.0f);


        Entities
        .WithAll<Platform>()
        .ForEach((ref BoardPosition boardPosition, ref TargetPosition translation) =>
        {
           
            if (boardPosition.Value.x == startX && boardPosition.Value.y == starty)
            {
                targetPos += translation.Value;
            }
        }).Run();

        SetComponent(player, new Translation { Value = targetPos });*/

    }
}
