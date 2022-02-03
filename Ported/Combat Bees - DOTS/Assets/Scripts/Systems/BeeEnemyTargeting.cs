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
        int teamAIndex = 0;
        int teamBIndex = 0;
        
        Entities.WithAll<BeeTag>().ForEach((Entity entity,ref Team team) =>
        {
            if (team.Value == TeamName.A)
            {
                beeAEntities.Add(entity);
                team.IndexInTeam = teamAIndex;
                teamAIndex++;
            }
            else if (team.Value == TeamName.B)
            {
                beeBEntities.Add(entity);
                team.IndexInTeam = teamBIndex;
                teamBIndex++;
            }
        }).Run();
      
        Entities.WithNativeDisableContainerSafetyRestriction(allTranslations).WithDisposeOnCompletion(beeAEntities)
            .WithDisposeOnCompletion(beeBEntities)
            .ForEach((Entity entity,ref BeeTargets beeTargets, ref RandomState randomState, ref BeeStatus beeStatus, in Team team, in BeeDead beeDead) => {
            if (beeStatus.Value == Status.Attacking)
            {
                if (beeTargets.EnemyTarget == Entity.Null && !beeDead.Value){
                        if (team.Value == TeamName.A && beeBEntities.Length > 0){
                            int randomIndex = randomState.Value.NextInt(0,beeBEntities.Length);
                            beeTargets.EnemyTarget = beeBEntities[randomIndex];
                        }
                        if(team.Value == TeamName.B && beeAEntities.Length > 0) {
                            int randomIndex = (randomState.Value.NextInt(0,beeAEntities.Length));
                            beeTargets.EnemyTarget = beeAEntities[randomIndex];
                        }
                    }
                    
                beeTargets.CurrentTargetPosition = allTranslations[beeTargets.EnemyTarget].Value;
            }
        }).Run();
    }
}