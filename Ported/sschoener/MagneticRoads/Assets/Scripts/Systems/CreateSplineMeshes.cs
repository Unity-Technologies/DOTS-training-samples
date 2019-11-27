using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Systems {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class CreateSplineMeshes : JobComponentSystem
    {
        EntityQuery m_RoadSetupQuery;
        EntityArchetype m_SplineMeshArchetype;
        EntityQuery m_SplineQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_RoadSetupQuery = GetEntityQuery(typeof(RoadSetupComponent));
            m_RoadSetupQuery.SetChangedVersionFilter(typeof(RoadSetupComponent));
            m_SplineMeshArchetype = EntityManager.CreateArchetype(
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(Static)
            );
            m_SplineQuery = GetEntityQuery(typeof(RoadSplineComponent), typeof(RenderMesh));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            bool hasChanged = m_RoadSetupQuery.CalculateEntityCount() > 0;
            if (!hasChanged)
                return default;
            var roadsEntity = m_RoadSetupQuery.GetSingletonEntity();
            var roads = EntityManager.GetComponentData<RoadSetupComponent>(roadsEntity);
            var roadsMaterial = EntityManager.GetSharedComponentData<RenderMesh>(roadsEntity).material;

            TrackUtils.SizeOfMeshData(roads.SplineResolution, out int verticesPerSpline, out int indicesPerSpline);

            int numSplines = roads.Splines.Value.Splines.Length;
            var vertices = new NativeArray<float3>(verticesPerSpline * numSplines, Allocator.TempJob);
            var triangles = new NativeArray<int>(indicesPerSpline * numSplines, Allocator.TempJob);

            int splinesPerMesh = 3 * roads.TrisPerMesh / indicesPerSpline;
            var job = new GenerateTrackMeshes
            {
                VerticesPerSpline = verticesPerSpline,
                IndicesPerSpline = indicesPerSpline,
                TrackSplines = roads.Splines,
                OutVertices = vertices,
                OutTriangles = triangles,
                SplinesPerMesh = splinesPerMesh,
            };
            job.Setup(roads.SplineResolution, roads.TrackRadius, roads.TrackThickness);
            job.Schedule(numSplines, 16).Complete();

            int numMeshes = numSplines / splinesPerMesh;
            using (new ProfilerMarker("CreateMeshes").Auto())
            {
                int remaining = numSplines % splinesPerMesh;
                numMeshes += remaining != 0 ? 1 : 0;
                for (int i = 0; i < numMeshes; i++)
                {
                    int splinesInMesh = i < numMeshes - 1 ? splinesPerMesh : remaining;
                    Mesh mesh = new Mesh();
                    mesh.name = "Generated Road Mesh";
                    mesh.SetVertices(vertices, i * splinesPerMesh * verticesPerSpline, splinesInMesh * verticesPerSpline);
                    mesh.SetIndices(triangles, i * splinesPerMesh * indicesPerSpline, splinesInMesh * indicesPerSpline, MeshTopology.Triangles, 0);
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    var spline = EntityManager.CreateEntity(m_SplineMeshArchetype);
                    EntityManager.SetComponentData(spline, new LocalToWorld { Value = float4x4.identity });
                    EntityManager.SetSharedComponentData(spline, new RenderMesh
                    {
                        material = roadsMaterial,
                        mesh = mesh
                    });
                }
            }

            Debug.Log($"{triangles.Length} road triangles ({numMeshes} meshes)");

            vertices.Dispose();
            triangles.Dispose();

            return default;
        }
    }
}
