using Unity.Collections;
using Unity.Entities;

namespace DOTSRATS
{
    public class SceneDebugSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BoardSpawner>();
            RequireSingletonForUpdate<GameState>();
            RequireSingletonForUpdate<CellStruct>();
        }

        protected override void OnUpdate()
        {
            var debug = this.GetSingleton<GameObjectRefs>().debug;
            if (debug == null)
                return;

            var gameStateEntity = GetSingletonEntity<GameState>();
            var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);
            var cellStructs = GetBuffer<CellStruct>(gameStateEntity);

            debug.SetCellStructs(cellStructs);
            debug.BoardSpawner = GetSingleton<BoardSpawner>();
            debug.GameState = GetSingleton<GameState>();
        }
    }
}