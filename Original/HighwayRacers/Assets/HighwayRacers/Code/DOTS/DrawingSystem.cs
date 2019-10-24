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
        m_Query = GetEntityQuery(ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadOnly<ColorComponent>());
    }

    Matrix4x4[] matrices = new Matrix4x4[1023];
    Vector4[] colorProperties = new Vector4[1023];

    protected override void OnUpdate()
    {
        mat = Game.instance.entityMaterial;
        mesh = Game.instance.entityMesh;


        var positions = m_Query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var colors = m_Query.ToComponentDataArray<ColorComponent>(Allocator.TempJob);
        var properties = new MaterialPropertyBlock();

        Assert.IsTrue(colors.Length == positions.Length);

        var numCars = positions.Length;
        var numBatches = numCars / 1023;

        for (int batch = 0; batch < numBatches; batch++)
        {
            for (var i = 0; i < 1023; i++)
            {
                var index = batch * 1023 + i;
                matrices[i] = positions[index].Value;
                colorProperties[i] = colors[index].Value;
            }

            properties.SetVectorArray("_Color",colorProperties);
            Graphics.DrawMeshInstanced(mesh,0,mat,matrices,1023,properties);
        }

        var rest = numCars % 1023;
        if (rest > 0 )
        {
            for (var i = 0; i < rest; i++)
            {
                var index = numBatches * 1023 + i;
                matrices[i] = positions[index].Value;
                colorProperties[i] = colors[index].Value;
            }

            properties.SetVectorArray("_Color", colorProperties);
            Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, rest, properties);
        }

        positions.Dispose();
        colors.Dispose();
    }
}
