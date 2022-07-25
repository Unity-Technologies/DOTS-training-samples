using Components;
using Unity.Entities;

namespace Authorings
{
    class InitialSpawnAuthoring : UnityEngine.MonoBehaviour
    {
    }

    class InitialSpawnBaker : Baker<InitialSpawnAuthoring>
    {
        public override void Bake(InitialSpawnAuthoring authoring)
        {
            AddComponent<InitialSpawn>();
        }
    }
}
