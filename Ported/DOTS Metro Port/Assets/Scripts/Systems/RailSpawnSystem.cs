using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct RailSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var rails = CollectionHelper.CreateNativeArray<Entity>(5, allocator);
        ecb.Instantiate(config.RailPrefab, rails);

        int i = 0 ;
        foreach (var rail in rails)
        {
            ecb.SetComponent(rail, new Translation { Value = new float3(i*10.0f, 0.0f, 0.0f) });
            i++;
        }

        foreach (var buffer in SystemAPI.Query<DynamicBuffer<BezierPoint>>())
        {
            foreach (var element in buffer)
            {
                var rail = ecb.Instantiate(config.RailPrefab);
                ecb.SetComponent(rail, new Translation { Value = element.location });
            }
            /*
            var array = buffer.AsNativeArray();

            BezierPath.MeasurePath(array);

            // Now, let's lay the rail meshes
            float _DIST = 0f;
            //Metro _M = Metro.INSTANCE;
            while (_DIST < BezierPath.GetPathDistance())
            {
                float _DIST_AS_RAIL_FACTOR = BezierPath.Get_distanceAsRailProportion(array, _DIST);
                float3 _RAIL_POS = BezierPath.Get_Position(array, _DIST_AS_RAIL_FACTOR);
                //Vector3 _RAIL_ROT = Get_RotationOnRail(_DIST_AS_RAIL_FACTOR);
                //            _RAIL.GetComponent<Renderer>().material.color = lineColour;

                //_RAIL.transform.LookAt(_RAIL_POS - _RAIL_ROT);

                var rail = ecb.Instantiate(config.RailPrefab);
                ecb.SetComponent(rail, new Translation { Value = _RAIL_POS });

                _DIST += 0.1f;// Metro.RAIL_SPACING;
            }*/

        }




        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}