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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BuildingSpawnerSystem : SystemBase
    {
        private EntityQuery m_BuildingSpawnerQuery;

        static readonly ComponentType k_FixedAnchorComponent = ComponentType.ReadOnly(typeof(Dots.FixedAnchor));

        public NativeArray<BeamData> beams;

        protected override void OnCreate()
        {
            m_BuildingSpawnerQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(BuildingSpawner) }
            });
            RequireForUpdate(m_BuildingSpawnerQuery);
        }

        protected override void OnDestroy()
        {
            beams.Dispose();
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
                    var thicknessMin = spawner.thicknessMin;
                    var thicknessMax = spawner.thicknessMax;

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

                    var beamDatas = new List<BeamData>();
                    var connectedComponents = new List<ConnectedComponent>();
                    var pointToCC = new Dictionary<int, ConnectedComponent>();
                    for (int i = 0; i < pointCount; i++)
                    {
                        for (int j = i + 1; j < pointCount; j++)
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

                            var barEntity = EntityManager.CreateEntity();
                            EntityManager.AddComponentData(barEntity, new Translation
                            {
                                Value = pos
                            });
                            EntityManager.AddComponentData(barEntity, new Rotation()
                            {
                                Value = rot
                            });
                            EntityManager.AddComponentData(barEntity, new NonUniformScale()
                            {
                                Value = scale
                            });

                            var desc = new RenderMeshDescription(spawner.mesh, spawner.material);
                            RenderMeshUtility.AddComponents(barEntity, EntityManager, desc);

                            var beamData = new BeamData()
                            {
                                p1 = p1,
                                p2 = p2,
                                oldD = new float3(0, 0, 0),
                                newD = new float3(0, 0, 0),
                                length = length
                            };
                            beamDatas.Add(beamData);
                            var beamIndex = beamDatas.Count - 1;
                            EntityManager.AddComponentData(barEntity, new Beam()
                            {
                                beamDataIndex = beamDatas.Count - 1
                            });


                            if (!pointToCC.ContainsKey(i) && !pointToCC.ContainsKey(j))
                            {
                                var currentComponent = new ConnectedComponent(i);
                                currentComponent.AddBeam(beamIndex);
                                pointToCC.Add(i, currentComponent);
                                pointToCC.Add(j, currentComponent);
                                connectedComponents.Add(currentComponent);
                            }
                            else if (pointToCC.ContainsKey(i) && pointToCC.ContainsKey(j))
                            {
                                var cc1 = pointToCC[i];
                                var cc2 = pointToCC[j];
                                cc1.Merge(cc2);
                                cc1.AddBeam(beamIndex);
                            }
                            else if (pointToCC.ContainsKey(i))
                            {
                                var cc = pointToCC[i];
                                cc.AddBeam(beamIndex);
                                pointToCC.Add(j, cc);
                            }
                            else
                            {
                                var cc = pointToCC[j];
                                cc.AddBeam(beamIndex);
                                pointToCC.Add(i, cc);
                            }
                        }
                    }

                    beams = new NativeArray<BeamData>(beamDatas.ToArray(), Allocator.Persistent);

                    foreach (var connectedComponent in connectedComponents)
                    {
                        if (connectedComponent.root != null)
                            continue;

                        var beamGroup = EntityManager.CreateEntity();
                        var entityBuffer = EntityManager.AddBuffer<BeamBufferElement>(beamGroup);
                        foreach (var beam in connectedComponent.beams)
                        {
                            entityBuffer.Add(beam);
                        }
                    }
                }
            }
        }

        Entity CreatePoint(float3 position)
        {
            var pointEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(pointEntity, new Anchor()
            {
                position = position,
                oldPosition = position,
                neighborCount = 0
            });
            if (position.y == 0)
            {
                EntityManager.AddComponent(pointEntity, k_FixedAnchorComponent);
            }

            return pointEntity;
        }
    }

    class ConnectedComponent
    {
        public ConnectedComponent root { get; set; }

        public List<int> beams;
        public int id { get; private set; }

        public ConnectedComponent(int id)
        {
            beams = new List<int>();
            root = null;
            this.id = id;
        }

        public void Merge(ConnectedComponent other)
        {
            if (id == other.id)
                return;

            if (id < other.id)
            {
                GetRoot().beams.AddRange(other.beams);
                other.beams.Clear();
                other.id = id;
                other.root = this;
            }
            else
            {
                other.GetRoot().beams.AddRange(beams);
                beams.Clear();
                id = other.id;
                root = other;
            }
        }

        public ConnectedComponent GetRoot()
        {
            var currentCC = this;
            while (currentCC.root != null)
                currentCC = currentCC.root;
            return currentCC;
        }

        public void AddBeam(int beamId)
        {
            GetRoot().beams.Add(beamId);
        }
    }

    // InternalBufferCapacity specifies how many elements a buffer can have before
    // the buffer storage is moved outside the chunk.
    [InternalBufferCapacity(8)]
    public struct BeamBufferElement : IBufferElementData
    {
        // Actual value each buffer element will store.
        public int Value;

        // The following implicit conversions are optional, but can be convenient.
        public static implicit operator int(BeamBufferElement e)
        {
            return e.Value;
        }

        public static implicit operator BeamBufferElement(int e)
        {
            return new BeamBufferElement { Value = e };
        }
    }

    struct BeamDataSpawn
    {
        public int p1Id;
        public int p2Id;

        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public float length;
    }
}