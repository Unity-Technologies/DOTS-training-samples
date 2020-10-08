using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.BlobData
{
    public struct Node
    {
        public Translation translation;
        public float4 Color;
        public ColorMask.Masks PathBitField;

        public int[] Childern;

        public int index;

        public int GetNext(float4 carColor, Dictionary<int, Node> road)
        {
            ColorMask.Masks mask = ColorMask.GetMask(carColor);
            int next = -1;
            for(int i = 0; i < Childern.Length; i++)
            {
                // TODO - Broken -> Default stay on the same road.
                if (next == -1 && road[Childern[i]].Color.Equals(Color))
                    next = Childern[i];

                if ((road[Childern[i]].PathBitField & mask) != ColorMask.Masks.None)
                    next = Childern[i];
            }
            return next;
        }
    }
}
