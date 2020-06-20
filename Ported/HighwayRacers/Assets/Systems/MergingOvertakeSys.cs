using Unity.Collections;
using Unity.Entities;

namespace HighwayRacer
{
    [UpdateAfter(typeof(OvertakingSys))]
    public class MergingOvertakeSys : SystemBase
    {
        public const float mergeTime = 1.2f; // number of seconds it takes to fully change lane 

        protected override void OnUpdate()
        {
            var leftECB = new EntityCommandBuffer(Allocator.TempJob);
            var rightECB = new EntityCommandBuffer(Allocator.TempJob);

            var speed = mergeTime * Time.DeltaTime;
            var time = Time.ElapsedTime;

            Entities.WithAll<MergingLeft, OvertakingLeft>().ForEach((Entity ent, ref LaneOffset laneOffset,
                ref TargetSpeed targetSpeed, in DesiredSpeed desiredSpeed) =>
            {
                laneOffset.Val += speed;
                if (laneOffset.Val > 0)
                {
                    laneOffset.Val = 0;
                    targetSpeed.Val = desiredSpeed.Unblocked;
                    // if only there were a method to remove multiple components in one call! (even better, I'd like to add and remove in one go)
                    leftECB.RemoveComponent<MergingLeft>(ent);
                    leftECB.RemoveComponent<OvertakingLeft>(ent);
                    leftECB.RemoveComponent<LaneOffset>(ent);
                }
            }).Run();

            Entities.WithAll<MergingRight, OvertakingRight>().ForEach((Entity ent, ref LaneOffset laneOffset,
                ref TargetSpeed targetSpeed, in DesiredSpeed desiredSpeed) =>
            {
                laneOffset.Val -= speed;
                if (laneOffset.Val < 0)
                {
                    laneOffset.Val = 0;
                    targetSpeed.Val = desiredSpeed.Unblocked;
                    rightECB.RemoveComponent<MergingRight>(ent);
                    leftECB.RemoveComponent<OvertakingRight>(ent);
                    rightECB.RemoveComponent<LaneOffset>(ent);
                }
            }).Run();

            leftECB.Playback(EntityManager);
            leftECB.Dispose();
            rightECB.Playback(EntityManager);
            rightECB.Dispose();
        }
    }
}