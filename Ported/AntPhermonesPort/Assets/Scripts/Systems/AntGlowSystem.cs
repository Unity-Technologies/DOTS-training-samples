using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Rendering;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class AntGlowSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref AntMovement ant) =>
            {
                var mesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                switch (ant.State)
                {
                    case AntState.LineOfSightToFood:
                        mesh.material.color = Color.white;
                        mesh.material.SetColor("_EmissionColor", Color.white * 10);
                        break;
                    case AntState.ReturnToNestWithLineOfSight:
                    case AntState.ReturnToNest:
                        mesh.material.color = Color.yellow;
                        mesh.material.SetColor("_EmissionColor", Color.yellow * 10);
                        break;
                    case AntState.Searching:
                    default:
                        mesh.material.color = Color.red;
                        mesh.material.SetColor("_EmissionColor", Color.red);
                        break;
                }
            }).Run();
    }
}