using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public struct LastPosition : IComponentData
{
    public float3 Value;
}

[UpdateAfter(typeof(AntMovementSystem))]
[UpdateAfter(typeof(AntExcitementSystem))]
public class PheromoneMapSystem : SystemBase
{
    private UnityEngine.Texture2D texture;

    protected override void OnStartRunning()
    {
        //UnityEngine.Debug.Log("OnStartRunning");

        var mapEntity = GetSingletonEntity<MapSetting>();
        var mapSetting = GetComponent<MapSetting>(mapEntity);

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

        var mapSetting = GetSingleton<MapSetting>();
        var trailSetting = GetSingleton<PheromoneTrailSetting>();
        var playerEntity = GetSingletonEntity<PlayerInput>();
        var playerSpeed = GetComponent<PlayerInput>(playerEntity).Speed;

        float deltaTime = Time.DeltaTime;

        var pheromoneMapEntity = GetSingletonEntity<Pheromone>();
        var pheromoneMapBuffer = GetBuffer<Pheromone>(pheromoneMapEntity).Reinterpret<float4>();

        Entities
            .WithAll<Ant>()
            .ForEach((ref LastPosition lastPos, in Translation translation, in Excitement excitement) =>
            {
                float length = math.length(translation.Value - lastPos.Value);
                float3 dir = (translation.Value - lastPos.Value) / length;

                int lastIndex = -1;
                float step = length / playerSpeed;
                for (float currSteps = 0; currSteps < length; currSteps += step)
                {
                    if (TryGetClosestPheronomoneIndexFromTranslation(dir * currSteps + lastPos.Value, mapSetting, out int index) && lastIndex != index)
                    {
                        float newPheromone = (trailSetting.Speed * excitement.Value * deltaTime) * (1f - pheromoneMapBuffer[index].x);
                        pheromoneMapBuffer[index] += new float4(math.min(newPheromone, 1f), 0, 0, 0);

                        lastPos.Value = translation.Value;
                        lastIndex = index;
                    }
                }

                //if (TryGetClosestPheronomoneIndexFromTranslation(translation.Value, mapSetting, out int index) && 
                //    TryGetClosestPheronomoneIndexFromTranslation(lastPos.Value, mapSetting, out int lastIndex) &&
                //    index != lastIndex)
                //{
                //    float newPheromone = (trailSetting.Speed * excitement.Value * deltaTime) * (1f - pheromoneMapBuffer[index].x);
                //    pheromoneMapBuffer[index] += new float4(math.min(newPheromone, 1f), 0, 0, 0);
                //}

                //lastPos.Value = translation.Value;

            }).Schedule();

        float trailDecay = math.pow(1f - trailSetting.Decay * deltaTime, playerSpeed);
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

    public static bool TryGetClosestPheronomoneIndexFromTranslation(float3 position, in MapSetting mapSetting, out int index)
    {
        int x = (int)math.round((position.x - mapSetting.Offset.x) / mapSetting.WorldSize * mapSetting.Size);
        int y = (int)math.round((position.y - mapSetting.Offset.y) / mapSetting.WorldSize * mapSetting.Size);

        //I need to check because sometime the ant is going beyond the bounds
        if (x < 0 || y < 0 || x >= mapSetting.Size || y >= mapSetting.Size)
        {
            index = 0;
            return false;
        }

        index = y * mapSetting.Size + x;
        return true;
    }
}
