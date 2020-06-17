using Unity.Entities;
using Unity.Mathematics;

namespace HighwayRacer
{
    public class SetColor : SystemBase
    {
        
        public readonly static float3 cruiseColor = new float3(0.5f, 0.5f, 0.5f);
        public readonly static float3 fastestColor = new float3(0, 1.0f, 0);
        public readonly static float3 slowestColor = new float3(1.0f, 0.0f, 0);
        
        protected override void OnUpdate()
        {
            const float minSpeed = 10.0f;

            Entities.ForEach((ref Color color, in Speed speed, in UnblockedSpeed unblockedSpeed, in OvertakeSpeed overtakeSpeed) =>
            {
                if (speed.Val >= unblockedSpeed.Val)
                {
                    var percentage = (speed.Val - unblockedSpeed.Val) / (overtakeSpeed.Val - unblockedSpeed.Val);
                    color.Val = new float4(math.lerp(cruiseColor, fastestColor, percentage), 1.0f);
                }
                else
                {
                    var percentage = (unblockedSpeed.Val - speed.Val) / (unblockedSpeed.Val - minSpeed);
                    color.Val = new float4(math.lerp(cruiseColor, slowestColor, percentage), 1.0f);
                }
            }).Run();

            // for debug, make left blue, right yellow
            Entities.ForEach((ref Color color, in MergingLeft mergingLeft) =>
            {
                color.Val = new float4(0, 0, 1, 1);
            }).Run();
            
            Entities.ForEach((ref Color color, in MergingRight mergingRight) =>
            {
                color.Val = new float4(0, 1, 1, 1);
            }).Run();
        }
    }
}