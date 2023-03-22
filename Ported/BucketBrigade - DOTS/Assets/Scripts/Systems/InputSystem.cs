using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.Rendering;

[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial class InputSystem : SystemBase
{
    public ComponentLookup<OnFire> m_OnFireActive;
    private Camera camera;
    
    protected override void OnCreate()
    {
        RequireForUpdate<Config>();
      m_OnFireActive = GetComponentLookup<OnFire>();
    }

    protected override void OnUpdate()
    {
      m_OnFireActive.Update(this);
        var config = SystemAPI.GetSingleton<Config>();
        var random = SystemAPI.GetSingleton<Random>();
        bool leftClick = Input.GetKeyDown(KeyCode.Mouse0);
        camera = Camera.main;
        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        //var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer((new WorldUnmanaged()));
        
        Entity tile;
        var worldPos = ScreenToWorld(new float2(mouseRay.origin.x,mouseRay.origin.z));
        //Debug.Log(worldPos);

        foreach (var (tileTransform,entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithEntityAccess().WithAll<Tile>())
        {
           if (leftClick)
            {
                var dist=Vector2.Distance(new Vector2(tileTransform.ValueRW.Position.x, tileTransform.ValueRW.Position.z), new Vector2(worldPos.x, worldPos.y));
                //Debug.Log(tileTransform.ValueRW.Position);
                if (dist < 0.5f)
                {
                    SystemAPI.SetComponent(entity, new URPMaterialPropertyBaseColor { Value = new float4(random.Value.NextFloat(0.5f,1f),0f,0f,1f) });
                    var fireMove = tileTransform.ValueRW.Position + new float3(0,   random.Value.NextFloat(1,3)  , 0);
                    tileTransform.ValueRW.Position = fireMove;
                    Debug.Log(entity.Index);
                }
                
            }
        }
        // if position of mouse = within grid, fireTile & fireTile is not on fire then enable onFire and give random temperature
    }
    public float2 ScreenToWorld(float2 screenPos)
    {
        //check if its withing bounds 
        var remapX=math.remap(4.9f, 5.1f, 0, 10,screenPos.x);
        var remapY=math.remap(4.9f, 5.1f, 0, 10,screenPos.y);
        var worldPos = new float2(remapX, remapY);
        return  worldPos;
    }
}

//4.9f, 5.1f, 0, 10,screenPos.x) for camera position at 5 15 5