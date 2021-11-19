using System.Collections.Generic;
using System.Linq;
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

        public NativeArray<BeamData> beams;
        public NativeArray<Anchor> anchors;
        public NativeArray<int> components;
        public NativeArray<int> componentsData;
        EntityArchetype m_BeamArchetype;

        public NativeReference<int> pointCount;

        public bool initialized;

        protected override void OnCreate()
        {
            m_BeamArchetype = EntityManager.CreateArchetype(
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<NonUniformScale>(),
                ComponentType.ReadWrite<Beam>());
            m_BuildingSpawnerQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(BuildingSpawner) }
            });
            RequireForUpdate(m_BuildingSpawnerQuery);
            pointCount = new NativeReference<int>(0, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            beams.Dispose();
            anchors.Dispose();
            pointCount.Dispose();
            components.Dispose();
            componentsData.Dispose();
        }

        protected override void OnUpdate()
        {
            var random = new Random(1234);

            var go = this.GetSingleton<GameObjectRefs>().Ground;
            var ground = go.GetComponent<Transform>();
            var groundSize = ground.localScale;
            var groundHalfX = groundSize.x / 2;
            var groundHalfY = groundSize.y / 2;

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

                    var anchorList = new List<Anchor>();
                    var beamList = new List<BeamDataSpawn>();
                    var pointCount = 0;

                    // buildings
                    for (int i = 0; i < buildingCount; i++)
                    {
                        int height = random.NextInt(minBuildingHeight, maxBuildingHeight);
                        Vector3 pos = new Vector3(random.NextFloat(-(groundHalfX - 10), groundHalfX - 10), 0f, random.NextFloat(-(groundHalfX - 10), groundHalfX - 10));
                        float spacing = 2f;
                        for (int j = 0; j < height; j++)
                        {
                            var anchor = new Anchor();
                            anchor.position = anchor.oldPosition = new float3(pos.x + spacing, j * spacing, pos.z - spacing);
                            if (anchor.position.y == 0)
                                anchor.anchored = true;
                            anchorList.Add(anchor);

                            anchor = new Anchor();
                            anchor.position = anchor.oldPosition = new float3(pos.x - spacing, j * spacing, pos.z - spacing);
                            if (anchor.position.y == 0)
                                anchor.anchored = true;
                            anchorList.Add(anchor);

                            anchor = new Anchor();
                            anchor.position = anchor.oldPosition = new float3(pos.x + 0f, j * spacing, pos.z + spacing);
                            if (anchor.position.y == 0)
                                anchor.anchored = true;
                            anchorList.Add(anchor);
                        }
                    }

                    // ground details
                    for (int i = 0; i < debrisCount; i++)
                    {
                        Vector3 pos = new Vector3(random.NextFloat(-groundHalfX, groundHalfX), 0f, random.NextFloat(-groundHalfY, groundHalfY));

                        var anchor = new Anchor();
                        anchor.position = anchor.oldPosition = new float3(pos.x + random.NextFloat(-.2f, -.1f), pos.y + random.NextFloat(0f, 3f), pos.z + random.NextFloat(.1f, .2f));
                        if (anchor.position.y == 0)
                            anchor.anchored = true;
                        anchorList.Add(anchor);

                        anchor = new Anchor();
                        anchor.position = anchor.oldPosition = new float3(pos.x + random.NextFloat(.2f, .1f), pos.y + random.NextFloat(0f, .2f), pos.z + random.NextFloat(-.1f, -.2f));
                        if (anchor.position.y == 0)
                            anchor.anchored = true;
                        anchorList.Add(anchor);
                    }


                    var connectedComponents = new List<ConnectedComponent>();
                    var pointToCC = new Dictionary<int, ConnectedComponent>();
                    for (int i = 0; i < anchorList.Count; i++)
                    {
                        for (int j = i + 1; j < anchorList.Count; j++)
                        {
                            var point1 = anchorList[i];
                            var point2 = anchorList[j];

                            Vector3 delta = point2.position - point1.position;
                            var length = delta.magnitude;

                            var thickness = random.NextFloat(thicknessMin, thicknessMax);
                            if (length <= .2f || length >= 5f)
                                continue;

                            point1.neighborCount++;
                            point2.neighborCount++;
                            anchorList[i] = point1;
                            anchorList[j] = point2;

                            Vector3 pos = (point1.position + point2.position) * .5f;
                            Quaternion rot = Quaternion.LookRotation(delta);
                            Vector3 scale = new Vector3(thickness, thickness, length);

                            var beamDataSpawn = new BeamDataSpawn()
                            {
                                p1Id = i,
                                p2Id = j,
                                length = length,
                                pos = pos,
                                rot = rot,
                                scale = scale
                            };
                            beamList.Add(beamDataSpawn);
                            var beamIndex = beamList.Count - 1;

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

                    anchors = new NativeArray<Anchor>(beamList.Count * 2, Allocator.Persistent);
                    for (var i = 0; i < anchorList.Count; ++i)
                    {
                        anchors[i] = anchorList[i];
                    }

                    this.pointCount.Value = anchorList.Count;

                    var beamDatas = new List<BeamData>();
                    foreach (var beamDataSpawn in beamList)
                    {
                        CreateBeamEntity(beamDataSpawn, beamDatas.Count, spawner.mesh, spawner.material);

                        var beamData = new BeamData()
                        {
                            p1 = beamDataSpawn.p1Id,
                            p2 = beamDataSpawn.p2Id,
                            oldD = new float3(0, 0, 0),
                            newD = new float3(0, 0, 0),
                            length = beamDataSpawn.length
                        };
                        beamDatas.Add(beamData);
                    }

                    beams = new NativeArray<BeamData>(beamDatas.ToArray(), Allocator.Persistent);

                    var componentCount = connectedComponents.Count(cc => cc.root == null);
                    var maxComponentSize = connectedComponents.Where(cc => cc.root == null).Max(cc => cc.beams.Count);
                    var componentList = new int[componentCount * 2];
                    var componentDataList = new int[componentCount * maxComponentSize];
                    var componentIndex = 0;
                    var componentDataIndex = 0;
                    foreach (var connectedComponent in connectedComponents)
                    {
                        if (connectedComponent.root != null)
                            continue;

                        var beamCount = connectedComponent.beams.Count;
                        componentList[componentIndex] = componentDataIndex;
                        componentList[componentIndex+1] = beamCount;
                        var currentDataIndex = componentDataIndex;
                        foreach (var beam in connectedComponent.beams)
                        {
                            componentDataList[currentDataIndex] = beam;
                            ++currentDataIndex;
                        }

                        componentIndex += 2;
                        componentDataIndex += maxComponentSize;
                    }

                    components = new NativeArray<int>(componentList, Allocator.Persistent);
                    componentsData = new NativeArray<int>(componentDataList, Allocator.Persistent);

                    Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"Built {connectedComponents.Count(cc => cc.root == null)} connected components");
                }
            }

            initialized = true;
        }

        Entity CreateBeamEntity(BeamDataSpawn beamData, int index, Mesh mesh, Material material)
        {
            var beamEntity = EntityManager.CreateEntity(m_BeamArchetype);
            EntityManager.SetComponentData(beamEntity, new Translation
            {
                Value = beamData.pos
            });
            EntityManager.AddComponentData(beamEntity, new Rotation()
            {
                Value = beamData.rot
            });
            EntityManager.AddComponentData(beamEntity, new NonUniformScale()
            {
                Value = beamData.scale
            });

            EntityManager.AddComponentData(beamEntity, new Beam()
            {
                beamDataIndex = index
            });

            var desc = new RenderMeshDescription(mesh, material);
            RenderMeshUtility.AddComponents(beamEntity, EntityManager, desc);

            return beamEntity;
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