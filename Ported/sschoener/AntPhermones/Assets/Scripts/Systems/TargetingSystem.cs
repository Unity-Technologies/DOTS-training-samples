using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TargetingSystem : JobComponentSystem
{
    EntityQuery m_MapQuery;
    EntityQuery m_AntSteeringQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
        m_AntSteeringQuery = GetEntityQuery(ComponentType.ReadOnly<AntSteeringSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return default;
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        var antSteering = m_AntSteeringQuery.GetSingleton<AntSteeringSettingsComponent>();
        return new Job {
            ColonyPosition = map.ColonyPosition,
            ResourcePosition = map.ResourcePosition,
            TargetSteerStrength =  antSteering.TargetSteerStrength,
            Obstacles = map.Obstacles
        }.Schedule(this, inputDeps);
    }
    
    [BurstCompile]
    struct Job : IJobForEach<PositionComponent, HasResourcesComponent, FacingAngleComponent>
    {
        public float2 ColonyPosition;
        public float2 ResourcePosition;
        public float TargetSteerStrength;
        
        public BlobAssetReference<ObstacleData> Obstacles;
        
        bool Linecast(float2 point1, float2 point2)
        {
            float2 d = point2 - point1;
            float dist = math.length(d);

            int stepCount = (int)math.ceil(dist * .5f);
            for (int i = 0; i < stepCount; i++)
            {
                float t = (float)i / stepCount;
                if (Obstacles.Value.HasObstacle(point1 + t * d))
                {
                    return true;
                }
            }

            return false;
        }

        public void Execute([ReadOnly] ref PositionComponent position, [ReadOnly] ref HasResourcesComponent hasResources, ref FacingAngleComponent facingAngle)
        {
            var targetPos = hasResources.Value ? ColonyPosition : ResourcePosition;
            var p = position.Value;
            if (!Linecast(p, targetPos))
            {
                float targetAngle = math.atan2(targetPos.y - p.y, targetPos.x - p.x);
                float deltaAngle = targetAngle - facingAngle.Value;
                if (deltaAngle > math.PI)
                {
                    facingAngle.Value += math.PI * 2f;
                }
                else if (deltaAngle < -math.PI)
                {
                    facingAngle.Value -= math.PI * 2f;
                }
                else if (math.abs(deltaAngle) < math.PI * .5f)
                {
                    facingAngle.Value += deltaAngle * TargetSteerStrength;
                }
            }
        }
    }
}
