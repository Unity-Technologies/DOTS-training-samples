using Unity.Rendering;
using UnityEngine;

namespace JumpTheGun
{
    public class Options : MonoBehaviour
    {
        [UnityEngine.Range(10, 100)] public int terrainSizeX = 15;
        [UnityEngine.Range(10, 100)] public int terrainSizeZ = 15;
        [UnityEngine.Range(0.5f, 10.0f)] public float terrainHeightMin = 2.5f;
        [UnityEngine.Range(0.5f, 10.0f)] public float terrainHeightMax = 5.5f;
        [UnityEngine.Range(0.0f, 10.0f)] public float boxHeightDamage = 0.4f;
        [UnityEngine.Range(1, 1000)] public int tankCount = 5;
        [UnityEngine.Range(0.1f, 20.0f)] public float tankLaunchPeriod = 1.0f;
        public bool invincibility = false;
        public RenderMeshProxy playerLook;
        public RenderMeshProxy tankBaseLook;
        public RenderMeshProxy tankCannonLook;
        public RenderMeshProxy bulletLook;
        public RenderMeshProxy blockLook;
    }
}
