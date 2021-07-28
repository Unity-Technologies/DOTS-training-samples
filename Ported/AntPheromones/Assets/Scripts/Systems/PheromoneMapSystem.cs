using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(AntMovementSystem))]
[UpdateAfter(typeof(AntExcitementSystem))]
public class PheromoneMapSystem : SystemBase
{
    private UnityEngine.Texture2D texture;

    protected override void OnStartRunning()
    {
        //UnityEngine.Debug.Log("OnStartRunning");

        var mapEntity = GetSingletonEntity<PheromoneMapSetting>();
        var mapSetting = GetComponent<PheromoneMapSetting>(mapEntity);

        if (texture != null && texture.width == mapSetting.Size && texture.height == mapSetting.Size)
            return;

        if (texture != null)
            UnityEngine.GameObject.Destroy(texture);

        texture = new UnityEngine.Texture2D(mapSetting.Size, mapSetting.Size, UnityEngine.TextureFormat.RGBAFloat, false);
        texture.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
        texture.filterMode = UnityEngine.FilterMode.Point;

        var buffer = GetBuffer<Pheromone>(mapEntity).Reinterpret<float4>();

        //Random rand = new Random(1234);
        //for (int i = 0; i < buffer.Length; i++)
        //{
        //    buffer[i] = rand.NextFloat4();       
        //}

        texture.LoadRawTextureData(buffer.AsNativeArray());
        texture.Apply();

        var pheromoneMapEntity = GetSingletonEntity<Pheromone>();
        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pheromoneMapEntity);
        renderMesh.material.mainTexture = texture;

        //UnityEngine.Debug.Log("OnStartRunning End");
    }

    protected override void OnUpdate()
    {
        //Debug.Log("PheromoneMapSystem OnUpdate");

        var mapSetting = GetSingleton<PheromoneMapSetting>();
        var trailSetting = GetSingleton<PheromoneTrailSetting>();
        var playerEntity = GetSingletonEntity<PlayerInput>();
        var playerSpeed = GetComponent<PlayerInput>(playerEntity).Speed;

        float deltaTime = Time.DeltaTime * playerSpeed;

        var pheromoneMapEntity = GetSingletonEntity<Pheromone>();
        var pheromoneMapBuffer = GetBuffer<Pheromone>(pheromoneMapEntity).Reinterpret<float4>();

        Entities
            .WithAll<Ant>()
            .ForEach((in Translation translation, in Excitement excitement) =>
            {
                if (TryGetClosestPheronomoneIndexFromTranslation(translation, mapSetting, out int index))
                {
                    float newPheromone = (trailSetting.Speed * excitement.Value * deltaTime) * (1f - pheromoneMapBuffer[index].x);
                    pheromoneMapBuffer[index] += new float4(math.min(newPheromone, 1f), 0, 0, 0);
                }

            }).Schedule();

        float trailDecay = 1f - trailSetting.Decay * deltaTime;
        var mapSize2 = mapSetting.Size * mapSetting.Size;

        Entities
            .ForEach((DynamicBuffer<Pheromone> pheromoneMap) =>
            {
                var buffer = pheromoneMap.Reinterpret<float4>();
                for (int i = 0; i < mapSize2; i++)
                {
                    buffer[i] *= trailDecay;
                }

            }).Schedule();

        //We need to complete the jobs to set the texture because we can't LoadRawTextureData or even have a Texture2D inside a job.
        Dependency.Complete();

        //PERFORMANCE: We probably can set the texture array with the DynamicBuffer reference one time and just do Apply to flush the array into the texture.
        texture.LoadRawTextureData(pheromoneMapBuffer.AsNativeArray());
        texture.Apply();
    }

    private static bool TryGetClosestPheronomoneIndexFromTranslation(in Translation translation, in PheromoneMapSetting mapSetting, out int index)
    {
        int x = (int)math.round((translation.Value.x - mapSetting.Offset.x) / mapSetting.WorldSize * mapSetting.Size);
        int y = (int)math.round((translation.Value.y - mapSetting.Offset.y) / mapSetting.WorldSize * mapSetting.Size);

        //I need to check because sometime the ant is goign beyond the bounds
        if (x < 0 || y < 0 || x >= mapSetting.Size || y >= mapSetting.Size)
        {
            index = 0;
            return false;
        }

        index = y * mapSetting.Size + x;
        return true;
    }
}
