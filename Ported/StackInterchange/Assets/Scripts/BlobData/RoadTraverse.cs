using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.BlobData
{
    class RoadTraverse
    {
        List<Node> Road = new List<Node>();
        private long _index = 0;

        public void BuildRoad(RoadNode spawnerNode)
        {
            Traverse(spawnerNode);
        }

        public Node Traverse(RoadNode node)
        {
            if (node.Equals(default(RoadNode)))
                return null;

            // TODO - remove tmp color 
            float4 color = new float4(1, 1, 1, 1);
            var mask = ColorMask.GetMask(color);
            Node blobNode = new Node
            {
                PathBitField = mask,
                index = _index++,
                // TODO - Get value
                translation = default(Translation)
            };
            Road.Add(blobNode);

            // TODO - get the next node
            var nextNode = node;
            while(!nextNode.Equals(default(RoadNode)))
            {
                var child = Traverse(nextNode);
                if (child == null)
                    continue;

                blobNode.Childern.Add(child);
                blobNode.PathBitField |= child.PathBitField;

            }
            return blobNode;
        }
    }
}
