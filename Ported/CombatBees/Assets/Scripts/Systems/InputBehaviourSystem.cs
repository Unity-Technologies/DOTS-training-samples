using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct InputBehaviourSystem : ISystem
{
    private static Camera camera;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var halfFieldSize = config.fieldSize * .5f;
        if (!camera)
            camera = Camera.main;
        
        if (Input.GetMouseButtonDown(0))
        {
            var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
            for (var i = 0; i < 3; i++)
            {
                for (var j = -1; j <= 1; j += 2)
                {
                    var wallCenter = new Vector3();
                    wallCenter[i] = halfFieldSize[i] * j;
                    var plane = new Plane(-wallCenter, wallCenter);
                    float hitDistance;
                    if (Vector3.Dot(plane.normal, mouseRay.direction) < 0f)
                    {
                        if (plane.Raycast(mouseRay, out hitDistance))
                        {
                            Vector3 hitPoint = mouseRay.GetPoint(hitDistance);
                            bool insideField = true;
                            for (int k = 0; k < 3; k++)
                            {
                                if (Mathf.Abs(hitPoint[k]) > config.fieldSize[k] * .5f + .01f)
                                {
                                    insideField = false;
                                    break;
                                }
                            }

                            if (insideField)
                            {
                                SpawnResource(ecb, config, (float3)hitPoint);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void SpawnResource(EntityCommandBuffer ecb, Config config, float3 position)
    {
        var entity = ecb.Instantiate(config.resourcePrefab);
        ecb.SetComponent(entity, new LocalTransform
        {
            Position = position,
            Scale = 1f
        });
        ecb.SetComponentEnabled<ResourceCarried>(entity, false);
        ecb.SetComponentEnabled<ResourceDropped>(entity, true);
    }
}