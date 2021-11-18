using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Dots
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial class BeamRenderUpdateSystem : SystemBase
    {
        BuildingSpawnerSystem m_BuildingSpawnerSystem;

        protected override void OnCreate()
        {
            m_BuildingSpawnerSystem = World.GetExistingSystem<BuildingSpawnerSystem>();
        }

        protected override void OnUpdate()
        {
            var localBeams = m_BuildingSpawnerSystem.beams;
            Entities
                .WithNativeDisableContainerSafetyRestriction(localBeams)
                .ForEach((ref Translation t, ref Rotation r, ref Beam beam) =>
            {
                var beamData = localBeams[beam.beamDataIndex];
                var point1 = beamData.p1;
                var point2 = beamData.p2;

                var a1 = GetComponent<Anchor>(point1);
                var a2 = GetComponent<Anchor>(point2);

                var newD = beamData.newD;

                float dist = math.sqrt(newD.x * newD.x + newD.y * newD.y + newD.z * newD.z);

                t.Value.x = (a1.position.x + a2.position.x) * .5f;
                t.Value.y = (a1.position.y + a2.position.y) * .5f;
                t.Value.z = (a1.position.z + a2.position.z) * .5f;

                if (newD.x / dist * beamData.oldD.x + newD.y / dist * beamData.oldD.y + newD.z / dist * beamData.oldD.z < .99f)
                {
                    r.Value = Quaternion.LookRotation(newD);
                    beamData.oldD.x = newD.x / dist;
                    beamData.oldD.y = newD.y / dist;
                    beamData.oldD.z = newD.z / dist;
                }

                localBeams[beam.beamDataIndex] = beamData;
            }).ScheduleParallel();
        }
    }
}

