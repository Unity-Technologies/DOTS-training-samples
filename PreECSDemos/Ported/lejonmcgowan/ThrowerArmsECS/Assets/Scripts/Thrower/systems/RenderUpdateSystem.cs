using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class RenderUpdateSystem : SystemBase
{
    //todo look into combine with the FingerRenderUpdate?
    protected override void OnUpdate()
    {
        // TODO: SystemBase has a way to eliminate the need for this GetBufferFromEntity
        var JointsFromEntity = GetBufferFromEntity<JointElementData>(true);
        var UpBases = GetComponentDataFromEntity<ArmBasesUp>(true);
        var FingerParents = GetComponentDataFromEntity<FingerParent>(true);

        Entities.WithReadOnly(JointsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(FingerParents)
            .ForEach((ref Translation translation, ref Rotation rotation, ref NonUniformScale scale,
                in RenderComponentData renderData, in Thickness thickness) =>
            {
                var simulatedRef = renderData.entityRef;
                var joints = JointsFromEntity[simulatedRef];
                var jointPos = joints[renderData.jointIndex];
                var delta = joints[renderData.jointIndex + 1].value - jointPos.value; // from joint end to joint begin

                
                ArmBasesUp upBasis;

                if (HasComponent<FingerParent>(simulatedRef))
                {
                    var armRef = FingerParents[simulatedRef];
                    upBasis = UpBases[armRef].value;
                }
                else
                {
                    upBasis = UpBases[simulatedRef].value;
                }

                translation = new Translation()
                {
                    Value = jointPos + 0.5f * delta
                };
                rotation = new Rotation()
                {
                    Value = quaternion.LookRotation(math.normalize(delta), upBasis)
                };

                scale = new NonUniformScale()
                {
                    Value = new float3(thickness, thickness, math.length(delta))
                };
            }).Run();
    }
}