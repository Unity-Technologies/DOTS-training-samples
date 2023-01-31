using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(ParentSystem))]
[BurstCompile]
partial struct LineSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;

    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var platforms = state.EntityManager.Instantiate(config.PlatformPrefab, config.LineCount, Allocator.Temp);
        float lineCounter = 0f;
        float offset = 20f;
        foreach (var platform in platforms)
        {
            var position = new float3();
            position.xz = random.NextFloat2() * 2;
            URPMaterialPropertyBaseColor color = RandomColor();
            //state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(line, color);

            SetStairColor(state, platform, color);

            //state.EntityManager.SetComponentData<Line>(line, new Line { LineColor = color.Value });
            state.EntityManager.SetComponentData<WorldTransform>(platform, new WorldTransform { Position = position, Scale = 1 });
            lineCounter++;
        }



        state.Enabled = false;
    }

    void SetStairColor(SystemState state, Entity platform, URPMaterialPropertyBaseColor color) 
    {
        //foreach (var item in SystemAPI.Query<Child>().WithAll<Parent>().WithAll<Stair>)
        foreach (var item in SystemAPI.Query<Stair>().WithAll<Child>().WithAll<URPMaterialPropertyBaseColor>())
        {
            Debug.Log(item.ToString());
        }
        //DynamicBuffer<Child> stairSet = SystemAPI.GetBuffer<Child>(platform);
        //for (int i = 1; i < stairSet.Length - 1; i++)
        //{
        //    DynamicBuffer<Child> steps = SystemAPI.GetBuffer<Child>(stairSet[i].Value);
        //    foreach (var step in steps)
        //    {
        //        state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(step.Value, color);
        //    }
        //}

        //foreach (Child set in stairSet)
        //{
            
        //    var baseColor = state.EntityManager.GetComponentData<URPMaterialPropertyBaseColor>(set.Value);
        //    //if(baseColor)
        //    {
        //        baseColor = color;
        //        state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(set.Value, baseColor);
        //    }

        //}
    }
}
