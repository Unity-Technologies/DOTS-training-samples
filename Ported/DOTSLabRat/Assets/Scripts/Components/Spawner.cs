using Unity.Entities;

namespace DOTSRATS
{
    // Might make sense to move this into the GameState component if its never going to change
    public struct Spawner : IComponentData
    {
        public int maxRats;
        public int maxCats;
        public float spawnRate;
    }
}
