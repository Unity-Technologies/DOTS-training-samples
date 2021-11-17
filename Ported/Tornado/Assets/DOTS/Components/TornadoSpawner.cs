using UnityEngine;
using Unity.Entities;

namespace Dots
{
    public class TornadoSpawner : IComponentData
    {
        public Mesh mesh;
        public Material material;

        public int debrisCount;
        public bool simulate;
        public float spinRate;
        public float upwardSpeed;
        public float initRange;
        public float force;
        public float maxForceDist;
        public float height;
        public float upForce;
        public float inwardForce;
    }
}