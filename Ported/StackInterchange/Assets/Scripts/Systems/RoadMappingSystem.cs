using Assets.Scripts.BlobData;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    class RoadMappingSystem : SystemBase
    {
        private EntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var roadNodes = GetComponentDataFromEntity<RoadNode>(true);
            var translations = GetComponentDataFromEntity<Translation>(true);
            var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.WithAll<CarSpawner>().ForEach((Entity roadStart, int entityInQueryIndex) =>
            {
                BuildRoadMap(roadNodes, translations, roadStart);

                ecb.DestroyEntity(entityInQueryIndex, roadStart);   
            }).WithReadOnly(roadNodes).WithReadOnly(translations).WithoutBurst().ScheduleParallel();

            ecbSystem.AddJobHandleForProducer(Dependency);
        }

        static BlobAssetReference<RoadAsset> BuildRoadMap(in ComponentDataFromEntity<RoadNode> roadNodes,
            in ComponentDataFromEntity<Translation> translations, Entity roadStart)
        {
            Dictionary<int, Node> traversedRoad = new Dictionary<int, Node>();
            int index = 0;
            Traverse(roadStart, in roadNodes, in translations, index, traversedRoad);

            int size = 0;
            for (int i = 0; i < traversedRoad.Count; i++)
            {
                var entry = traversedRoad.ElementAt(i);

                if (size < entry.Key)
                    size = entry.Key;
            }
            return default;

            /*
            if(!roads.Equals(default))
            {
                for(int i = 0; i < roads.Value.Value.Nodes.GetLength(0); i++)
                {
                    if(roads.Value.Value.Nodes[i, 0].Equals(default))
                    {
                        for (int j = 0; j < traversedRoad.Count; j++)
                        {
                            var entry = traversedRoad.ElementAt(i);
                            roads.Value.Value.Nodes[i, entry.Value.index] = entry.Value;
                        }
                        break;
                    }
                }
            }

            using(BlobBuilder builder = new BlobBuilder(Unity.Collections.Allocator.Temp) )
            {
                ref var root = ref builder.ConstructRoot<RoadAsset>();
                root.Nodes = new Node[100, size + 1];
                root.RoadId = 1234;
                for(int i = 0; i < traversedRoad.Count; i++)
                {
                    var entry = traversedRoad.ElementAt(i);
                    root.Nodes[0, entry.Value.index] = entry.Value;
                }
                return builder.CreateBlobAssetReference<RoadAsset>(Unity.Collections.Allocator.Temp);
            }
            */
        }

        static Node Traverse(Entity node, in ComponentDataFromEntity<RoadNode> roadNodes, 
            in ComponentDataFromEntity<Translation> translations, int index, Dictionary<int, Node> traversedRoad)
        {
            if (node == Entity.Null)
                return default;

            var entity = roadNodes[node];
            Node blobNode = new Node
            {
                PathBitField = ColorMask.GetMask(entity.color),
                index = index++,
                Childern = new int[0],
                translation = translations[node]
            };
            traversedRoad[index] = blobNode;

            Entity[] routes = { entity.nextNode, entity.exitNode };
            for (var i = 0; i < routes.Length; i++)
            {
                if (routes[i] == Entity.Null)
                    continue;

                var child = Traverse(routes[i], in roadNodes, in translations, index, traversedRoad);
                if (child.Equals(default(Node)))
                    continue;

                int[] childList = new int[blobNode.Childern.Length + 1];
                blobNode.Childern.CopyTo(childList, 0);
                childList[childList.Length - 1] = child.index;

                // TODO - look into make this not terrible
                blobNode = new Node
                {
                    PathBitField = blobNode.PathBitField | child.PathBitField,
                    index = blobNode.index,
                    Childern = childList,
                    translation = blobNode.translation
                };
                traversedRoad[blobNode.index] = blobNode;
            }

            return blobNode;
        }
    }
}
