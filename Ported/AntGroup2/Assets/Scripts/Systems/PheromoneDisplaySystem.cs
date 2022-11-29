using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = UnityEngine.Random;

//[UpdateAfter(typeof(PheromoneDecaySystem))]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class PheromoneDisplaySystem : SystemBase
{
    public const int PheromoneTextureSizeX = 64;
    public const int PheromoneTextureSizeY = 64;
        
    public GameObject displayPlane;
    private Material displayMaterial;
    private Texture2D pheromoneTexture;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        displayPlane = GameObject.Find("DisplayPlane");
        
        pheromoneTexture = new Texture2D(PheromoneTextureSizeX, PheromoneTextureSizeY);   // TEMP: Hardcoded size
        
        if (displayPlane.TryGetComponent<Renderer>(out var displayRenderer))
            displayMaterial = displayRenderer.material;

        // TEMP: Dummy Add a single entity.
        //var e = EntityManager.CreateEntity();
        //EntityManager.AddComponent<PheromoneMap>(e);
    }

    protected override void OnUpdate()
    {
        // Assume there's only one PheromoneMap
        Entities.ForEach((Entity ent, in DynamicBuffer<PheromoneMap> map) =>
        {
            //DynamicBuffer<PheromoneMap> mapBuffer = EntityManager.GetBuffer<PheromoneMap>(ent);
            // TODO...
            
            //Debug.Log("Foo");

            int2 texSize = new int2(pheromoneTexture.width, pheromoneTexture.height);
            
            // Clear
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    pheromoneTexture.SetPixel(i, j, Color.black);
                }    
            }
            
            // Random fill contents
            /*for (int i = 0; i < 64; i++)
            {
                float x = Random.value;
                float y = Random.value;
            
                pheromoneTexture.SetPixel((int)(x*(texSize.x-1)), (int)(y*(texSize.x-1)), Color.red);
            }*/
            
            for (int i = 0; i < map.Length; i++)
            {
                float amount = map[i].amount;
                int colorAmount = (int)(math.saturate(amount) * 255);

                int x = i % PheromoneTextureSizeX;
                int y = i / PheromoneTextureSizeX;

                pheromoneTexture.SetPixel(x, y, new Color(colorAmount, 0, 0, 255));
            }
            pheromoneTexture.Apply();
            
            displayMaterial.mainTexture = pheromoneTexture;
        }).WithAll<PheromoneMap>().WithoutBurst().Run();
    }

}
