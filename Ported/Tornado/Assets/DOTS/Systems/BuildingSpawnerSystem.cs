using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    public partial class BuildingSpawnerSystem : SystemBase
    {

        private EntityQuery m_BuildingSpawnerQuery;

        static readonly ComponentType k_PointComponent = ComponentType.ReadWrite(typeof(Dots.Point));
        static readonly ComponentType k_AnchorComponent = ComponentType.ReadOnly(typeof(Dots.FixedAnchor));
        static readonly ComponentType k_AnchorListComponent = ComponentType.ReadWrite(typeof(Dots.AnchorList));
        static readonly ComponentType k_TranslationComponent = ComponentType.ReadWrite(typeof(Translation));
        static readonly ComponentType k_RotationComponent = ComponentType.ReadWrite(typeof(Rotation));
        static readonly ComponentType k_ScaleComponent = ComponentType.ReadWrite(typeof(NonUniformScale));
        static readonly ComponentTypes k_TransformComponent = new ComponentTypes(k_TranslationComponent, k_RotationComponent, k_ScaleComponent);

        protected override void OnCreate()
        {

            m_BuildingSpawnerQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(BuildingSpawner) }
            });
            RequireForUpdate(m_BuildingSpawnerQuery);
        }

        protected override void OnUpdate()
        {

            var random = new Random(1234);

            using (var entities = m_BuildingSpawnerQuery.ToEntityArray(Allocator.TempJob))
            {
                foreach (Entity entity in entities)
                {
                    BuildingSpawner spawner = EntityManager.GetComponentObject<BuildingSpawner>(entity);
                    EntityManager.DestroyEntity(entity);

                    var buildingCount = spawner.buildingCount;
                    var debrisCount = spawner.debrisCount;
                    var minBuildingHeight = spawner.minHeight;
                    var maxBuildingHeight = spawner.maxHeight;
                    var thicknessMin = .25f;
                    var thicknessMax = .35f;

                    var pointPosList = new NativeArray<float3>(buildingCount * maxBuildingHeight * 3 + debrisCount * 2, Allocator.Temp);
                    var pointCount = 0;

                    // buildings
                    var pointEntities = new List<Entity>();
                    for (int i = 0; i < buildingCount; i++)
                    {
                        int height = random.NextInt(minBuildingHeight, maxBuildingHeight);
                        Vector3 pos = new Vector3(random.NextFloat(-45f, 45f), 0f, random.NextFloat(-45f, 45f));
                        float spacing = 2f;
                        for (int j = 0; j < height; j++)
                        {
                            var position = new float3(pos.x + spacing, j * spacing, pos.z - spacing);
                            pointPosList[pointCount++] = position;
                            var pointEntity = CreatePoint(position);
                            pointEntities.Add(pointEntity);

                            position = new float3(pos.x - spacing, j * spacing, pos.z - spacing);
                            pointPosList[pointCount++] = position;
                            pointEntity = CreatePoint(position);
                            pointEntities.Add(pointEntity);

                            position = new float3(pos.x + 0f, j * spacing, pos.z + spacing);
                            pointPosList[pointCount++] = position;
                            pointEntity = CreatePoint(position);
                            pointEntities.Add(pointEntity);
                        }
                    }

                    // ground details
                    for (int i = 0; i < debrisCount; i++)
                    {
                        Vector3 pos = new Vector3(random.NextFloat(-55f, 55f), 0f, random.NextFloat(-55f, 55f));

                        var position = new float3(pos.x + random.NextFloat(-.2f, -.1f), pos.y + random.NextFloat(0f, 3f), pos.z + random.NextFloat(.1f, .2f));
                        pointPosList[pointCount++] = position;
                        var pointEntity = CreatePoint(position);
                        pointEntities.Add(pointEntity);

                        position = new float3(pos.x + random.NextFloat(.2f, .1f), pos.y + random.NextFloat(0f, .2f), pos.z + random.NextFloat(-.1f, -.2f));
                        pointPosList[pointCount++] = position;
                        pointEntity = CreatePoint(position);
                        pointEntities.Add(pointEntity);
                    }

                    for (int i = 0; i < pointEntities.Count; i++)
                    {
                        for (int j = i + 1; j < pointEntities.Count; j++)
                        {
                            var p1 = pointEntities[i];
                            var p2 = pointEntities[j];

                            var point1 = pointPosList[i];
                            var point2 = pointPosList[j];

                            Vector3 delta = new Vector3(point2.x - point1.x, point2.y - point1.y, point2.z - point1.z);
                            var length = delta.magnitude;

                            var thickness = random.NextFloat(thicknessMin, thicknessMax);
                            if (length <= .2f || length >= 5f)
                                continue;

                            Vector3 pos = new Vector3(point1.x + point2.x, point1.y + point2.y, point1.z + point2.z) * .5f;
                            Quaternion rot = Quaternion.LookRotation(delta);
                            Vector3 scale = new Vector3(thickness, thickness, length);
                            var matrix = Matrix4x4.TRS(pos, rot, scale);

                            var barEntity = EntityManager.CreateEntity();
                            EntityManager.AddComponents(barEntity, k_TransformComponent);
                            EntityManager.SetComponentData(barEntity, new Translation
                            {
                                Value = pos
                            });
                            EntityManager.SetComponentData(barEntity, new Rotation()
                            {
                                Value = rot
                            });
                            EntityManager.SetComponentData(barEntity, new NonUniformScale()
                            {
                                Value = scale
                            });

                            var desc = new RenderMeshDescription(spawner.mesh, spawner.material);
                            RenderMeshUtility.AddComponents(barEntity, EntityManager, desc);

                            EntityManager.AddComponent(barEntity, k_AnchorListComponent);
                            EntityManager.SetComponentData(barEntity, new AnchorList()
                            {
                                p1 = p1,
                                p2 = p2
                            });
                        }
                    }
                }
            }
        }

        Entity CreatePoint(float3 position)
        {
            var pointEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent(pointEntity, k_PointComponent);
            EntityManager.SetComponentData(pointEntity, new Point()
            {
                value = position
            });
            if (position.y == 0)
            {
                EntityManager.AddComponent(pointEntity, k_AnchorComponent);
            }

            return pointEntity;
        }
    }
}