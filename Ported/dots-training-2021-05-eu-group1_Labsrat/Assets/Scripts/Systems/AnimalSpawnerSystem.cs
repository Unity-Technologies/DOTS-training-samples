using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public struct SpawnCat : IJobEntity
{
    [ReadSingleton] public GameConfig gameConfig;
    [ReadSingleton] public Time time;
    
    // we can get the single field of a singleton directly instead of the struct
    [ReadSingleton(typeof(TimeUntilNextCatSpawn))] public float timeUntilNextCatSpawn;
    
    // when is the query performed? at schedule time? Do we actually want results of a query insteadof the query itself?
    public EntityQuery catQuery = new EntityQuery(ComponentType.ReadOnly<Cat>());  
    
    public void Execute()
    {
        if (timeUntilNextCatSpawn > 0)
        {
            timeUntilNextCatSpawn -= time.dt;
            return;
        }

        var catCount = catQuery.CalculateEntityCountWithoutFiltering(); 
        if (catCount >= gameConfig.NumOfCats)
        {
            return;
        }
        
        timeUntilNextCatSpawn = gameConfig.CatSpawnDelay;
            
        Random random = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)time.timeNow : gameConfig.Seed ^ 2984576396);
        for (int i = 0; i < gameConfig.MaxAnimalsSpawnedPerFrame; i++)
        {
            if (catcount++ >= gameConfig.NumOfCats)
                return;
                                
            var xPos = random.NextInt(gameConfig.BoardDimensions.x);
            var yPos = random.NextInt(gameConfig.BoardDimensions.y);
            var randDir = random.NextInt(3);
            var rotation = Unity.Mathematics.quaternion.RotateY(Mathf.PI * randDir / 2);

            Entity cat = Instantiate(gameConfig.CatPrefab);
            Add<Cat>(cat);
            Set(cat, new Translation() { Value = new float3(xPos, 0, yPos) });
            Set(cat, new Rotation() { Value = rotation });
            Add<Direction>(cat);
            Set(cat, new Direction() { Value = Direction.FromRandomDirection(randDir) });
        }
    } 
}

public struct SpawnMouse : IJobEntity
{
    [ReadSingleton] public GameConfig gameConfig;
    [ReadSingleton] public Time time;
    
    [ReadSingleton(typeof(TimeUntilNextMouseSpawn))] public float timeUntilNextMouseSpawn;
    
    public EntityQuery mouseQuery = new EntityQuery(ComponentType.ReadOnly<Mouse>());
    public void Execute()
    {
        if (timeUntilNextMouseSpawn > 0)
        {
            timeUntilNextMouseSpawn -= time.dt;
            return;
        }

        var mouseCount = mouseQuery.CalculateEntityCountWithoutFiltering(); 
        if (mouseCount >= gameConfig.NumOfCats)
        {
            return;
        }
        
        timeUntilNextMouseSpawn = gameConfig.CatSpawnDelay;
            
        Random random = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)timeNow+1 : gameConfig.Seed ^ 1236534);
        bool mouseflipflop = true;
        quaternion angleEast = quaternion.RotateY(Direction.GetAngle(Cardinals.East));
        quaternion angleWest = quaternion.RotateY(Direction.GetAngle(Cardinals.West));
        for (int i = 0; i < gameConfig.MaxAnimalsSpawnedPerFrame; i++)
        {
            if (mousecount++ >= gameConfig.NumOfMice)
                return;
            
            int xPos = 0;
            int yPos = 0;
            Cardinals dir = Cardinals.East;
            quaternion rotation = angleEast;
            if (mouseflipflop)
            {
                xPos = gameConfig.BoardDimensions.x - 1;
                yPos = gameConfig.BoardDimensions.y - 1;
                dir = Cardinals.West;
                rotation = angleWest;
            }

            if (gameConfig.MiceSpawnInRandomLocations)
            {
                xPos = random.NextInt(gameConfig.BoardDimensions.x);
                yPos = random.NextInt(gameConfig.BoardDimensions.y);
            }

            Entity mouse = Instantiate(gameConfig.MousePrefab);
            Set(mouse, new Translation() { Value = new float3(xPos, 0, yPos) });
            Set(mouse, new Rotation() { Value = rotation });
            Set(mouse, new Direction() { Value = dir });
            mouseflipflop = !mouseflipflop;
        }
    } 
}
