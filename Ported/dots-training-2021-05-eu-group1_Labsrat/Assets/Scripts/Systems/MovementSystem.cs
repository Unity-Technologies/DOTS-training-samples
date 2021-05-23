using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

struct MoveCats : IJobEntity
{
    [ReadSingleton] public Time time;
    [Singleton] public GameConfig gameconfig;

    public void Execute(ref Translation translation, in Cat cat, in Direction direction)
    {
        var offset = direction.GetDirection() * math.min(Constants.kMaxTravel, time.dt * gameConfig.CatSpeed);
        translation.Value.x += offset.x;
        translation.Value.z += offset.y;
    }
}

struct MoveMice : IJobEntity
{
    [ReadSingleton] public  Time time;
    [Singleton] public GameConfig gameconfig;
    
    public void Execute(ref Translation translation, in Mouse mouse, in Direction direction)
    {
        var offset = direction.GetDirection() * math.min(Constants.kMaxTravel, time.dt * gameConfig.MouseSpeed); 
        translation.Value.x += offset.x;
        translation.Value.z += offset.y;
    }
}