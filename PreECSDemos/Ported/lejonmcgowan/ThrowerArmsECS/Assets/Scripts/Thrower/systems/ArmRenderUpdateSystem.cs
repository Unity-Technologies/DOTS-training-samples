using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class ArmRenderUpdateSystem: SystemBase
{
    
    //todo look into combine with the FingerRenderUpdate?
    protected override void OnUpdate()
    {
        // TODO: SystemBase has a way to eliminate the need for this GetBufferFromEntity
        var ArmJointsFromEntity = GetBufferFromEntity<ArmJointElementData>(true);
        var UpBases = GetComponentDataFromEntity<ArmBasesUp>(true);
        
        Entities.WithReadOnly(ArmJointsFromEntity)
            .WithReadOnly(UpBases)
            .ForEach((ref Translation translation,ref Rotation rotation,ref NonUniformScale scale,in ArmRenderComponentData armRef) =>
        {
            var joints = ArmJointsFromEntity[armRef.armEntity];
            var jointPos = joints[armRef.jointIndex];
            var delta = joints[armRef.jointIndex + 1].value - jointPos.value ; // from joint end to joint begin
            var upBasis = UpBases[armRef.armEntity].value;
            
            var debugDelta = jointPos + 0.5f * delta;
            
            translation = new Translation()
            {
                Value = debugDelta
            };
            rotation = new Rotation()
            {
                Value = quaternion.LookRotation(math.normalize(delta),upBasis)
            };
            
            scale = new NonUniformScale()
            {
                Value = new float3(0.15f,0.15f,math.length(delta))
            };

        }).ScheduleParallel();
    }
}
