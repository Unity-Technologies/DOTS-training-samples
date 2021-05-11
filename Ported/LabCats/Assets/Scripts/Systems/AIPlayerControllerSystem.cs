using Unity.Entities;
using Unity.Transforms;

public class AIPlayerControllerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Dependency = Entities.ForEach((Entity e, ref AITargetCell aiTargetCell, ref ArrowReference arrows, ref Translation translation) => 
        {
            //Fetch DynamicBuffer<GridCellContent> + GridCellSize + GridSize from the Grid singleton  
            
        }).ScheduleParallel(Dependency);
    }
}
