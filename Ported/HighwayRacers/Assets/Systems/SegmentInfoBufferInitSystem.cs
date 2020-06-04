using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SegmentInfoBufferInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<RoadInfo>();
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfo)));
    }

    protected override void OnUpdate()
    {
        var roadInfoEntity = GetSingletonEntity<RoadInfo>();
        if (!HasComponent<SegmentsAddedToBufferTag>(roadInfoEntity))
        {
            var roadInfo = GetComponent<RoadInfo>(roadInfoEntity);
            var segmentInfos = EntityManager.AddBuffer<SegmentInfoElement>(roadInfoEntity);
            segmentInfos.ResizeUninitialized(roadInfo.SegmentCount);

            Entities.ForEach((in SegmentInfo segmentInfo, in Entity entity) =>
            {
                segmentInfos[segmentInfo.Order] = new SegmentInfoElement {Entity = entity, SegmentInfo = segmentInfo};
            }).Run();

            EntityManager.AddComponent<SegmentsAddedToBufferTag>(roadInfoEntity);
        }
    }
}