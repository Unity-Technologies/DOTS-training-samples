using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial class FarmerFindTileToTillSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<GameConfig>();
        RequireForUpdate<Ground>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var ground = SystemAPI.GetSingletonEntity<Ground>();
        BufferFromEntity<GroundTile> tileBufferEntity = GetBufferFromEntity<GroundTile>();
        DynamicBuffer<GroundTile> tiles;
        if (!tileBufferEntity.TryGetBuffer(ground, out tiles))
            return; // Should always exist, but be cautious.

        var config = SystemAPI.GetSingleton<GameConfig>();
        var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(World.Unmanaged);

        int groundWidth = config.MapSize.x;
        int groundHeight = config.MapSize.y;
        float dt = Time.DeltaTime;

        Random random = new Random((uint)math.abs(Time.ElapsedTime) + 1);
        Entities.WithAll<Farmer>().WithNone<TillGroundTarget>().ForEach((Entity farmer, FarmerIntent intent, MovementAspect mover) =>
        {
            if (intent.value == FarmerIntentState.TillGround)
            {
                //mover.WorldPosition
                int tileIndex;
                if (TryGetRandomOpenTile(ref random, tiles, config.MapSize, mover.Position, out tileIndex))
                {
                    //float2 tileTranslation = GroundUtilities.GetTileTranslation(tileIndex, groundWidth);
                    int2 tileCoords = GroundUtilities.GetTileCoords(tileIndex, groundWidth);
                    mover.DesiredLocation = tileCoords;
                    mover.HasDestination = true;
                    tiles[tileIndex] = new GroundTile() { tileState = GroundTileState.Claimed };
                    ecb.AddComponent<TillGroundTarget>(farmer, new TillGroundTarget { tileIndex = tileIndex, tileTranslation = tileCoords });
                }
            }
        }).Schedule();
    }

    private static bool TryGetRandomOpenTile(ref Random random, in DynamicBuffer<GroundTile> tiles, int2 mapSize, int2 farmerPos, out int tileIndex)
    {
        int searchSize = 8;
        int2 minSearch = new int2(-searchSize, -searchSize);
        int2 maxSearch = new int2(searchSize, searchSize);

        tileIndex = 0;

        int attempts = 8;
        while (attempts > 0)
        {
            var rndPos = random.NextInt2(minSearch, maxSearch);
            var tryPos = farmerPos + rndPos;
            tryPos = math.clamp(tryPos, new int2(0, 0), new int2(mapSize.x-1, mapSize.y - 1));
            //tileIndex = random.NextInt(map.x * map.y);
            tileIndex = MapUtil.MapCordToIndex(mapSize, tryPos);
            GroundTileState tileState = tiles[tileIndex].tileState;
            if (tileState == GroundTileState.Open || tileState == GroundTileState.Tilled) 
                return true;

            --attempts;
        }

        return false;
    }
}