using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Scenes;

[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial class InputSystem : SystemBase
{
    public ComponentLookup<OnFire> m_OnFireActive;
    private Camera camera;
    Vector3 m_DistanceFromCamera;
    Plane m_Plane;
    Tile Tile;
    private EntityQuery newRequests;
    
    protected override void OnCreate()
    { 
        newRequests = GetEntityQuery(typeof(SceneLoader));
        RequireForUpdate<SceneLoader>();
        RequireForUpdate<Config>();
        m_OnFireActive = GetComponentLookup<OnFire>();
    }

    protected override void OnUpdate()
    {
        var requests = newRequests.ToComponentDataArray<SceneLoader>(Allocator.Persistent);
        m_OnFireActive.Update(this);
        var config = SystemAPI.GetSingleton<Config>();
        var random = SystemAPI.GetSingleton<Random>();
        var sceneLoader = SystemAPI.GetSingleton<SceneLoader>();
        bool leftClick = Input.GetKeyDown(KeyCode.Mouse0);
        bool resetClick = Input.GetKeyDown(KeyCode.R);
        camera = Camera.main;
        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        var worldPos = ScreenToWorld(new float2(mouseRay.origin.x ,mouseRay.origin.z));
        var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(this.EntityManager.WorldUnmanaged);


        foreach (var (tileTransform,entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithEntityAccess().WithAll<Tile>())
        {
           if (leftClick)
           {
               var dist=Vector2.Distance(new Vector2(tileTransform.ValueRW.Position.x, tileTransform.ValueRW.Position.z), new Vector2(worldPos.x, worldPos.y));
               if (dist < 0.5f*config.cellSize)
               {
                   ecb.SetComponent(entity, new Tile { Temperature = config.flashpoint});
                   ecb.SetComponentEnabled<OnFire>(entity,true);
               }
                
           }
        }
        
        /*if (resetClick)
        {
            for (int i = 0; i < requests.Length; i += 1)
            {
                SceneSystem.LoadSceneAsync(World.Unmanaged, requests[i].SceneReference, new SceneSystem.LoadParameters());
            }
            requests.Dispose();
            EntityManager.DestroyEntity(newRequests);
        }*/
    }
    public float2 ScreenToWorld(float2 screenPos)
    {
        //check if its within bounds 
        var remapX=math.remap(4.9f, 5.1f, 0, 10,screenPos.x);
        var remapY=math.remap(4.9f, 5.1f, 0, 10,screenPos.y);
        var worldPos = new float2(remapX, remapY);
        return  worldPos;
    }
}

//4.9f, 5.1f, 0, 10, --> for camera position at 5 15 5