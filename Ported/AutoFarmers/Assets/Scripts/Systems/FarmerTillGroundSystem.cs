using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(FarmerFindTileToTillSystem))]
public partial class FarmerTillGroundSystem : SystemBase
{
    private EntityQuery m_query;

    protected override void OnCreate()
    {
        RequireForUpdate<GameConfig>();
        RequireForUpdate<Ground>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var ground = SystemAPI.GetSingletonEntity<Ground>();
        var config = SystemAPI.GetSingleton<GameConfig>();
        var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(World.Unmanaged);

        //Entities.WithAll<Farmer>().ForEach((Entity entity, ref TillGroundTarget target, ref MovementAspect movement, ref FarmerIntent intent) =>
        //{
        //    if (!movement.AtDesiredLocation)
        //        return;

        //    // Done tilling.
        //    ecb.RemoveComponent<TillGroundTarget>(entity);
        //    intent.value = FarmerIntentState.None;

        //    // Plant a seed.
        //    var plant = ecb.Instantiate(config.PlantPrefab);
        //    ecb.SetComponent<Translation>(plant, new Translation {
        //        Value = new float3(target.tileTranslation.x, .2f, target.tileTranslation.y)
        //    });
        //    tiles[target.tileIndex] = new GroundTile() {
        //        tileState = GroundTileState.Planted,
        //        plantEntityByTile = plant
        //    };
        //}).Schedule();
        Entities.WithAll<Farmer>().WithStructuralChanges().ForEach((Entity entity, MovementAspect movement, ref TillGroundTarget target, ref FarmerIntent intent) =>
        {
            if (!movement.AtDesiredLocation)
                return;

            // Plant a seed.
            var plant = EntityManager.Instantiate(config.PlantPrefab);
            EntityManager.SetComponentData<PlantHealth>(plant, new PlantHealth { Health = 0f });
            EntityManager.SetComponentData<Translation>(plant, new Translation {
                Value = new float3(target.tileTranslation.x, .2f, target.tileTranslation.y)
            });
            var tiles = GetBuffer<GroundTile>(ground);
            tiles[target.tileIndex] = new GroundTile() {
                tileState = GroundTileState.Planted,
                plantEntityByTile = plant
            };

            // Done tilling.
            EntityManager.RemoveComponent<TillGroundTarget>(entity);
            intent.value = FarmerIntentState.None;

        }).Run();
    }
}