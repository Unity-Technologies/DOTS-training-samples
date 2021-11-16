using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DecaySystem : SystemBase
{
    protected override void OnUpdate()
    {
        //var deltaTime = Time.DeltaTime;
        //Entities
        //    .WithStructuralChanges()
        //    .ForEach((Entity entity, ref Decay decay, ref Scale scale) =>
        //{
        //    scale.Value -= decay.Rate * deltaTime;

        //    if (scale.Value < 0)
        //    {
        //        EntityManager.DestroyEntity(entity);
        //    }
        //}).Schedule();
    }
}
