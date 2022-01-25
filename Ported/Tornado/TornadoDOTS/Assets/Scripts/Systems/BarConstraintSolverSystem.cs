using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(JointsSimulationSystem))]
public partial class BarConstraintSolverSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var getTranslation = GetComponentDataFromEntity<Translation>();
        var getAnchorPoint = GetComponentDataFromEntity<AnchorPoint>(true);
        Entities
            .WithNativeDisableContainerSafetyRestriction(getTranslation)
            .WithNativeDisableContainerSafetyRestriction(getAnchorPoint)
            .ForEach((in BarConnection bar) =>
            {
                var joint1Pos = getTranslation[bar.Joint1].Value;
                var joint2Pos = getTranslation[bar.Joint2].Value;
                var delta = joint2Pos - joint1Pos;
                var dist = math.length(delta);
                var extraDist = dist - bar.Length;

                var push = (delta / dist * extraDist) * 0.5f;
                var isJoint1Anchor = getAnchorPoint.HasComponent(bar.Joint1);
                var isJoint2Anchor = getAnchorPoint.HasComponent(bar.Joint2);
                if (!isJoint1Anchor && !isJoint2Anchor)
                {
	                joint1Pos += push;
	                joint2Pos -= push;
                }
                else if (isJoint1Anchor)
                {
	                joint2Pos -= (push * 2);
                }
                else if (isJoint2Anchor)
                {
	                joint1Pos += (push * 2);
                }

                getTranslation[bar.Joint1] = new Translation() {Value = joint1Pos};
                getTranslation[bar.Joint2] = new Translation() {Value = joint2Pos};
            }).ScheduleParallel();
    }
}