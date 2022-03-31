using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class TargetSystem : SystemBase
    {
        const float aggression = 0.5f; // Move to be a configurable parameter

        EntityQuery teamTargetsQuery0;
        EntityQuery teamTargetsQuery1;
        EntityQuery resourceTargets;

        protected override void OnCreate()
        {
            teamTargetsQuery0 = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<TeamShared>());
            teamTargetsQuery1 = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<TeamShared>());
            teamTargetsQuery0.SetSharedComponentFilter(new TeamShared { TeamId = 0 });
            teamTargetsQuery1.SetSharedComponentFilter(new TeamShared { TeamId = 1 });

            resourceTargets = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Components.Resource>(), ComponentType.ReadOnly<KinematicBodyState>());
            resourceTargets.SetSharedComponentFilter(new KinematicBodyState(){isEnabled = 0});
        }

        protected override void OnUpdate()
        {
            var teamTargets0 = teamTargetsQuery0.ToEntityArray(Allocator.TempJob);
            var teamTargets1 = teamTargetsQuery1.ToEntityArray(Allocator.TempJob);


            var resources = resourceTargets.ToEntityArray(Allocator.TempJob);
            
            var globalSystemVersion = GlobalSystemVersion;

            // Only run these two on chunks with no TargetType set.
            // This job runs witohut
            Entities
                .WithReadOnly(teamTargets0)
                .WithDisposeOnCompletion(teamTargets0)
                .WithReadOnly(teamTargets1)
                .WithDisposeOnCompletion(teamTargets1)
                .WithReadOnly(resources)
                .WithDisposeOnCompletion(resources)
                .ForEach((Entity entity, ref TargetType target, ref TargetEntity targetEntity, in Team team) =>
                {
                    if (team.TeamId == 0)
                        UpdateTargetEntityAndType(globalSystemVersion, teamTargets1, entity, ref target, ref targetEntity, ref resources);
                    else
                        UpdateTargetEntityAndType(globalSystemVersion, teamTargets0, entity, ref target, ref targetEntity, ref resources);
                }).ScheduleParallel();

         
            // Resolve the changing target position
            Entities
              .ForEach((Entity entity, ref CachedTargetPosition cache, in TargetType target, in TargetEntity targetEntity) =>
              {
                  if (target.Value == TargetType.Type.Enemy || target.Value == TargetType.Type.Resource)
                  {
                      if (HasComponent<Translation>(targetEntity.Value))
                      {
                          float3 pos = GetComponent<Translation>(targetEntity.Value).Value;
                          cache.Value = pos;
                      }
                      else
                      {
                          cache.Value = default;
                      }
                  }
              }).ScheduleParallel();


        }

        private static void UpdateTargetEntityAndType(uint globalSystemVersion, in NativeArray<Entity> attackables,/* in NativeArray<Translation> positions,*/ Entity entity, ref TargetType target, ref TargetEntity targetEntity, ref NativeArray<Entity> resources)
        {
            if (attackables.Length == 0)
            {
                return;
            }
            var random = Random.CreateFromIndex(globalSystemVersion ^ (uint)entity.Index);

            // Relying on this check stops filtering being effective, but otherwise there'd be a lot of structural churn
            if (target.Value == TargetType.Type.None)
            {
                if (random.NextFloat() < aggression)
                {
                    int attackableIndex = random.NextInt(attackables.Length);
                    target = new TargetType
                    {
                        Value = TargetType.Type.Enemy
                    };

                    targetEntity = new TargetEntity
                    {
                        Value = attackables[attackableIndex]
                    };
                }
                else
                {
                    if(resources.Length <= 0)
                        return;
                    
                    int resourceIndex = random.NextInt(resources.Length - 1);
                    targetEntity = new TargetEntity()
                    {
                        Value = resources[resourceIndex]
                    };
                    target = new TargetType()
                    {
                        Value = TargetType.Type.Resource
                    };
                }
            }
        }
    }
}