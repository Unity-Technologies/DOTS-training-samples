using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class ResourceSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {        
        if (!HasSingleton<GameRuntimeData>())
            return;

        if(Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Entity runtimeDataEntity = GetSingletonEntity<GameRuntimeData>();
            GameRuntimeData runtimeData = EntityManager.GetComponentData<GameRuntimeData>(runtimeDataEntity);
            Plane plane = new Plane(math.up(), runtimeData.GridCharacteristics.Center);

            if (plane.Raycast(mouseRay, out float hitDistance)) 
            {           
                DynamicBuffer<ResourceSpawnEvent> spawnEvents = EntityManager.GetBuffer<ResourceSpawnEvent>(runtimeDataEntity);
                spawnEvents.Add(new ResourceSpawnEvent
                {
                    Position = (float3)Camera.main.transform.position + ((float3)mouseRay.direction * hitDistance) + (math.up() * 1f),
                });
            }
        }
    }
}
