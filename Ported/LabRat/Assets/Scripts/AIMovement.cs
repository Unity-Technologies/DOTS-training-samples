using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class AIMovement : SystemBase
{
    static readonly float AISpeed = 10f;
    EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_HolePositionQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        
        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Arrow)}
        };
        m_HolePositionQuery = EntityManager.CreateEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var randomSeed = (uint)System.DateTime.Now.Ticks;
        var boardSize = GetSingleton<GameInfo>().boardSize;
        var prefab = GetSingleton<GameInfo>().ArrowPrefab;
        
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithAll<Timer, AICursor>().ForEach((Entity e, int entityInQueryIndex, in Timer timer) =>
        {
            if(timer.Value <= 0f)
                ecb.RemoveComponent<Timer>(entityInQueryIndex, e);
        }).ScheduleParallel();
        
        // get toutes les fleches Entity
        var arrowsEntities = m_HolePositionQuery.ToEntityArray(Allocator.TempJob);
        var arrowsPositions = m_HolePositionQuery.ToComponentDataArray<Arrow>(Allocator.TempJob);
        
        Entities.WithNone<Timer>().ForEach((Entity e, int entityInQueryIndex, ref AICursor cursor, ref Position position, ref DynamicBuffer<PlayerArrow> arrows, in PlayerTransform playerIndex) =>
        {
            var direction = cursor.Destination - position.Value;
            var distance = math.lengthsq(direction);
            var movement = deltaTime * AISpeed * math.normalize(direction);
            var movementLength = math.lengthsq(movement);

            if (distance == 0 || movementLength >= distance)
            {
                position.Value = cursor.Destination;
                var random = new Random((uint)(randomSeed + entityInQueryIndex));
                cursor.Destination = new int2(random.NextInt(0, boardSize.x), random.NextInt(0, boardSize.y));
                ecb.AddComponent(entityInQueryIndex, e, new Timer(){Value = random.NextFloat(1f,2f)});
                //TODO : get the cell and add a Direction and player to it?
                PlaceArrow(arrows, 
                    new int2((int)math.round(position.Value.x), (int)math.round(position.Value.y)), 
                    boardSize.y, 
                    playerIndex.Index, 
                    ecb, 
                    entityInQueryIndex, 
                    prefab,
                    arrowsEntities, arrowsPositions);
            }
            else
            {
                position.Value += movement;
            }

        }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        var a = arrowsEntities.Dispose(Dependency);
        var b = arrowsPositions.Dispose(Dependency);
        Dependency = JobHandle.CombineDependencies(a, b);
    }

    public static void PlaceArrow(DynamicBuffer<PlayerArrow> arrows, 
        int2 tilePosition, int boardSize, int playerIndex, 
        EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity prefab,
        NativeArray<Entity> existingArrows, NativeArray<Arrow> existingArrowsPositions)
    {
        int tileIndex = tilePosition.x + tilePosition.y * boardSize;
        for (int i = 0; i < arrows.Length; i++)
        {
            if (tileIndex == arrows[i].TileIndex)
            {
                for (int j = 0; j < existingArrows.Length; j++)
                {
                    if (existingArrowsPositions[j].Position == tileIndex)
                    {
                        ecb.DestroyEntity(sortKey, existingArrows[j]);
                        arrows.RemoveAt(i);
                        return;
                    }
                }
            }
        }
        if (arrows.Length == 3)
        {
            for (int j = 0; j < existingArrows.Length; j++)
            {
                if (existingArrowsPositions[j].Position == arrows[0].TileIndex)
                {
                    ecb.DestroyEntity(sortKey, existingArrows[j]);
                }
            }
            arrows.RemoveAt(0);
        }

        
        
        var entity = ecb.Instantiate(sortKey, prefab);
        ecb.SetComponent(sortKey, entity, new Translation() {Value = new float3(tilePosition.x, 0.61f, tilePosition.y)});
        ecb.SetComponent(sortKey, entity, PlayerUtility.ColorFromPlayerIndex(playerIndex));
        ecb.SetComponent(sortKey, entity, new Arrow() {Position = tileIndex});
        arrows.Add(new PlayerArrow() { TileIndex = tileIndex });
    }
}