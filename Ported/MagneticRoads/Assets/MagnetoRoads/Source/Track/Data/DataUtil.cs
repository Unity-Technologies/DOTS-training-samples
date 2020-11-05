using Unity.Mathematics;
using UnityEngine;

namespace Magneto.Track
{
    public static class DataUtil
    {
        public static Vector3 GetVector3(this int3 source)
        {
            return new Vector3(source.x, source.y, source.z);
        }
    }
}