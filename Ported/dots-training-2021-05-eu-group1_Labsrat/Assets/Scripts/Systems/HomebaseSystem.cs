using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct HomebaseData
{
    public Entity playerEntity;
    public float2 position;
}

struct InitHomebases : IJobEntityMain
{
    [Out] public NativeList<HomebaseData> homeBases;    

    // run before first Execute; if returns false, then job does nothing
    public bool Pre()
    {
        return homeBases.IsEmpty;
    }

    public void Execute(in Homebase homebase, in Translation translation)
    {
        homeBases.Add(new HomebaseData()
        {
            playerEntity = homebase.PlayerEntity,
            position = new float2(translation.Value.x, translation.Value.z)
        });
    }

    public void Teardown()
    {
        homeBases.Dispose();
    }
}

struct MouseReachesHome : IJobEntity
{
    [Read] public NativeList<HomebaseData> homeBases;
    public Lookup<Score> scoreData;

    [Include(typeof(Mouse))]
    public void Execute(in Translation translation, in Direction direction)
    {
        float2 mousePos = new float2(translation.Value.x, translation.Value.z);
        foreach (var homebase in homeBases)
        {
        if (Utils.SnapTest(mousePos, homebase.position, direction.Value)
            && math.distancesq(mousePos, homebase.position) < 1)
            {
                Score score = scoreData[homebase.playerEntity];
                score.Value += 1;
                scoreData[homebase.playerEntity] = score;
                
                Destroy();
            }
        }
    }
}

struct CatReachesHome : IJobEntity
{
    [Read] public NativeList<HomebaseData> homeBases;
    public Lookup<Score> scoreData;

    [Include(typeof(Cat))]
    public void Execute(in Translation translation, in Direction direction)
    {
        float2 catPos = new float2(translation.Value.x, translation.Value.z);
        foreach (var homebase in homeBases)
        {
            if (Utils.SnapTest(catPos, homebase.position, direction.Value)
            && math.distancesq(catPos, homebase.position) < 1)
            {
                Score score = scoreData[homebase.playerEntity];
                score.Value = math.max(0, score.Value-30);
                scoreData[homebase.playerEntity] = score;
                Destroy();
            }
        }
    }
}

