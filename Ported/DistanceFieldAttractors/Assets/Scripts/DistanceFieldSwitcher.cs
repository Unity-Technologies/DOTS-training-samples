using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class DistanceFieldSwitcher : SystemBase
{
    static int s_modelCount = System.Enum.GetValues(typeof(DistanceFieldModel)).Length;
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var modelCount = s_modelCount;

        Entities
            .WithName("DistanceFieldSwitcher")
            .ForEach((ref DistanceField distanceField) =>
        {
            distanceField.SwitchCooldown += deltaTime * 0.1f;

            if(distanceField.SwitchCooldown > 1f)
            {
                distanceField.SwitchCooldown -= 1f;
                int newModel = distanceField.rng.NextInt(0, modelCount - 1);

                if (newModel >= (int)distanceField.Value)
                {
                    newModel++;
                }
                distanceField.Value = (DistanceFieldModel)newModel;
            }
        }).Run();
    }
}
