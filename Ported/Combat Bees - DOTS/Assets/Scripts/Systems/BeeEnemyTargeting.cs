using System;
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
            .ForEach((Entity entity,ref BeeTargets beeTargets, ref RandomState randomState, ref BeeStatus beeStatus, in Team team, in BeeDead beeDead) => {
            if (beeStatus.Value == Status.Attacking)
            {
                if (beeTargets.EnemyTarget == Entity.Null && !beeDead.Value){
                        if (team.Value == TeamName.A && beeBEntities.Length > 0){
                            int randomIndex = randomState.Value.NextInt(0,beeBEntities.Length);
                            beeTargets.EnemyTarget = beeBEntities[randomIndex];
                        }
                        if(team.Value != TeamName.A && beeAEntities.Length > 0) {
                            int randomIndex = (randomState.Value.NextInt(0,beeAEntities.Length));
                            beeTargets.EnemyTarget = beeAEntities[randomIndex];
                        }
                    }
                    
                // try,catch: another bee killed this bee, so reset this bee's target.
                try {
                    beeTargets.CurrentTargetPosition = allTranslations[beeTargets.EnemyTarget].Value;
                }
                catch (Exception e) {
                    Debug.Log($"Bee dead value was NULL, resetting: {e}");
                    beeTargets.EnemyTarget = Entity.Null;
                    beeStatus.Value = Status.Idle;
                }
                
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