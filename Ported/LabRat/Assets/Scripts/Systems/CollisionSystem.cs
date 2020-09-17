using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class CollisionSystem : SystemBase
{

    private EntityQuery m_CatQuery;
    private EntityQuery m_MouseQuery;

    private EntityQuery m_PlayerQuery;
    private NativeArray<Position> m_PlayerPositions;
    private NativeArray<Score> m_PlayerScores;

    private EntityQuery m_HoleQuery;

    protected override void OnCreate()
    {
        m_PlayerQuery = GetEntityQuery(typeof(Score), ComponentType.ReadOnly<BaseTag>());
        m_PlayerPositions = m_PlayerQuery.ToComponentDataArray<Position>(Allocator.Persistent);
        m_PlayerScores = m_PlayerQuery.ToComponentDataArray<Score>(Allocator.Persistent);

        m_HoleQuery = GetEntityQuery(typeof(Position), ComponentType.ReadOnly<Hole>());
    }

    protected override void OnUpdate()
    {

        m_CatQuery = GetEntityQuery(typeof(Position), ComponentType.ReadOnly<CatTag>());
        m_MouseQuery = GetEntityQuery(typeof(Position), ComponentType.ReadOnly<MouseTag>());
        NativeArray<Position> catPositions = m_CatQuery.ToComponentDataArray<Position>(Allocator.TempJob);
        NativeArray<Position> mousePositions = m_MouseQuery.ToComponentDataArray<Position>(Allocator.TempJob);
        NativeArray<Entity> mouseEntities = m_MouseQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Position> holePositions = m_HoleQuery.ToComponentDataArray<Position>(Allocator.TempJob);

        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer();
        const float threshold = 0.25f;

        // Loop each player entity 
        Entities.WithDisposeOnCompletion(mousePositions).WithAll<BaseTag>().ForEach((ref Score score, in Position pos) =>
        {
            // mouse x player
            for (int i = 0; i < mousePositions.Length; i++)
            {
                float2 diff = mousePositions[i].Value - pos.Value;
                float distance = (diff.x * diff.x) + (diff.y * diff.y);
                if (distance < threshold)
                {
                    score.Value++;
                    ecb.DestroyEntity(mouseEntities[i]);
                }
            }

        }).Run();

        // Loop each mouse entity 
        Entities.WithDisposeOnCompletion(catPositions).WithAll<MouseTag>().ForEach((Entity entity, ref Direction dir, in Position pos) =>
        {

            // mouse x hole 
            // foreach (var hole in holePositions)
            for (int i = 0; i < holePositions.Length; i++)
            {
                float2 diff = pos.Value - holePositions[i].Value;
                float distance = (diff.x * diff.x) + (diff.y * diff.y);
                if (distance < threshold)
                {
                    ecb.DestroyEntity(entity);
                    return;
                }
            }

            // mouse x cat
            //foreach (var cat in catPositions)
            for (int i = 0; i < catPositions.Length; i++)
            {
                float2 diff = catPositions[i].Value - pos.Value;
                float distance = (diff.x * diff.x) + (diff.y * diff.y);
                if (distance < threshold)
                {
                    ecb.DestroyEntity(entity);
                    return;
                }
            }

            // To do: mouse x arrow, direction of mouse will be changed after mouse hit arrow 

        }).Run();


        // Loop each cat entity 
        Entities.WithAll<CatTag>().ForEach((Entity entity, ref Direction dir, in Position pos) =>
        {
            // cat x hole 
            for (int i = 0; i < holePositions.Length; i++)
            {
                float2 diff = pos.Value - holePositions[i].Value;
                float distance = (diff.x * diff.x) + (diff.y * diff.y);
                if (distance < threshold)
                {
                    ecb.DestroyEntity(entity);
                    return;
                }
            }

            // To do: cat x arrow, direction of cat will be changed after cat hit arrow 

        }).Run();
    }

}
