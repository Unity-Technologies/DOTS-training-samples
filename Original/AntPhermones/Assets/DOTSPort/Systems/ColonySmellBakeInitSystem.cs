using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct ColonySmellBakeInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromoneBufferElement>();
        state.RequireForUpdate<AntSpawner>();
        state.RequireForUpdate<ObstacleArcPrimitive>();
        state.RequireForUpdate<CollisionHashSet>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var colony = SystemAPI.GetSingletonEntity<AntSpawner>();
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        var pheromonesBuffer = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();
        var obstacleBuffer = SystemAPI.GetSingletonBuffer<ObstacleArcPrimitive>();
        var collisionHashSet = SystemAPI.GetSingleton<CollisionHashSet>().CollisionSet;

        var localTransform = state.EntityManager.GetComponentData<LocalTransform>(colony);
        int2 pos = int2.zero;
        pos.x = (int)localTransform.Position.x;
        pos.y = (int)localTransform.Position.y;
        
        
        DFSSmell(pos, 200, pheromonesBuffer, globalSettings.MapSizeX, globalSettings.MapSizeY, obstacleBuffer.AsNativeArray(), collisionHashSet);
    }
    
    
    static void DFSSmell(int2 startPosition, int distance, DynamicBuffer<PheromoneBufferElement> buffer, int mapSizeX, int mapSizeY, in NativeArray<ObstacleArcPrimitive> ObstaclePrimtitveBuffer, NativeHashSet<int2> collisionHashSet)
    {
        float heatStep = 1f / (float)distance;
        
        var startNode = new PositionHeat() { heat = 1, position = startPosition };

        HashSet<int> pheromonesSet = new();
        HashSet<int2> closedSet = new HashSet<int2>();
        Queue<PositionHeat> openQueue = new Queue<PositionHeat>();

        openQueue.Enqueue(startNode);

        int totalMapSize = mapSizeX * mapSizeY;
        
        int exitCount = 1000000;

        while (openQueue.Count > 0)
        {
            var pH = openQueue.Dequeue();
            
            if(closedSet.Contains(pH.position))
                continue;

            closedSet.Add(pH.position);
            
            if(pH.heat - heatStep <= 0)
                continue;

           int pixel = PheromonesSystem.PheromoneIndexClamp(pH.position.x, pH.position.y, mapSizeX, mapSizeY);

         //  if (pixel >= totalMapSize || pheromonesSet.Contains(pixel))
         //      continue;

         if (pixel > 0 && pixel < totalMapSize && !pheromonesSet.Contains(pixel))
         {
             float2 dir = (float2)(pH.position - startPosition);
             if (!collisionHashSet.Contains(pH.position) && !ObstacleSpawnerSystem.CalculateRayCollision(ObstaclePrimtitveBuffer, (float2)startPosition / (float2)mapSizeX, dir / (float2)mapSizeX, out _, out _))
             {
                 var value = buffer[pixel].Value;
                 value.z = pH.heat;
                 buffer[pixel] = value;
                 
                 pheromonesSet.Add(pixel);
             }
         }

         exitCount--;
         if (exitCount <= 0)
         {
             Debug.LogError($"Exit!!!!!");
             break;
         }
               
           
          // buffer[pixel] += pH.heat;

           for (int i = 0; i < 8; i++)
            {
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x - 1, pH.position.y - 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x - 1, pH.position.y) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x - 1, pH.position.y + 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x, pH.position.y + 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x + 1, pH.position.y + 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x + 1, pH.position.y) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x + 1, pH.position.y - 1) } );
                AddToQueue(openQueue, closedSet, new PositionHeat(){ heat = pH.heat - heatStep, position = new int2(pH.position.x, pH.position.y - 1) } );
            }
        }
    }
    
    static void AddToQueue(Queue<PositionHeat> openQueue, HashSet<int2> closedSet, PositionHeat ph)
    {
        if(closedSet.Contains(ph.position))
            return;
        
        openQueue.Enqueue(ph);
    }
    
    class PositionHeat
    {
        public float heat;
        public int2 position;
    }
}
