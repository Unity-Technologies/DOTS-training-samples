using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class BeeInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BeeTag>().ForEach((Entity entity, ref RandomState randomState, ref Agression agression) =>
        {
            if (agression.Value == -1f)
            {
                //Debug.Log("Index: " + entity.Index);
                //Debug.Log("State: " + randomState.Random.state);
                agression.Value = randomState.Random.NextFloat();
            }
        }).Schedule();
    }
}