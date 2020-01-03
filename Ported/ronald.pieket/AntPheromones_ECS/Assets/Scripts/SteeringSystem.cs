using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class SteeringSystem : JobComponentSystem
{
  static public EntityQuery PheromoneQuery;
  private Random Random;

  static float ReadPheromones(in NativeArray<float> pheromones, int mapSize, float x, float y)
  {
    int ix = (int)math.floor( x * mapSize);
    int iy = (int)math.floor( y * mapSize);
    if (ix >= 0 && ix < mapSize && iy >= 0 && iy < mapSize)
    {
      return pheromones[ix + iy * mapSize];
    }
    else
    {
      return 0f;
    }
  }

  static float PheromoneSteering(in ConfigComponent config, in NativeArray<float> pheromones, float2 position, float heading, float distance)
  {
    float output = 0;

    for (int i = -1; i <= 1; i += 2)
    {
      float angle = heading + i * math.PI * .25f;
      float testX = position.x + math.cos(angle) * distance;
      float testY = position.y + math.sin(angle) * distance;
      output += ReadPheromones(pheromones, config.MapSize, testX, testY) * i;
    }
    return output;
  }

  static int BucketIndexForPos(in ConfigComponent config, float posX, float posY)
  {
    if (posX >= 0f && posY >= 0f && posX < 1f && posY < 1f)
    {
      int x = (int)(posX * config.BucketResolution);
      int y = (int)(posY * config.BucketResolution);
      return x + y * config.BucketResolution;
    }
    else
    {
      return -1;
    }
  }

  static int WallSteering(in ConfigComponent config, in GameboardComponent gameboard, float2 position, float heading, float distance)
  {
    int output = 0;
    ref var buckets = ref gameboard.ObstacleGrid.Value.Buckets;

    for (int i = -1; i <= 1; i+=2)
    {
      float angle = heading + i * math.PI*.25f;
      float testX = position.x + math.cos(angle) * distance;
      float testY = position.y + math.sin(angle) * distance;

      int index = BucketIndexForPos(config, testX, testY);
      if (index >= 0 && buckets[index].Positions.Length > 0)
      {
        output -= i;
      }
    }
    return output;
  }

  static public bool Linecast(in ConfigComponent config, in GameboardComponent gameboard, float2 point1, float2 point2)
  {
    float dx = point2.x - point1.x;
    float dy = point2.y - point1.y;
    float dist = math.sqrt(dx * dx + dy * dy);

    int stepCount = (int)math.ceil(dist * config.BucketResolution);
    float oneOverStepCount = 1f / stepCount;
    ref var buckets = ref gameboard.ObstacleGrid.Value.Buckets;
    for (int i = 0; i < stepCount; i++)
    {
      float t = i * oneOverStepCount;
      float testX = point1.x + dx * t;
      float testY = point1.y + dy * t;
      int index = BucketIndexForPos(config, testX, testY);
      if (buckets[index].Positions.Length > 0)
      {
        return true;
      }
    }
    return false;
  }

  protected override void OnCreate()
  {
    base.OnCreate();

    Random = new Random();
    Random.InitState((uint)DateTime.UtcNow.Millisecond);

    PheromoneQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneElement>());
  }

  [BurstCompile]
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    inputDependencies.Complete();

    Random random = new Random();
    random.InitState(Random.NextUInt());

    var config = GetSingleton<ConfigComponent>();
    var gameboard = GetSingleton<GameboardComponent>();

    var pheromonesFromEntity = GetBufferFromEntity<PheromoneElement>(true);
    var pheromonesEntity = PheromoneQuery.GetSingletonEntity();

    float2 colonyPosition = gameboard.ColonyPosition;

    Entities
      .WithAll<AntTag>()
      .WithReadOnly(pheromonesFromEntity)
      .ForEach((ref HeadingComponent heading, ref Translation translation, in SpeedComponent speed, in TargetComponent target, in ResourceComponent resource) =>
      {
        var pheromones = pheromonesFromEntity[pheromonesEntity].Reinterpret<float>().AsNativeArray();

        float2 pos = new float2(translation.Value.x, translation.Value.y);
        float facingAngle = heading.Value;

        // Random steering
        facingAngle += config.RandomSteerStrength * (random.NextFloat() * 2f - 1f);
        facingAngle += config.WallSteerStrength * WallSteering(config, gameboard, pos,facingAngle, 1.5f / 128);
        facingAngle += config.PheromoneSteerStrength * PheromoneSteering(config, pheromones, pos, facingAngle, 3f / 128);

        // If target can be seen, turn towards it
        if (!Linecast(config, gameboard, pos, target.Value))
        {
          float targetAngle = math.atan2(target.Value.y - pos.y,target.Value.x - pos.x);
          if (targetAngle - facingAngle > math.PI)
          {
            facingAngle += math.PI * 2f;
          }
          else if (targetAngle - facingAngle < -math.PI)
          {
            facingAngle -= math.PI * 2f;
          }
          else
          {
            if (math.abs(targetAngle - facingAngle) < math.PI * .5f)
            {
              facingAngle += (targetAngle - facingAngle ) * config.GoalSteerStrength;
            }
          }
        }

        // Set up for next couple of checks
        float vx = math.cos(facingAngle) * speed.Value;
        float vy = math.sin(facingAngle) * speed.Value;
        float ovx = vx;
        float ovy = vy;

        // Turn around at the edge of the board
        float testX = pos.x + vx;
        float testY = pos.y + vy;
        if ((vx < 0f && testX < 0f) || (vx > 0f && testX >= 1f))
        {
          vx = -vx;
        }
        if ((vy < 0f && testY < 0f) || (vy > 0f && testY >= 1f))
        {
          vy = -vy;
        }

        // Avoid obstacles
        var index = BucketIndexForPos(config, pos.x, pos.y);
        if (index >= 0)
        {
          var obstacleRadius = 2f / 128;
          ref var buckets = ref gameboard.ObstacleGrid.Value.Buckets;
          ref var obstaclePositions = ref buckets[index].Positions;
          for (int i = 0; i < obstaclePositions.Length; ++i)
          {
            var obstaclePosition = obstaclePositions[i];
            float dx = pos.x - obstaclePosition.x;
            float dy = pos.y - obstaclePosition.y;
            float sqrDist = dx * dx + dy * dy;
            if (sqrDist < obstacleRadius * obstacleRadius)
            {
              float rdist = math.rsqrt(sqrDist);
              dx *= rdist;
              dy *= rdist;
              pos.x = obstaclePosition.x + dx * obstacleRadius;
              pos.y = obstaclePosition.y + dy * obstacleRadius;

              vx -= dx * (dx * vx + dy * vy) * 1.5f;
              vy -= dy * (dx * vx + dy * vy) * 1.5f;
            }
          }
        }

        // Inward/outward pressure
        {
          float inwardOrOutward = -config.OutwardStrength;
          float pushRadius = .4f;
          if (resource.HasResource)
          {
            inwardOrOutward = config.InwardStrength;
            pushRadius = 1f;
          }

          float dx = colonyPosition.x - pos.x;
          float dy = colonyPosition.y - pos.y;
          float dist = math.sqrt(dx * dx + dy * dy);
          inwardOrOutward *= 1f - math.clamp(dist / pushRadius, 0f, 1f);
          vx += dx / dist * inwardOrOutward;
          vy += dy / dist * inwardOrOutward;
        }

        if (ovx != vx || ovy != vy)
        {
          facingAngle = math.atan2(vy,vx);
        }

        heading.Value = facingAngle;
        translation.Value.x = pos.x;
        translation.Value.y = pos.y;
      })
      .Run();

    return default;
  }
}
