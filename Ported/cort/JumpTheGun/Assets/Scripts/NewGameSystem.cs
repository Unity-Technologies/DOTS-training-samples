using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

namespace JumpTheGun
{
    [UpdateBefore(typeof(TerrainSystem))]
    public class NewGameSystem : ComponentSystem
    {
        private EntityQuery _newGameQuery;
        private EntityQuery _playerQuery;
        private Options _options;
        private TankFireSystem _tankFireSystem;
        private TimerText _timerText;
        protected override void OnCreateManager()
        {
            _playerQuery = GetEntityQuery(new ComponentType[]
            {
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<ArcState>(),
                ComponentType.Exclude<Scale>(),
            });
            _newGameQuery = GetEntityQuery(new ComponentType[]
            {
                ComponentType.ReadWrite<NewGameTag>(),
            });
            _tankFireSystem = World.GetOrCreateSystem<TankFireSystem>();
        }

        protected override void OnUpdate()
        {
            // Check for game-over condition (1+ entities with the NewGameTag component)
            var newGameChunks = _newGameQuery.CreateArchetypeChunkArray(Allocator.TempJob);
            if (newGameChunks.Length > 0)
            {
                if (_options == null)
                {
                    _options = GameObject.Find("Options").GetComponent<Options>();
                }

                Debug.Log("New game started!");

                // Destroy all existing entities between games
                var entityManager = World.EntityManager;
                var allEntities = entityManager.GetAllEntities();
                entityManager.DestroyEntity(allEntities);
                allEntities.Dispose();
                // If the bootstrap object still exists, it can be disposed of as well.
                Object.Destroy(GameObject.Find("Bootstrapper"));

                // Create cannonball prefab entity
                _tankFireSystem.CreateCannonballPrefab();

                // Reset the terrain -- this destroys any existing cached terrain data, forcing it to be regenerated
                // the next time the system is updated.
                var terrain = World.GetOrCreateSystem<TerrainSystem>();
                terrain.Reset();

                // Reset the timer text to zero
                if (_timerText == null)
                {
                    _timerText = GameObject.Find("TimerText").GetComponent<TimerText>();
                }
                _timerText.ResetToZero();
            }
            newGameChunks.Dispose();
        }
    }
}
