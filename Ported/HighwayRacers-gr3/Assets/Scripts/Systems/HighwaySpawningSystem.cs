using System.Numerics;
using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct HighwaySpawningSystem : ISystem
{
    
    public void OnCreate(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var highwaySpawnJob = new HighwayPlacement
        {
            ECB = ecb
        };
        
        highwaySpawnJob.Run();
        
        state.Enabled = false;
    }
}

[BurstCompile]
partial struct HighwayPlacement : IJobEntity
{
    public EntityCommandBuffer ECB;
    
    void Execute(in HighwayConfig config)
    {
        int NUM_LANES = 4;
        float LANE_SPACING = 1.9f;
        float MID_RADIUS = 31.46f;
        float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1.0f) / 2.0f;
        float straightPieceLength = ( (float)config.InsideLaneLength - CURVE_LANE0_RADIUS * 4.0f) / 4.0f;
        float3 pos = float3.zero;
        float rot = math.radians(90);
        
        for (int i = 0; i < 8; i++)
        {
            if (i % 2 == 0)
            {
                // straight piece
                quaternion nextRotation = quaternion.EulerXYZ(0, rot, 0);

                var instance = ECB.Instantiate(config.StraightRoadPrefab);
                ECB.SetComponent(instance, new Translation { Value = pos });
                ECB.AddComponent(instance, new NonUniformScale() {Value = new float3(1, 1, straightPieceLength )});
                ECB.SetComponent(instance, new Rotation() { Value = nextRotation });
                pos += math.mul(nextRotation, new float3(0, 0, straightPieceLength));
            }
            else
            {
                // curve piece
                quaternion nextRotation = quaternion.EulerXYZ(0, rot, 0);

                var curveinstance = ECB.Instantiate(config.CurvedRoadPrefab);
                ECB.SetComponent(curveinstance, new Translation { Value = pos });
                ECB.SetComponent(curveinstance, new Rotation() { Value = nextRotation });
                pos += math.mul(nextRotation, new float3(MID_RADIUS, 0, MID_RADIUS));
                rot = math.PI / 2.0f * (i / 2.0f + 1.0f) + math.radians(45);
            }
        }
    }
}