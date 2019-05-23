using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace JumpTheGun
{
    [UpdateAfter(typeof(PlayerBounceSystem))]
    public class PlayerPositionCacheSystem : ComponentSystem
    {
        public float3 PlayerPosition { get; private set; }

        private EntityQuery _playerQuery;
        protected override void OnCreateManager()
        {
            _playerQuery = GetEntityQuery(new ComponentType[]
            {
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<ArcState>(),
                ComponentType.Exclude<Scale>(), // necessary to exclude cannonball entities
            });
        }

        public void SetInitialPosition(float3 initialPlayerPos)
        {
            PlayerPosition = initialPlayerPos;
        }

        protected override void OnUpdate()
        {
            // Retrieve the player entity's Position.
            var playerChunks = _playerQuery.CreateArchetypeChunkArray( Allocator.TempJob);
            Assert.AreEqual(1, playerChunks.Length);
            var playerPositionType = GetArchetypeChunkComponentType<Translation>(true);
            var playerPositions = playerChunks[0].GetNativeArray(playerPositionType);
            Assert.AreEqual(1, playerPositions.Length);
            PlayerPosition = playerPositions[0].Value;
            playerChunks.Dispose();
        }
    }
}
