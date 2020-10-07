using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.BlobData
{
    class Node
    {
        public Translation translation;
        public float4 Color;
        public ColorMask.Masks PathBitField;

        public List<Node> Childern = new List<Node>();
        public long index;

        public Node GetNext(float4 carColor)
        {
            ColorMask.Masks mask = ColorMask.GetMask(carColor);

            Node next = null;
            foreach(var child in Childern)
            {
                // Default stay on the same road.
                if (next == null && child.Color.Equals(Color))
                    next = child;

                if ((child.PathBitField & mask) != ColorMask.Masks.None)
                    next = child;
            }
            return next;
        }
    }
}
