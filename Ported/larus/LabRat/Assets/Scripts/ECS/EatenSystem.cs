using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class EatenSystem : JobComponentSystem
{
    private BoardSystem m_Board;
    private EndSimulationEntityCommandBufferSystem m_Buffer;

    protected override void OnCreate()
    {
        m_Board = World.GetExistingSystem<BoardSystem>();
        m_Buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var board = GetSingleton<BoardDataComponent>();
        var cellMap = m_Board.CellMap;
        var homebaseMap = m_Board.HomeBaseMap;
        var ecb = m_Buffer.CreateCommandBuffer().ToConcurrent();

        var job = Entities.WithAll<EatenComponentTag>().ForEach((Entity entity, int entityInQueryIndex, in Translation position) =>
        {
            Util.PositionToCoordinates(position.Value, board, out var cellCoord, out var cellIndex);
            CellComponent cell = new CellComponent();
            if (!cellMap.TryGetValue(cellIndex, out cell))
                return;

            if ((cell.data & CellData.HomeBase) == CellData.HomeBase)
            {
                // TODO: port the score tracking to ECS
                var playerId = homebaseMap[cellIndex];
                ecb.DestroyEntity(entityInQueryIndex, entity);
                //PlayerCursor.GetPlayerData(playerId).mouseCount++;
            }

            if ((cell.data & CellData.Eater) == CellData.Eater)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }
        }).WithReadOnly(cellMap).WithReadOnly(homebaseMap).Schedule(inputDeps);
        m_Buffer.AddJobHandleForProducer(job);
        return job;
    }
}
