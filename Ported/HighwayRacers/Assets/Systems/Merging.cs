using Unity.Collections;
using Unity.Entities;

namespace HighwayRacer
{
    public class Merging : SystemBase
    {
        public const float mergeTime = 1.2f;  // number of seconds it takes to fully change lane 

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            var speed = mergeTime * Time.DeltaTime;
            
            Entities.WithAll<MergingLeft>().ForEach((Entity ent, ref LaneOffset laneOffset) =>
            {
                laneOffset.Val += speed;
                if (laneOffset.Val > 0)
                {
                    laneOffset.Val = 0;
                    ecb.RemoveComponent<MergingLeft>(ent);   // if only there were a method to remove multiple components in one call!
                    ecb.RemoveComponent<LaneOffset>(ent);
                }
            }).Run();

            Entities.WithAll<MergingRight>().ForEach((Entity ent, ref LaneOffset laneOffset) =>
            {
                laneOffset.Val -= speed;
                if (laneOffset.Val < 0)
                {
                    laneOffset.Val = 0;
                    ecb.RemoveComponent<MergingRight>(ent);   
                    ecb.RemoveComponent<LaneOffset>(ent);
                }
            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();

        }
    }
}