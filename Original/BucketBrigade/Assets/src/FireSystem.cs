using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct HeatMapConfig : IComponentData
{
    public int Width;
    public int Height;
}

public struct HeatMapData : IBufferElementData
{
    public byte Value;
}

public class HeatMapCreateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .WithAny<HeatMapConfig>()
            .WithNone<HeatMapData>()
            .ForEach((Entity e, in HeatMapConfig cfg) =>
            {
                var buffer = ecb.AddBuffer<HeatMapData>(e);
                for (int y = 0; y < cfg.Height; y++)
                {
                    var iy = y * cfg.Width;
                    for (int x = 0; x < cfg.Width; x++)
                    {
                        buffer[iy + x] = new HeatMapData() { Value = 0 };
                    }
                }
            }).Run();
        ecb.Playback(EntityManager);
    }
}

public class FireSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAny<HeatMapConfig>()
            .WithNone<HeatMapData>()
            .ForEach((Entity e, in HeatMapConfig cfg, in HeatMapData data) =>
            {
            }).ScheduleParallel();
    }
}

[UpdateAfter(typeof(FireSystem))]
public class FireRenderSystem : SystemBase
{
    Mesh mesh;
    Material mat;
    MaterialPropertyBlock properties;
    ComputeBuffer buffer;
    Matrix4x4[] localToWorlds;
    protected override void OnUpdate()
    {
        //buffer.SetData(
        properties.SetBuffer("", buffer);
        Graphics.DrawMeshInstanced(mesh, 0, mat, localToWorlds, localToWorlds.Length, properties);
        Entities
            .WithAny<HeatMapConfig>()
            .WithNone<HeatMapData>()
            .ForEach((Entity e, in HeatMapConfig cfg, in HeatMapData data) =>
            {
                //Graphics.DrawMeshInstanced(mesh, 0, barMaterial, matrices[i], matrices[i].Length, matProps[i])

            }).Run();

    }
}
