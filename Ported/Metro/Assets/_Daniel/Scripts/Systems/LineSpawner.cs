using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

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
        var random = Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var lines = state.EntityManager.Instantiate(config.PlatformPrefab, config.LineCount, Allocator.Temp);
        int lineCounter = 0;
        float offset = 20;
        foreach (var line in lines)
        {
            var position = new float3(lineCounter, lineCounter, lineCounter + offset);
            //position.xz = new floa;
            URPMaterialPropertyBaseColor color = RandomColor();
            //state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(line, color);

            SetStairColor(state, line, color);

            state.EntityManager.SetComponentData<Line>(line, new Line { LineColor = color.Value } );
            state.EntityManager.SetComponentData<WorldTransform>(line, new WorldTransform { Position = position, Scale = 1 });
            lineCounter++;
        }



        state.Enabled = false;
    }

    void SetStairColor(SystemState state, Entity line, URPMaterialPropertyBaseColor color) 
    {
        //DynamicBuffer<Child> db = SystemAPI.GetBuffer<Child>(line);
        //foreach (var stairSet in null)
        //{
        //    //state.
        //    var stairs = SystemAPI.Query<Stair>();

        //}
    }
}
