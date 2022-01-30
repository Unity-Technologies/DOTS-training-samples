using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

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
            .ForEach((Entity entity,ref BeeTargets beeTargets, ref RandomState randomState, in BeeStatus beeStatus, in Team team, in BeeDead beeDead) => {
            if (beeStatus.Value == Status.Attacking)
            {
                if (team.Value == TeamName.A && beeTargets.EnemyTarget == Entity.Null&& beeBEntities.Length>0 && !beeDead.Value)
                {
                    //for different initial random (some times throws index out of range with spawning of new bees)
                    // int randomIndex =(int) (((randomState.Value.NextFloat(0,beeBEntities.Length)+team.IndexInTeam))/2);

                    //with this approach randomIndex for all the bees is same value in the begining
                    int randomIndex = randomState.Value.NextInt(0,beeBEntities.Length);
                    beeTargets.EnemyTarget = beeBEntities[randomIndex];
                    // Debug.Log(team.IndexInTeam);
                    // Debug.Log(randomIndex);
                }
                if (team.Value != TeamName.A && beeTargets.EnemyTarget == Entity.Null&& beeAEntities.Length>0&&!beeDead.Value)
                {
                    // int randomIndex =(int) (((randomState.Value.NextFloat(0,beeAEntities.Length)+team.IndexInTeam))/2);

                    int randomIndex = (randomState.Value.NextInt(0,beeAEntities.Length));
                    beeTargets.EnemyTarget = beeAEntities[randomIndex];
                    // Debug.Log(randomIndex);
                }
                
                beeTargets.CurrentTargetPosition = allTranslations[beeTargets.EnemyTarget].Value;
            }
        }).Run();
        // Entities.WithAll<BeeTag>().ForEach((Entity entity,ref BeeTargets beeTargets,ref BeeStatus beeStatus) =>
        // {
        //     if (beeTargets.EnemyTarget != Entity.Null)
        //     {
        //         var comp = GetComponent<BeeDead>(beeTargets.EnemyTarget);
        //         if (comp.Value)
        //         {
        //             beeTargets.EnemyTarget = Entity.Null;
        //             // beeStatus.Value = Status.Idle;
        //         }
        //     }
        //        
        //    
        //
        // }).Run();
    }
}