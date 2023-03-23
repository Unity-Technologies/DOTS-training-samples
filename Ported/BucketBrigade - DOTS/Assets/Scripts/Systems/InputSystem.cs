using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.Rendering;
using Unity.Scenes;
using UnityEngine.SceneManagement;

[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial class InputSystem : SystemBase
{
    public ComponentLookup<OnFire> m_OnFireActive;
    private Camera camera;
    Vector3 m_DistanceFromCamera;
    Plane m_Plane;
    Tile Tile;
    
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
        bool resetClick = Input.GetKeyDown(KeyCode.R);
        camera = Camera.main;
        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        var worldPos = ScreenToWorld(new float2(mouseRay.origin.x ,mouseRay.origin.z));

        foreach (var (tileTransform,entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithEntityAccess().WithAll<Tile>())
        {
           if (leftClick)
           {
               var dist=Vector2.Distance(new Vector2(tileTransform.ValueRW.Position.x, tileTransform.ValueRW.Position.z), new Vector2(worldPos.x, worldPos.y));
               if (dist < 0.5f*config.cellSize)
               {
                   /*Tile.Temperature = 1f;
                   var isOnFire = Tile.Temperature >= config.flashpoint;
                   SystemAPI.SetComponentEnabled<OnFire>(entity, isOnFire);*/
                   SystemAPI.SetComponent(entity, new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.colour_fireCell_hot });
                   var fireMove = tileTransform.ValueRW.Position + new float3(0,   random.Value.NextFloat(1,3)  , 0);
                   tileTransform.ValueRW.Position = fireMove;
                   Debug.Log("index:"+ entity.Index);
               }
                
           }
           
        }
        if (resetClick)
        {
            SceneManager.LoadScene("Scenes/MainScene" + "Scenes/MainScene/Elfi Sub Scene");
        }
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

//4.9f, 5.1f, 0, 10, --> for camera position at 5 15 5