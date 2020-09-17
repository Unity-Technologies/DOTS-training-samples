using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class NpcInputSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CellData>();
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<NpcInput>()));
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        EntityCommandBufferSystem sys = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();

        var boardSize = GetSingleton<BoardSize>();

        var cellDataEntity = GetSingletonEntity<CellData>();
        var cellData = EntityManager.GetComponentObject<CellData>(cellDataEntity);

        var cells = cellData.cells;
        var arrows = cellData.directions;

        Entities.ForEach((ref NpcInput npc) => {
            npc.timeSinceLastMove += deltaTime;

            if (npc.timeSinceLastMove > npc.nextMoveDelay)
            {
                var pos = new int2(npc.random.NextInt2(boardSize.Value));
                var arrayPos = pos.y * boardSize.Value.x + pos.x;

                if (arrows[arrayPos] == 0)
                {
                    npc.timeSinceLastMove = 0f;
                    npc.nextMoveDelay = npc.random.NextFloat(5f, 15f);

                    var direction = (DirectionEnum)npc.random.NextInt(4);

                    ArrowPlacementRequest request = new ArrowPlacementRequest { player = npc.player, position = pos, direction = direction, remove = false };
                    ecb.AddComponent(cells[arrayPos], request);
                }
            }
        }).Schedule();
    }
}
