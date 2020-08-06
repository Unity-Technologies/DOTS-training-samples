using Unity.Entities;
using Unity.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

[UpdateAfter(typeof(PheromoneOutputSystem))]
public class DecayPheromoneSystem : SystemBase
{
    private static AntDefaults defaults;

    protected override void OnCreate()
    {
        base.OnCreate();
        defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
        defaults.InitPheromoneBuffers(defaults.mapSize, defaults.mapSize);
    }

    protected override void OnUpdate()
    {
        int sizeOfBuffer = defaults.bufferSize;
        NativeArray<float> pheromoneMap = defaults.GetCurrentPheromoneMapBuffer();
/*
        Entities
            .ForEach((Entity entity, in PheromoneDecay decay) =>
            {

                for (int x = 0; x < sizeOfBuffer; x++)
                    pheromoneMap[x] *= 1f;//decay.decaySpeed;
            }).Run();
*/
        defaults.SwapPheromoneBuffer();
    }

#if UNITY_EDITOR
    class CreateAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            AntDefaults defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
            if (defaults == null || defaults.Equals(null))
            {
                Debug.LogError($"There is no {typeof(AntDefaults)}");
                return;
            }

            //RenderTexture rt = new RenderTexture(defaults.mapSize, defaults.mapSize, 0, RenderTextureFormat.RG32);
            //rt.Create();
            Texture2D rt = new Texture2D(defaults.mapSize, defaults.mapSize, UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            AssetDatabase.CreateAsset(rt, pathName);
            Selection.activeObject = rt;

            if (defaults.pheromoneMap == null || defaults.pheromoneMap.Equals(null))
                defaults.pheromoneMap = rt;
        }
    }

    [MenuItem("Assets/Create/PheromoneMap", priority = 0)]
    static void CreateMapMenu()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateAsset>(), "PheromoneMap.asset", null, null);
    }
#endif
}
