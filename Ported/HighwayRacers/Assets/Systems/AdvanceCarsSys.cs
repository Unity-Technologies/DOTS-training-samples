using Unity.Entities;

namespace HighwayRacer
{
    public class AdvanceCarsSys : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            float trackLength = 400.0f;  // todo: get from central source of truth
            
            Entities.ForEach((ref TrackPos trackPos, in Speed speed) =>
            {
                trackPos.Val += speed.Val;
                if (trackPos.Val > trackLength)
                {
                    trackPos.Val -= trackLength;
                }
            }).Run();
        }
    }
}