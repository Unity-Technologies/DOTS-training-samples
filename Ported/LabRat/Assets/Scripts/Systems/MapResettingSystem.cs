using Unity.Entities;

[UpdateBefore(typeof(MapSpawningSystem))]
public partial class MapResettingSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;

    private EntityQuery creaturesToDestroy;
    private EntityQuery exitsToDestroy;
    private EntityQuery arrowsToDestroy;
    private EntityQuery tilesToDestroy;
    private EntityQuery wallsToDestroy;
    private EntityQuery mapDataToDestroy;
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<MapReset>();
        ecbSystem = World.GetExistingSystem<EntityCommandBufferSystem>();

        creaturesToDestroy = GetEntityQuery(ComponentType.ReadOnly<Creature>());
        exitsToDestroy = GetEntityQuery(ComponentType.ReadOnly<Exit>());
        arrowsToDestroy = GetEntityQuery(ComponentType.ReadOnly<Arrow>());
        tilesToDestroy = GetEntityQuery(ComponentType.ReadOnly<MapTile>());
        wallsToDestroy = GetEntityQuery(ComponentType.ReadOnly<Wall>());
        mapDataToDestroy = GetEntityQuery(ComponentType.ReadOnly<MapData>());
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer();
        var entity = GetSingletonEntity<MapReset>();
        
        ecb.DestroyEntitiesForEntityQuery(creaturesToDestroy);
        ecb.DestroyEntitiesForEntityQuery(exitsToDestroy);
        ecb.DestroyEntitiesForEntityQuery(arrowsToDestroy);
        ecb.DestroyEntitiesForEntityQuery(tilesToDestroy);
        ecb.DestroyEntitiesForEntityQuery(wallsToDestroy);
        ecb.DestroyEntitiesForEntityQuery(mapDataToDestroy);
        
        ecb.RemoveComponent<MapReset>(entity);
        ecb.RemoveComponent<MapWasSpawned>(entity);
        
        
    }
}