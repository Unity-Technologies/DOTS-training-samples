using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class PrepareCarDrawingSystem : JobComponentSystem
{
    EntityQuery m_PositionQuery;

    // This will be used by Drawing system
    public NativeArray<LocalToWorld> Positions;
    public NativeArray<CarColor> Colors;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_PositionQuery = GetEntityQuery(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadOnly<CarColor>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Positions = m_PositionQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        Colors = m_PositionQuery.ToComponentDataArray<CarColor>(Allocator.TempJob);

        return inputDeps;
    }
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(PrepareCarDrawingSystem))]
public class CarDrawingSystem : ComponentSystem
{
    Matrix4x4[] matrices = new Matrix4x4[1023];
    Vector4[] colorProperties = new Vector4[1023];

    protected override void OnUpdate()
    {
        var deltaTime = UnityEngine.Time.deltaTime;
        if (deltaTime <= float.Epsilon) // possible when the game is paused
            return;

        var ps = World.GetOrCreateSystem<PrepareCarDrawingSystem>();
        var properties = new MaterialPropertyBlock();
        var mat = CarMeshContent.instance.carMaterial;
        var mesh = CarMeshContent.instance.carMesh;

        Assert.IsTrue(ps.Colors.Length == ps.Positions.Length);

        var numCars = ps.Positions.Length;
        var numBatches = numCars / 1023;
        for (int batch = 0; batch < numBatches; batch++)
        {
            NativeArray<Matrix4x4>.Copy(ps.Positions.Reinterpret<Matrix4x4>().GetSubArray(batch * 1023, 1023),
                matrices, 1023);
            NativeArray<Vector4>.Copy(ps.Colors.Reinterpret<Vector4>().GetSubArray(batch * 1023, 1023),
                colorProperties, 1023);
            properties.SetVectorArray("_Color", colorProperties);
            Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, 1023, properties);
        }

        var rest = numCars % 1023;
        if (rest > 0)
        {
            NativeArray<Matrix4x4>.Copy(ps.Positions.Reinterpret<Matrix4x4>().GetSubArray(numBatches * 1023, rest),
                matrices, rest);
            NativeArray<Vector4>.Copy(ps.Colors.Reinterpret<Vector4>().GetSubArray(numBatches * 1023, rest),
                colorProperties, rest);
            properties.SetVectorArray("_Color", colorProperties);
            Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, rest, properties);
        }

        ps.Positions.Dispose();
        ps.Colors.Dispose();
    }
}
