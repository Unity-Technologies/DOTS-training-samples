using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DeduplicationSystem : SystemBase
{
    protected override void OnCreate()
    {
    }
    protected override void OnUpdate()
    {
        NativeList<Entity> lockedEntities = new NativeList<Entity>(Allocator.TempJob);
        Entities.
            WithStructuralChanges().
            WithAll<NeedsDeduplication>().
            ForEach(
            (Entity farmerEntity, int entityInQueryIndex, in TargetEntity targetEntity) =>
            {
                EntityManager.RemoveComponent<NeedsDeduplication>(farmerEntity);

                if (lockedEntities.Contains(targetEntity.target))
                {
                    // Unassign this entity because it is already taken by another farmer
                    EntityManager.RemoveComponent<TargetEntity>(farmerEntity);
                    EntityManager.RemoveComponent<PickUpCropTask>(farmerEntity);
                    EntityManager.RemoveComponent<DropOffCropTask>(farmerEntity);
                    EntityManager.RemoveComponent<TillTask>(farmerEntity);
                }
                else
                {
                    lockedEntities.Add(targetEntity.target);
                    EntityManager.AddComponent<Assigned>(targetEntity.target);
                }
            }).Run();

        lockedEntities.Dispose();
    }

}
