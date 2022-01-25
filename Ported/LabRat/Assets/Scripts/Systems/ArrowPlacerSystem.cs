using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

//[UpdateAfter(typeof(MapSpawningSystem))]
//[UpdateAfter(typeof(PlayerInputSystem))]
public partial class ArrowPlacerSystem : SystemBase
{

    private EndSimulationEntityCommandBufferSystem mECBSystem;

    protected override void OnCreate()
    {
        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var config = GetSingleton<Config>();
        
        float2 cellSize = new float2(config.MapWidth, config.MapHeight);

        float time = (float)Time.ElapsedTime;

        Entities
            .WithAll<Player>()
            .ForEach((Entity playerEntity, int nativeThreadIndex, ref PlayerSpawnArrow shouldSpawn, ref ArrowsDeployed arrowsDeployed, in CursorPosition playerPos, in Color playerColor) =>
            {
                if (!shouldSpawn.Value)
                {
                    return;
                }

                var coordinate = CalculateProjected(playerPos.Value, cellSize, out var direction);
                if (direction != DirectionEnum.None &&
                    coordinate.x > 0 && coordinate.x < cellSize.x &&
                    coordinate.y > 0 && coordinate.y < cellSize.y)
                {
                    var arrow = ecb.Instantiate(nativeThreadIndex, config.ArrowPrefab);
                    ecb.SetComponent(nativeThreadIndex, arrow, new Arrow { PlacedTime = time });
                    ecb.SetComponent(nativeThreadIndex, arrow, new Tile(coordinate));
                    ecb.SetComponent(nativeThreadIndex, arrow, new Direction(direction));
                    ecb.SetComponent(nativeThreadIndex, arrow, new URPMaterialPropertyBaseColor { Value = playerColor.Value });
                    ecb.SetComponent(nativeThreadIndex, arrow, new PlayerOwned { Owner = playerEntity });
                    ecb.SetComponent(nativeThreadIndex, arrow, new Translation
                    {
                        Value = new float3(coordinate.x, .1f, coordinate.y)
                    });
                    float rot = direction switch
                    {
                        DirectionEnum.North => math.PI,
                        DirectionEnum.East => math.PI*0.5f,
                        DirectionEnum.West => math.PI*-0.5f,
                        _ => 0f
                    };
                    ecb.SetComponent(nativeThreadIndex, arrow, new Rotation
                    {
                        Value = quaternion.Euler(math.PI*0.5f, rot, 0)
                    });
                }

                shouldSpawn.Value = false;
            }).ScheduleParallel();
        mECBSystem.AddJobHandleForProducer(Dependency);
    }

    public static int2 CalculateProjected(float2 coord, float2 cellSize, out DirectionEnum cellDirection)
    {
        cellDirection = DirectionEnum.North;

        var intCoord = new int2((int)coord.x, (int)coord.y);
        var tilePt = coord - intCoord- 0.5f;
        if (math.abs(tilePt.y) > math.abs(tilePt.x))
            cellDirection = tilePt.y < 0 ? DirectionEnum.North : DirectionEnum.South;
        else
            cellDirection = tilePt.x > 0 ? DirectionEnum.East : DirectionEnum.West;

        return intCoord;

    }
}
