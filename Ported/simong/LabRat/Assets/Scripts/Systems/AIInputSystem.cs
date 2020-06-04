using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public struct AIInfoComponent : IComponentData
{
    public int PlayerID;
    public float2 TargetPosition;
    public int2 CellPosition;
    public bool Placed;
}

[AlwaysUpdateSystem]
class AIInputSystem : SystemBase
{
    private EntityCommandBufferSystem commandBufferSystem;
    private EntityArchetype arrowRequestArchetype;
    private Random m_Random = new Random(1234);
    private EntityQuery m_AIPlayerQuery;
    private EntityQuery m_PlayerBaseQuery;

    protected override void OnCreate()
    {
        arrowRequestArchetype = EntityManager.CreateArchetype(typeof(ArrowRequest));
        
        m_AIPlayerQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<AIInfoComponent>()
            }
        });
        
        m_PlayerBaseQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<PlayerBase>(),
                ComponentType.ReadOnly<Position2D>(), 
            }
        });
        
        commandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    
    private NativeArray<float2> m_cursorPositions = new NativeArray<float2>(4, Allocator.Persistent);

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_cursorPositions.Dispose();
    }

    protected override void OnUpdate()
    {
        if (!ConstantData.Instance)
            return;
        
        int2 uiSize = new int2(ConstantData.Instance.UICanvasDimensions.x, ConstantData.Instance.UICanvasDimensions.y);
        int2 gridSize = new int2(ConstantData.Instance.BoardDimensions.x, ConstantData.Instance.BoardDimensions.y);
        float2 cellSize = new float2(ConstantData.Instance.CellSize.x, ConstantData.Instance.CellSize.y);
        int numPlayers = ConstantData.Instance.NumPlayers;
        int numAI = numPlayers - 1;
        
        if (m_AIPlayerQuery.CalculateEntityCount() < numAI)
        {
            for (int i = 0; i < numAI; i++)
            {
                Entity e = EntityManager.CreateEntity();
                EntityManager.AddComponentData(e, new AIInfoComponent
                {
                    PlayerID = i + 1,
                    TargetPosition = uiSize/2
                });
                
                m_cursorPositions[i] = uiSize / 2;
            }
        }
        else
        {
            NativeArray<float2> cursorPositions = m_cursorPositions;
            Random r = new Random(m_Random.NextUInt());
            Camera c = Camera.main;
            int2 cPixelSize = new int2(c.pixelWidth, c.pixelHeight);
            
            var ecb = commandBufferSystem.CreateCommandBuffer();
            
            var baseComponents = m_PlayerBaseQuery.ToComponentDataArrayAsync<PlayerBase>(Allocator.TempJob, out var playerBaseComponentsHandle);
            var basePositionComponents = m_PlayerBaseQuery.ToComponentDataArrayAsync<Position2D>(Allocator.TempJob, out var playerBasePositionHandle);
            
            Dependency = JobHandle.CombineDependencies(Dependency, playerBaseComponentsHandle);
            Dependency = JobHandle.CombineDependencies(Dependency, playerBasePositionHandle);
            
            Entities
                .WithoutBurst()
                .WithDeallocateOnJobCompletion(baseComponents)
                .ForEach((Entity aiPlayerEntity, ref AIInfoComponent aiInfo) =>
                {
                    if (r.NextFloat(100f) > 99.5f)
                    {
                        aiInfo.CellPosition = new int2(r.NextInt(0, gridSize.x), r.NextInt(0, gridSize.y));
                        aiInfo.TargetPosition = new int2(Utility.GridCoordinatesToScreenPos(c, aiInfo.CellPosition, cellSize) * cPixelSize);
                        aiInfo.Placed = false;
                    }

                    cursorPositions[aiInfo.PlayerID] = math.lerp(cursorPositions[aiInfo.PlayerID], aiInfo.TargetPosition, 0.1f);

                    if (math.length(cursorPositions[aiInfo.PlayerID] - aiInfo.TargetPosition) < 1f && aiInfo.Placed == false)
                    {
                        int2 position = aiInfo.CellPosition;
                        GridDirection dir = (GridDirection) (1 << m_Random.NextInt(0, 3));
                        
                        for (int i = 0; i < baseComponents.Length; i++)
                        {
                            if (baseComponents[i].PlayerID == aiInfo.PlayerID)
                            {
                                dir = Utility.DirectionForVector(basePositionComponents[i].Value - position);
                            }
                        }
                        
                        int playerId = aiInfo.PlayerID;
                        Entity e = ecb.CreateEntity(arrowRequestArchetype);
                        ecb.SetComponent(e, new ArrowRequest
                        {
                            Direction = dir,
                            OwnerID = playerId,
                            Position = position
                        });
                        
                        aiInfo.Placed = true;
                    }
                }).Run();

            for (int i = 1; i < 4; i++)
                UIHelper.Instance.SetCursorPosition(i, new int2(cursorPositions[i]));
            
            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}