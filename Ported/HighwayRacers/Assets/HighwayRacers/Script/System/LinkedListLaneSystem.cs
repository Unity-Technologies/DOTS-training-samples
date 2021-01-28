using Unity.Entities;


public class LinkedListLaneSystem : SystemBase
{
    private EntityQuery NeedsRefresh;

    protected override void OnCreate()
    {
        NeedsRefresh = GetEntityQuery(typeof(LinkedListLane), typeof(CarMovement));
    }

    protected override void OnUpdate()
    {
        
    }
}
