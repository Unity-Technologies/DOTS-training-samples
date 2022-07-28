using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
    
        var sceneCamera = GameObject.Find("Main Camera");
        Ray mouseRay = sceneCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        var isMouseTouchingField = false;
        var field = config.PlayVolume * 2;
        Vector3 hitPoint = new Vector3(0, 0, 0);
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var resourceColor = new URPMaterialPropertyBaseColor
            { Value = (Vector4) new Color(0.1572f, 0.4191f, 0.0739f, 1.0f) };
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var newResource = ecb.Instantiate(config.ResourcePrefab);
        for (int i=0;i<3;i++) {
            for (int j=-1;j<=1;j+=2) {
                Vector3 wallCenter = new Vector3();
                wallCenter[i] = field[i] * .5f*j;
                Plane plane = new Plane(-wallCenter,wallCenter);
                float hitDistance;
                if (Vector3.Dot(plane.normal,mouseRay.direction) < 0f) {
                    if (plane.Raycast(mouseRay,out hitDistance)) {
                        hitPoint = mouseRay.GetPoint(hitDistance);
                        bool insideField = true;
                        for (int k = 0; k < 3; k++) {
                            if (Mathf.Abs(hitPoint[k]) > field[k] * .5f+.01f) {
                                insideField = false;
                                break;
                            }
                        }
                        if (insideField) {
                            isMouseTouchingField = true;
                            break;
                        }
                    }
                }
            }
            if (isMouseTouchingField) {
                break;
            }
        }
        if (isMouseTouchingField)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ecb.SetComponentEnabled<Gravity>(newResource, true);
                ecb.SetComponent(newResource, new Resource());
                Vector3 newResourceSpawnLocation = hitPoint;
                ecb.SetComponent(newResource, new Translation { Value = newResourceSpawnLocation });
                ecb.SetComponent(newResource, resourceColor);
            }
        }
    }
}