using System.Xml.Serialization;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public struct CatPositions : IJobEntity
{
    [ReadSingleton] public GameConfig gameConfig;
    [OutPersist] public NativeList<float2> catPositions;

    public void Init()
    {
        catPositions = new NativeList<float2>(gameConfig.NumOfCats, Allocator.Persistent);
    }

    [Include(typeof(Cat))]
    public void Execute(in Translation translation)
    {
        catPositions.AddNoResize(new float2(translation.Value.x, translation.Value.z));
    }
}

public struct CatCollision : IJobEntity
{
    public NativeList<float2> catPositions;

    [Include(typeof(Mouse))]
    public void Execute(in Translation translation, in Direction direction)
    {
        float2 mousePosition = new float2(translation.Value.x, translation.Value.z);
        foreach (var catPosition in catPositions)
        {
            if (math.distancesq(mousePosition, catPosition) < 0.4f)
            {
                // Cat collides with mouse
                Destroy();
            }
        }
    }
}

