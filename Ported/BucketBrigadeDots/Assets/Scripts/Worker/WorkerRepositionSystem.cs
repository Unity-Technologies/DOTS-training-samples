using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct WorkerRepositionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var teams = SystemAPI.GetComponentLookup<Team>(true);
        
        foreach (var (workerState, worker, workerTransform) in SystemAPI.Query<
                     RefRW<WorkerState>, 
                     RefRO<Worker>, 
                     RefRW<LocalTransform>>())
        {
            if (workerState.ValueRO.Value != WorkerStates.Repositioning)
                continue;

            var teamEntity = worker.ValueRO.Team;
            var team = teams[teamEntity];
            
            var delta = team.FirePosition - workerTransform.ValueRW.Position.xz;
            var length = math.length(delta);
            if (length > 0.1f)
            {
                var direction = math.normalize(delta);

                var moveAmount = direction * 10f * SystemAPI.Time.DeltaTime;
                workerTransform.ValueRW.Position += new float3(moveAmount.x, 0f, moveAmount.y);
            }
            else 
                workerState.ValueRW.Value = WorkerStates.Idle;
        }
    }
}  