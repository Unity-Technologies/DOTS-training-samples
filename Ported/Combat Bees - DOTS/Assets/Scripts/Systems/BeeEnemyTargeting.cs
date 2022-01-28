using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial class BeeEnemyTargeting : SystemBase
{
    protected override void OnUpdate()
    {
        NativeList<Entity> beeAEntities = new NativeList<Entity>(Allocator.TempJob);
        NativeList<Entity> beeBEntities = new NativeList<Entity>(Allocator.TempJob);
        
        var allTranslations = GetComponentDataFromEntity<Translation>(true);

        Entities.WithAll<BeeTag>().ForEach((Entity entity, in Team team) =>
        {
            if(team.Value == TeamName.A)
                beeAEntities.Add(entity);
            else if (team.Value == TeamName.B)
                beeBEntities.Add(entity);
        }).Run();
        
        Entities.WithNativeDisableContainerSafetyRestriction(allTranslations).WithDisposeOnCompletion(beeAEntities)
            .WithDisposeOnCompletion(beeBEntities)
            .ForEach((Entity entity,ref BeeTargets beeTargets, ref RandomState randomState, in BeeStatus beeStatus, in Team team) => {
            if (beeStatus.Value == Status.Attacking)
            {
                if (team.Value == TeamName.A && beeTargets.EnemyTarget == Entity.Null)
                {
                    int randomIndex = randomState.Value.NextInt(beeBEntities.Length);
                    beeTargets.EnemyTarget = beeBEntities[randomIndex];
                }
                if (team.Value != TeamName.A && beeTargets.EnemyTarget == Entity.Null)
                {
                    int randomIndex = randomState.Value.NextInt(beeAEntities.Length);
                    beeTargets.EnemyTarget = beeAEntities[randomIndex];
                }
                
                beeTargets.CurrentTargetPosition = allTranslations[beeTargets.EnemyTarget].Value;
            }
        }).Schedule();
    }
}