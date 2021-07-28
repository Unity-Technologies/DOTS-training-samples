using Unity.Entities;

public class AntExcitementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var generalSettingsEntity = GetSingletonEntity<GeneralSettings>();
        var antSpeed = GetComponent<GeneralSettings>(generalSettingsEntity).AntSpeed;
        var normalExcitement = GetComponent<GeneralSettings>(generalSettingsEntity).NormalExcitement;
        var holdingResourceExcitement = GetComponent<GeneralSettings>(generalSettingsEntity).HoldingResourceExcitement;
        Entities
            .WithAll<Ant>()
            .WithAll<HoldingResource>()
            .ForEach((ref Excitement excitement, in Speed speed) => 
            {
                excitement.Value = holdingResourceExcitement * speed.Value / antSpeed;

            }).ScheduleParallel();

        Entities
            .WithAll<Ant>()
            .WithNone<HoldingResource>()
            .ForEach((ref Excitement excitement, in Speed speed) =>
            {
                excitement.Value = normalExcitement * speed.Value / antSpeed;

            }).ScheduleParallel();
    }
}
