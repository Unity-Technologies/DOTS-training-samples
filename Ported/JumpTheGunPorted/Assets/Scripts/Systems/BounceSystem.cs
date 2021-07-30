using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BounceSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _ECBSys;
    
    protected override void OnCreate()
    {
        _ECBSys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        
        RequireSingletonForUpdate<GameObjectRefs>();
        RequireSingletonForUpdate<HeightBufferElement>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ECBSys.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();

        var boxMapEntity = GetSingletonEntity<HeightBufferElement>(); // ASSUMES the singleton that has height buffer also has occupied
        var heightMap = EntityManager.GetBuffer<HeightBufferElement>(boxMapEntity);
        var occupiedMap = EntityManager.GetBuffer<OccupiedBufferElement>(boxMapEntity);

        // need terrain length to calculate index to our height map array
        var refs = this.GetSingleton<GameObjectRefs>();
        var config = refs.Config.Data;
        int terrainLength = config.TerrainLength;
        int terrainWidth = config.TerrainWidth;
        float midPlaneY = (config.MinTerrainHeight + config.MaxTerrainHeight) / 2;

        UnityEngine.Ray ray = refs.Camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        float3 mouseWorldPos = new float3(0, midPlaneY, 0);
        float t = (midPlaneY - ray.origin.y) / ray.direction.y;
        mouseWorldPos.x = ray.origin.x + t * ray.direction.x;
        mouseWorldPos.z = ray.origin.z + t * ray.direction.z;
        float3 mouseLocalPos = mouseWorldPos;

        Entities
            .WithName("bounce_update")
            .WithAll<Player>()
            .WithReadOnly(heightMap)
            .WithReadOnly(occupiedMap)
#if NO_BURST
            .WithoutBurst()
#endif
            .ForEach((Entity entity, int entityInQueryIndex, ref ParabolaTValue tValue, in Translation translation) =>
            {
                if (tValue.Value < 0)
                {
                    // solving parabola path

                    //start at player and move towards the box the mouse is over
                    float2 currentPos = new float2(
                        math.clamp(math.round(translation.Value.x), 0, terrainLength - 1),
                        math.clamp(math.round(translation.Value.z), 0, terrainWidth - 1)
                    );
                    int startBoxCol = (int)currentPos.x;
                    int startBoxRow = (int)currentPos.y;
                    float startY = heightMap[startBoxRow * terrainLength + startBoxCol] + Player.Y_OFFSET;

                    // target box is one move forward in the direction of the mouse
                    // getting local world position of mouse.  Is where camera ray intersects xz plane with y = 
                    float2 mouseBoxPos = new float2(
                        math.clamp(math.round(mouseLocalPos.x), 0, terrainLength - 1),
                        math.clamp(math.round(mouseLocalPos.z), 0, terrainWidth - 1)
                    );

                    float2 movePos = mouseBoxPos;
                    if (math.abs(mouseBoxPos.x - currentPos.x) > 1 || math.abs(mouseBoxPos.y - currentPos.y) > 1)
                    {
                        // mouse position is too far away.  Find closest position
                        movePos = currentPos;
                        if (mouseBoxPos.x != currentPos.x)
                        {
                            movePos.x += mouseBoxPos.x > currentPos.x ? 1 : -1;
                        }
                        if (mouseBoxPos.y != currentPos.y)
                        {
                            movePos.y += mouseBoxPos.y > currentPos.y ? 1 : -1;
                        }

                    }

                    int endBoxCol = (int)movePos.x;
                    int endBoxRow = (int)movePos.y;

                    // don't move if target is occupied by checking some map of where tanks are?
                    if (occupiedMap[endBoxRow * terrainLength + endBoxCol])
                    {
                        // stay where you are
                        endBoxCol = startBoxCol;
                        endBoxRow = startBoxRow;
                    }

                    float endY = heightMap[endBoxRow * terrainLength + endBoxCol] + Player.Y_OFFSET;

                    float height = math.max(startY, endY);

                    // make height max of adjacent boxes when moving diagonally
                    if (startBoxCol != endBoxCol && startBoxRow != endBoxRow)
                    {
                        height = math.max(height, math.max(heightMap[endBoxRow * terrainLength + startBoxCol], heightMap[startBoxRow * terrainLength + endBoxCol]));
                    }
                    height += Player.BOUNCE_HEIGHT;

                    JumpTheGun.Parabola.Create(startY, height, endY, out float a, out float b, out float c);

                    float2 startPos = new float2(startBoxCol, startBoxRow);
                    float2 endPos = new float2(endBoxCol, endBoxRow);
                    float dist = math.distance(startPos, endPos);
                    float duration = math.max(1, dist) * Player.BOUNCE_BASE_DURATION;

                    // determine forward vector for the full parabola
                    float3 forward = new float3(endBoxCol, 0, endBoxRow) - new float3(startBoxCol, 0, startBoxRow);

                    // construct the parabola data struct for use in the movement system
                    parallelWriter.AddComponent(entityInQueryIndex, entity, new Parabola
                    {
                        StartY = startY,
                        Height = height,
                        EndY = endY,
                        A = a,
                        B = b,
                        C = c,
                        Duration = duration,
                        Forward = forward
                    });

                    // start the parabola movement
                    tValue.Value = 0;
                }
            })
#if NO_PARALLEL
            .Schedule();
#else
            .ScheduleParallel();
#endif

        _ECBSys.AddJobHandleForProducer(Dependency);
    }
}