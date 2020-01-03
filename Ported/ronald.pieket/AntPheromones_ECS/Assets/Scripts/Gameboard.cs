using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class Gameboard
{
  static Random MyRandom;

  static BlobAssetReference<ObstacleGrid> GameboardGridBuilder(in ConfigComponent config, in List<float2>[,] obstacleBuckets)
  {
    using (var builder = new BlobBuilder(Allocator.Temp))
    {
      ref var root = ref builder.ConstructRoot<ObstacleGrid>();
      var buckets = builder.Allocate(ref root.Buckets, config.BucketResolution * config.BucketResolution);

      for (int x = 0; x < config.BucketResolution; x++)
      {
        for (int y = 0; y < config.BucketResolution; y++)
        {
          var bucket = obstacleBuckets[x, y];
          var obstacles = builder.Allocate(ref buckets[x + y * config.BucketResolution].Positions, bucket.Count);
          for (int i = 0; i < bucket.Count; ++i)
          {
            obstacles[i] = bucket[i];
          }
        }
      }

      return builder.CreateBlobAssetReference<ObstacleGrid>(Allocator.Persistent);
    }
  }

  static float2[] GenerateObstacleArray(in ConfigComponent config)
  {
    var obstacleList = new List<float2>();
    for (int i = 1; i <= config.ObstacleRingCount; i++)
    {
      float ringRadius = (i / (config.ObstacleRingCount + 1f)) * .5f;
      float circumference = ringRadius * 2f * math.PI;
      int maxCount = (int)math.ceil(circumference / (2f * config.ObstacleRadius) * 2f);
      int offset = MyRandom.NextInt(maxCount);
      int holeCount = MyRandom.NextInt(2) + 1;
      for (int j = 0; j < maxCount; j++)
      {
        float t = (float)j / maxCount;
        if ((t * holeCount) % 1f < config.ObstaclesPerRing)
        {
          float angle = (j + offset) / (float)maxCount * (2f * math.PI);
          var position = new float2(.5f + math.cos(angle) * ringRadius,.5f + math.sin(angle) * ringRadius);
          obstacleList.Add(position);
        }
      }
    }

    return obstacleList.ToArray();
  }


  static List<float2>[,] GenerateObstacles(in ConfigComponent config, in float2[] obstacleArray)
  {
    List<float2>[,] obstacleBuckets = new List<float2>[config.BucketResolution, config.BucketResolution];

    for (int x = 0; x < config.BucketResolution; x++)
    {
      for (int y = 0; y < config.BucketResolution; y++)
      {
        obstacleBuckets[x,y] = new List<float2>();
      }
    }

    float radius = config.ObstacleRadius;
    for (int i = 0; i < obstacleArray.Length; i++)
    {
      float2 pos = obstacleArray[i];
      for (int x = (int)math.floor((pos.x - radius) * config.BucketResolution); x <= (int)math.floor((pos.x + radius) * config.BucketResolution); x++)
      {
        if (x < 0 || x >= config.BucketResolution)
        {
          continue;
        }
        for (int y = (int)math.floor((pos.y - radius) * config.BucketResolution); y <= (int)math.floor((pos.y + radius) * config.BucketResolution); y++)
        {
          if (y < 0 || y >= config.BucketResolution)
          {
            continue;
          }
          obstacleBuckets[x, y].Add(obstacleArray[i]);
        }
      }
    }

    return obstacleBuckets;
  }

  static public void Initialize()
  {
    MyRandom = new Random();
    MyRandom.InitState((uint)DateTime.UtcNow.Millisecond);
  }

  static public GameboardComponent CreateGameboardComponent(ConfigComponent config)
  {
    var obstacleArray = GenerateObstacleArray(config);
    var obstacleBuckets = GenerateObstacles(config, obstacleArray);
    var gameboardGrid= GameboardGridBuilder(config, obstacleBuckets);

    float resourceAngle = MyRandom.NextFloat() * 2f * math.PI;
    var colonyPosition = new float2(.5f, .5f);
    var resourcePosition = new float2(.5f + math.cos(resourceAngle) * .475f,.5f + math.sin(resourceAngle) * .475f);

    var gameboardComponent = new GameboardComponent()
    {
      ObstacleGrid = gameboardGrid,
      ResourcePosition = resourcePosition,
      ColonyPosition = colonyPosition
    };

    using (var builder = new BlobBuilder(Allocator.Temp))
    {
      var obstacleCount = obstacleArray.Length;
      ref var root = ref builder.ConstructRoot<ObstacleList>();
      var positions = builder.Allocate(ref root.Positions, obstacleCount);
      for (int i = 0; i < obstacleCount; ++i)
      {
        positions[i] = obstacleArray[i];
      }
      gameboardComponent.ObstacleList = builder.CreateBlobAssetReference<ObstacleList>(Allocator.Persistent);
    }
    return gameboardComponent;
  }
}
