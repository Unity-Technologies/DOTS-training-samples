using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CameraSimulationSystem : ComponentSystem
{
    private EntityQuery cameraQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        cameraQuery = GetEntityQuery( new EntityQueryDesc {All=new ComponentType[]{typeof(GenerationSystem.State)}});
        RequireForUpdate(cameraQuery);
    }
    
    protected override void OnUpdate()
    {
        var simulationState = GetSingleton<GenerationSystem.State>();
        
        var cameraEntity = GetSingletonEntity<CameraTag>();
        var cameraPosition = EntityManager.GetComponentData<Translation>(cameraEntity);

        var cameraOrientation = (Quaternion)EntityManager.GetComponentData<Rotation>(cameraEntity).Value;
        var cameraForward = cameraOrientation * new Vector3(0f, 0f, 1f);
        var offsetVector = new Vector3(0f, 0f, 1f) + cameraForward * 60f;
        
        cameraPosition.Value = new float3(simulationState.tornadoX, 10f, simulationState.tornadoZ) - (float3)offsetVector;
        EntityManager.SetComponentData(cameraEntity, cameraPosition);
    }
}

