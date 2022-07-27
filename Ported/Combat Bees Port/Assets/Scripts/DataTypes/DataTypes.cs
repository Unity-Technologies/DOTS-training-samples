using System;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    [Serializable]
    struct BasePosition
    {
        public float3 position;
        
        public float3 GetCenter()
        {
            var offset = 7.5f;
            if (position.x < 0)
            {
                offset *= -1;
            }
        
            return new float3(position.x - offset, position.y, position.z);
        }
    
        public float3 GetBaseLowerLeftCorner()
        {
            var offsetZ = 10;
            var offsetY = -10;

            return new float3(position.x, offsetY, position.y - offsetZ);
        }
        
        public float3 GetBaseRightCorner()
        {
            var offsetX = 15;
            var offsetZ = 10;
            var offsetY = 10;

            if (position.x < 0)
            {
                offsetX *= -1;
            }

            return new float3(position.x - offsetX, offsetY, position.y + offsetZ);
        }
    }
}
