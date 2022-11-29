using UnityEngine;
using Unity.Entities;


partial class PheromoneDisplaySystem : SystemBase
{
    public GameObject displayPlane;
    private Material displayMaterial;
    private Texture2D pheromoneTexture;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        displayPlane = GameObject.Find("DisplayPlane");
        
        pheromoneTexture = new Texture2D(64, 64);   // TEMP: Hardcoded size
        
        if (displayPlane.TryGetComponent<Renderer>(out var displayRenderer))
            displayMaterial = displayRenderer.material;

        // TEMP: Dummy Add a single entity.
        var e = EntityManager.CreateEntity();
        EntityManager.AddComponent<PheromoneMap>(e);
    }

    protected override void OnUpdate()
    {
        // Assume there's only one PheromoneMap
        Entities.ForEach((Entity ent, in DynamicBuffer<PheromoneMap> map) =>
        {
            //DynamicBuffer<PheromoneMap> mapBuffer = EntityManager.GetBuffer<PheromoneMap>(ent);
            // TODO...
            
            Debug.Log("Foo");
            
            // Clear
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    pheromoneTexture.SetPixel(i, j, Color.black);
                }    
            }
            
            // Random fill contents
            for (int i = 0; i < 64; i++)
            {
                float x = Random.value;
                float y = Random.value;
            
                pheromoneTexture.SetPixel((int)(x*63), (int)(y*63), Color.red);
            }
            pheromoneTexture.Apply();
            
            displayMaterial.mainTexture = pheromoneTexture;
        }).WithAll<PheromoneMap>().WithoutBurst().Run();
    }

}
