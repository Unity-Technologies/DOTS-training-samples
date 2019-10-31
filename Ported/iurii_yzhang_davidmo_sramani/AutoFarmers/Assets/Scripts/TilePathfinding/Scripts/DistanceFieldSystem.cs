using GameAI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Pathfinding
{
    public class DistanceField
    {
        private int2 worldSize;
        
        private EntityQuery plantQuery;
        private EntityQuery stoneQuery;
        
        private JobHandle plantJob;
        private bool plantExpecting = false;
        private bool plantFirstActive;
        NativeArray<int> plantDistField1;
        NativeArray<int> plantDistField2;

        public NativeArray<int> PlantDistFieldRead
        {
            get
            {
                PlantSwapIfNeeded();
                return plantFirstActive ? plantDistField1 : plantDistField2;
            }
        }

        private NativeArray<int> PlantDistFieldWrite
        {
            get
            {
                PlantSwapIfNeeded();
                return plantFirstActive ? plantDistField2 : plantDistField1;
            }
        }
        
        //[BurstCompile]
        public struct MarkFieldJob : IJob
        {
            public NativeArray<PlantPositionRequest> plants;
            public NativeArray<int>   result;
            public NativeQueue<int2>   queue;
            public NativeQueue<int2>   queue2;
            public NativeHashMap<int2, byte>   visited;
            public int2 worldSize;
            public void Execute()
            {
                var n = result.Length;
                var pn = plants.Length;

                for (var i = 0; i < n; i++)
                    result[i] = int.MaxValue;

                var useFirstQueue = true;

                for (var p = 0; p < pn; p++)
                {
                    var pos = plants[p].position;
                    if (visited.ContainsKey(pos) == false)
                    {
                        queue.Enqueue(pos);
                        visited.Add(pos, 1);
                    }
                }
                
                var qR = useFirstQueue ? queue : queue2;
                var qW = useFirstQueue ? queue2 : queue;

                int V = 0;
                for (;;)
                {
                    if (queue.Count == 0 && queue2.Count == 0)
                        break;

                    for (;;)
                    {
                        if (qR.Count == 0)
                            break;
                        
                        var p = qR.Dequeue();
                        var indx = p.y * worldSize.x + p.x;

                        result[indx] = V;

                        var p1 = new int2(p.x - 1, p.y);
                        var p2 = new int2(p.x + 1, p.y);
                        var p3 = new int2(p.x, p.y - 1);
                        var p4 = new int2(p.x, p.y + 1);

                        if (p.x > 0 && visited.ContainsKey(p1) == false)
                        {
                            qW.Enqueue(p1);
                            visited.Add(p1, 1);
                        }

                        if (p.x < worldSize.x - 1 && visited.ContainsKey(p2) == false)
                        {
                            qW.Enqueue(p2);
                            visited.Add(p2, 1);
                        }

                        if (p.y > 0 && visited.ContainsKey(p3) == false)
                        {
                            qW.Enqueue(p3);
                            visited.Add(p3, 1);
                        }

                        if (p.y < worldSize.y - 1 && visited.ContainsKey(p4) == false)
                        {
                            qW.Enqueue(p4);
                            visited.Add(p4, 1);
                        }
                    }

                    ++V;
                    useFirstQueue = !useFirstQueue;
                    
                    qR = useFirstQueue ? queue : queue2;
                    qW = useFirstQueue ? queue2 : queue;
                }
            }
        }

        public DistanceField(int2 size, EntityQuery plantQuery, EntityQuery stoneQuery)
        {
            worldSize = size;

            this.plantQuery = plantQuery;
            this.stoneQuery = stoneQuery;
                
            plantDistField1 = new NativeArray<int>(worldSize.x * worldSize.y, Allocator.Persistent);
            plantDistField2 = new NativeArray<int>(worldSize.x * worldSize.y, Allocator.Persistent);
        }
        
        ~DistanceField()
        {
            plantDistField1.Dispose();
            plantDistField2.Dispose();
        }
        
        public JobHandle SchedulePlantField()
        {
            if (plantJob.IsCompleted)
            {
                var writePlant = PlantDistFieldWrite; // before plantExpecting = true
                
                var allPlants = plantQuery.ToComponentDataArray<PlantPositionRequest>(Allocator.Persistent);
                
                var q1 = new NativeQueue<int2>(Allocator.Persistent);
                var q2 = new NativeQueue<int2>(Allocator.Persistent);
                var visited = new NativeHashMap<int2, byte>(worldSize.x * worldSize.y, Allocator.Persistent);

                using (var allStones = stoneQuery.ToComponentDataArray<StonePositionRequest>(Allocator.TempJob))
                {
                    foreach (var stone in allStones)
                    {
                        for (int x = stone.position.x; x < stone.position.x + stone.size.x; x++)
                            for (int y = stone.position.y; y < stone.position.y + stone.size.y; y++)
                                visited[new int2(x, y)] = 1;
                    }    
                }
                
                
                plantExpecting = true;

                plantJob = new MarkFieldJob
                {
                    plants = allPlants,
                    result = writePlant,
                    queue = q1,
                    queue2 = q2,
                    visited = visited,
                    worldSize = worldSize
                }.Schedule();

                plantJob = allPlants.Dispose(plantJob);
                plantJob = visited.Dispose(plantJob);
                plantJob = q1.Dispose(plantJob);
                plantJob = q2.Dispose(plantJob);
            }
            
            return plantJob;
        }

        private void PlantSwapIfNeeded()
        {
            // if we scheduled last time and now it is completed
            if (plantJob.IsCompleted && plantExpecting)
            {
                plantFirstActive = !plantFirstActive;
            }

            plantExpecting = false;
        }

        public int2 PathToPlant(int2 currentPosition, out bool reached)
        {
            var current = GetDistanceFieldValue(currentPosition);

            reached = current == 0;
            if (reached)
            {
                return currentPosition;
            }
            
            foreach (var dp in new[]
            {
                new int2(-1, 0), 
                new int2(1, 0),
                new int2(0, -1),
                new int2(0, 1)
            })
            {
                var newPos = dp + currentPosition;
                var nextVal = GetDistanceFieldValue(newPos);
                if (nextVal < current)
                {
                    reached = nextVal == 0;
                    return newPos;
                }
            }

            return currentPosition;
        }

        public int GetDistanceFieldValue(int2 p)
        {
            if (p.x >= 0 && p.x < worldSize.x)
            {
                if (p.y >= 0 && p.y < worldSize.y)
                {
                    return PlantDistFieldRead[p.y * worldSize.x + p.x];
                }
            }

            return int.MaxValue;
        }
    }
}