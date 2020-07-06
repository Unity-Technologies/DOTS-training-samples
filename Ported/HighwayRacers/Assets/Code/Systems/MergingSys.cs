using Unity.Collections;
using Unity.Entities;

namespace HighwayRacer
{
    // todo: could be combined with MergingSpeedSys
    [UpdateAfter(typeof(AvoidanceAndSpeedOvertakeSys))]
    public class MergingSys : SystemBase
    {
        public const float mergeTime = 1.2f; // number of seconds it takes to fully change lane 

        protected override void OnUpdate()
        {
            var leftECB = new EntityCommandBuffer(Allocator.TempJob);
            var rightECB = new EntityCommandBuffer(Allocator.TempJob);

            var speed = mergeTime * Time.DeltaTime;
            var time = Time.ElapsedTime;

            Entities.WithAll<MergingLeft>().WithNone<OvertakingLeft>().ForEach((Entity ent, ref LaneOffset laneOffset,
                ref TargetSpeed targetSpeed, in DesiredSpeed desiredSpeed) =>
            {
                laneOffset.Val += speed;
                if (laneOffset.Val > 0)
                {
                    laneOffset.Val = 0;
                    targetSpeed.Val = desiredSpeed.Overtake;
                    // if only there were a method to remove multiple components in one call! (even better, I'd like to add and remove in one go)
                    leftECB.RemoveComponent<MergingLeft>(ent);
                    leftECB.RemoveComponent<LaneOffset>(ent);
                    leftECB.AddComponent<OvertakingRight>(ent, new OvertakingRight() {Time = time});
                }
            }).Run();

            Entities.WithAll<MergingRight>().WithNone<OvertakingRight>().ForEach((Entity ent, ref LaneOffset laneOffset,
                ref TargetSpeed targetSpeed, in DesiredSpeed desiredSpeed) =>
            {
                laneOffset.Val -= speed;
                if (laneOffset.Val < 0)
                {
                    laneOffset.Val = 0;
                    targetSpeed.Val = desiredSpeed.Overtake;
                    rightECB.RemoveComponent<MergingRight>(ent);
                    rightECB.RemoveComponent<LaneOffset>(ent);
                    rightECB.AddComponent<OvertakingLeft>(ent, new OvertakingLeft() {Time = time});
                }
            }).Run();

            leftECB.Playback(EntityManager);
            leftECB.Dispose();
            rightECB.Playback(EntityManager);
            rightECB.Dispose();
        }
    }
}