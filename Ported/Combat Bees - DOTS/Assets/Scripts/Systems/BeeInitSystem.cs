using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class BeeInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }
    
    protected override void OnUpdate()
    {
        uint counter = 1; // needed to make a random seed
       // Because we can't get a random number at the authoring-phase, we do it in a later moment in this system.
        Entities.WithAll<BeeTag>().ForEach((Entity entity, ref RandomState randomState, ref Agression agression) =>
        {
            // make a new random value with a new seed. This should be done inside Entities.foreach, because we
            // want a different value for each entity that is the same
            randomState.Value = new Unity.Mathematics.Random(counter * randomState.Value.NextUInt(1,10000000));
            counter++;

            if (agression.Value < 0f) // for initialization
            {
                agression.Value = randomState.Value.NextFloat();
            }
        }).Schedule();
    }
}