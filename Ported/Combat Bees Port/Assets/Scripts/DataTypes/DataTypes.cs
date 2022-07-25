using System;
using Unity.Mathematics;

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
    
        // public float3 GetCenter()
        // {
        //     
        // }
    }
}
