using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public partial class CannonBallUpdateSystem : SystemBase
{
    private EntityCommandBufferSystem ecsSystem;
    protected override void OnCreate()
    {
        ecsSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var entityElement = GetSingletonEntity<HeightElement>();
        var heightBuffer = this.GetBuffer<HeightElement>(entityElement, false);
        var terrainData = GetSingleton<TerrainData>();

        var ecb = ecsSystem.CreateCommandBuffer();
        var ecbParallel = ecb.AsParallelWriter();
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in Translation translation, in CannonBallTag tag, in NormalizedTime time) =>
            {
                if (time.value >= 1.0f)
                {
                    var boxPos = TerrainUtility.BoxFromLocalPosition_Unsafe(translation.Value);
                    int index = boxPos.x + boxPos.y * terrainData.TerrainWidth;
                    heightBuffer[index] = math.max(Constants.HEIGHT_MIN, heightBuffer[index] - Constants.boxHeightDamage);
                    ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                }
            }).Schedule();
            ecsSystem.AddJobHandleForProducer(Dependency);
    }

}