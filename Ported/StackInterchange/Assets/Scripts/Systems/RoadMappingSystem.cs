using Assets.Scripts.BlobData;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
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

            Entities.WithAll<CarSpawner>().ForEach((Entity roadStart, int entityInQueryIndex, in Translation translation, in RoadNode spawnerNode) =>
            {
                BuildRoadMap(roadNodes, translations, roadStart);

                //ecb.DestroyEntity(entityInQueryIndex, roadStart);   
            }).WithReadOnly(roadNodes).WithReadOnly(translations).Schedule();

            ecbSystem.AddJobHandleForProducer(Dependency);
        }

        static Dictionary<int, Node> BuildRoadMap(in ComponentDataFromEntity<RoadNode> roadNodes,
            in ComponentDataFromEntity<Translation> translations, Entity roadStart)
        {
            Dictionary<int, Node> traversedRoad = new Dictionary<int, Node>();
            int index = 0;
            Traverse(roadStart, in roadNodes, in translations, index, traversedRoad);

            return traversedRoad;
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
