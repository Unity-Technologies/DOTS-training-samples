using HighwayRacers;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class DrawingSystem : ComponentSystem
{
    public Material mat;
    public Mesh mesh;

    EntityQuery m_Query;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(LocalToWorld), typeof(ColorComponent));
    }

    protected override void OnUpdate()
    {
        mat = Game.instance.entityMaterial;
        mesh = Game.instance.entityMesh;


        var positions = m_Query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var matrices = new Matrix4x4[positions.Length];
        var colors = m_Query.ToComponentDataArray<ColorComponent>(Allocator.TempJob);
        var properties = new MaterialPropertyBlock();
        var colorProperties = new Vector4[colors.Length];

        Assert.IsTrue(colors.Length == positions.Length);
        for (var i = 0; i != positions.Length; i++)
        {
            matrices[i] = positions[i].Value;
            colorProperties[i] = colors[i].Value;
        }
        properties.SetVectorArray("_Color",colorProperties);
        Graphics.DrawMeshInstanced(mesh,0,mat,matrices,matrices.Length,properties);

        positions.Dispose();
        colors.Dispose();
    }
}
