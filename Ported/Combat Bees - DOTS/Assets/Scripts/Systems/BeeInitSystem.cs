using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class BeeInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        uint counter = 1;
       // Because we can't get a random number at the authoring-phase, we do it in a later moment in this system.
        Entities.WithAll<BeeTag>().ForEach((Entity entity, ref RandomState randomState, ref Agression agression) =>
        {
            
            randomState.Value = new Unity.Mathematics.Random(counter * randomState.Value.NextUInt(0,10000000));
            counter++;

            if (agression.Value < 0f)
            {
                agression.Value = randomState.Value.NextFloat();
            }
        }).Schedule();
    }
}