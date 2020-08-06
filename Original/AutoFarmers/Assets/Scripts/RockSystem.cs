using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class RockSystem : SystemBase
{
    // private EntityCommandBufferSystem m_CmdBufSystem;

    protected override void OnUpdate()
    {
        Entities
        .WithStructuralChanges()
        .ForEach((Entity entity, in RockDataSpawner rockDataSpawner) =>
        {
            SpawnRock(rockDataSpawner.rockCount, rockDataSpawner);
            EntityManager.RemoveComponent<RockDataSpawner>(entity);

        }).Run();

        // DEBUG TEST
        /*
        var health = GetComponentDataFromEntity<StartHealth>();
        Entities
            .WithoutBurst()
            .WithNativeDisableParallelForRestriction(health)
            .WithAll<Rock>().ForEach((int entityInQueryIndex, Entity entity) =>
            {
                Debug.Log(health[entity].Value);

            }).Run();
        */
    }

    internal void SpawnRock(int count, in RockDataSpawner rockData)
    {
        for (int i = 0; i < count; ++i)
        {
            var rock = EntityManager.Instantiate(rockData.prefab);

            EntityManager.AddComponent<NonUniformScale>(rock);
            EntityManager.AddComponent<RockRect>(rock);
            EntityManager.AddComponent<Health>(rock);
            EntityManager.AddComponent<BatchNumber>(rock);

            // Init
            int width = UnityEngine.Random.Range(0, 4);
            int height = UnityEngine.Random.Range(0, 4);
            int rockX = UnityEngine.Random.Range(0, rockData.mapSize.x - width);
            int rockY = UnityEngine.Random.Range(0, rockData.mapSize.y - height);
            RectInt rect = new RectInt(rockX, rockY, width, height);
            int health = (rect.width + 1) * (rect.height + 1) * 15;
            int startHealth = health;
            float depth = UnityEngine.Random.Range(.4f, .8f);

            Vector2 center2D = rect.center;
            Vector3 worldPos = new Vector3(center2D.x + .5f, depth * .5f, center2D.y + .5f);
            Vector3 scale = new Vector3(rect.width + .5f, depth, rect.height + .5f);
            Matrix4x4 matx = Matrix4x4.TRS(worldPos, Quaternion.identity, scale);

            EntityManager.AddComponentData<Rock>(rock, new Rock { matrix = matx });
            EntityManager.AddComponentData<Health>(rock, new Health { Value = health});
            EntityManager.AddComponentData<StartHealth>(rock, new StartHealth { Value = health});

            EntityManager.SetComponentData<Translation>(rock, new Translation { Value = new Unity.Mathematics.float3(center2D.x + .5f, depth * .5f, center2D.y + .5f) });
            EntityManager.SetComponentData<NonUniformScale>(rock, new NonUniformScale { Value = new Unity.Mathematics.float3(rect.width + .5f, depth, rect.height + .5f) });
        }
    }

}
