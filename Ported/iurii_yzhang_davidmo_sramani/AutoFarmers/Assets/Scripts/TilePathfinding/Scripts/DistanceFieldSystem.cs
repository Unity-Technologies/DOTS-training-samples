using GameAI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Pathfinding
{
    public class DistanceField
    {
        public enum FieldType
        {
            Plant,
            Stone,
            Shop
        }

        private FieldType fieldType;
        
        private int2 worldSize;
        
        private EntityQuery plantQuery;
        private EntityQuery stoneQuery;
        private EntityQuery shopQuery;
        
        private JobHandle job;
        private bool expecting = false;
        private bool firstActive;
        NativeArray<int> distField1;
        NativeArray<int> distField2;

        public NativeArray<int> DistFieldRead
        {
            get
            {
                SwapIfNeeded();
                return firstActive ? distField1 : distField2;
            }
        }

        private NativeArray<int> DistFieldWrite
        {
            get
            {
                SwapIfNeeded();
                return firstActive ? distField2 : distField1;
            }
        }
        
        [BurstCompile]
        public struct MarkFieldJob : IJob
        {
            public NativeList<int2> target;
            public NativeArray<int>    result;
            public NativeQueue<int2>   queue;
            public NativeQueue<int2>   queue2;
            public NativeHashMap<int2, byte>   visited;
            public int2 worldSize;
            public void Execute()
            {
                var n = result.Length;
                var pn = target.Length;

                for (var i = 0; i < n; i++)
                    result[i] = int.MaxValue;

                var useFirstQueue = true;

                for (var p = 0; p < pn; p++)
                {
                    var pos = target[p];
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

        public DistanceField(FieldType fieldType, int2 size, EntityQuery plantQuery, EntityQuery stoneQuery, EntityQuery shopQuery)
        {
            this.fieldType = fieldType;
            
            worldSize = size;

            this.shopQuery = shopQuery;
            this.plantQuery = plantQuery;
            this.stoneQuery = stoneQuery;
                
            distField1 = new NativeArray<int>(worldSize.x * worldSize.y, Allocator.Persistent);
            distField2 = new NativeArray<int>(worldSize.x * worldSize.y, Allocator.Persistent);
        }

        public void Dispose()
        {
            job.Complete();
            distField1.Dispose();
            distField2.Dispose();
        }
        
        public JobHandle Schedule()
        {
            if (job.IsCompleted)
            {
                var writePlant = DistFieldWrite; // before plantExpecting = true

                bool stonesAreObstacles = fieldType == FieldType.Plant || fieldType == FieldType.Shop;
                
                var targets = new NativeList<int2>(Allocator.Persistent);
                var q1 = new NativeQueue<int2>(Allocator.Persistent);
                var q2 = new NativeQueue<int2>(Allocator.Persistent);
                var visited = new NativeHashMap<int2, byte>(worldSize.x * worldSize.y, Allocator.Persistent);

                if (fieldType == FieldType.Plant)
                {
                    using (var allPlants = plantQuery.ToComponentDataArray<TilePositionable>(Allocator.TempJob))
                    {
                        var n = allPlants.Length;
                        Debug.Log($"Distance field scheduled with plants = {n}");
                        for (int i = 0; i < n; i++)
                            targets.Add(allPlants[i].Position);
                    }
                }
                else if (fieldType == FieldType.Stone)
                {
                    using (var allStones = stoneQuery.ToComponentDataArray<TilePositionable>(Allocator.TempJob))
                    using (var allSizes = stoneQuery.ToComponentDataArray<RockComponent>(Allocator.TempJob))
                    {
                        var n = allStones.Length;

                        Assert.AreEqual(n, allSizes.Length);

                        Debug.Log($"Distance field scheduled with stones = {n}");

                        // border
                        for (int i = 0; i < n; i++)
                        {
                            var stone = allStones[i];
                            var size = allSizes[i];
                            var rigX = stone.Position.x + size.Size.x - 1;
                            var lefX = stone.Position.x;
                            var topY = stone.Position.y + size.Size.y - 1;
                            var botY = stone.Position.y;
                            for (int x = stone.Position.x; x < stone.Position.x + size.Size.x; x++)
                            {
                                targets.Add(new int2(x, topY));
                                targets.Add(new int2(x, botY));
                            }
                            
                            for (int y = stone.Position.y + 1; y < stone.Position.y + size.Size.y - 1; y++)
                            {
                                targets.Add(new int2(lefX, y));
                                targets.Add(new int2(rigX, y));
                            }
                        }
                    }
                }
                else if (fieldType == FieldType.Shop)
                {
                    using (var allShops = shopQuery.ToComponentDataArray<TilePositionable>(Allocator.TempJob))
                    {
                        var n = allShops.Length;

                        Debug.Log($"Distance field scheduled with shops = {n}");

                        for (int i = 0; i < n; i++)
                            targets.Add(allShops[i].Position);
                    }
                }

                if (stonesAreObstacles)
                {
                    using (var allStones = stoneQuery.ToComponentDataArray<TilePositionable>(Allocator.TempJob))
                    using (var allSizes = stoneQuery.ToComponentDataArray<RockComponent>(Allocator.TempJob))
                    {
                        var n = allStones.Length;
                        for(int i=0; i<n; i++)
                        {
                            var stone = allStones[i];
                            var size = allSizes[i];
                            for (int x = stone.Position.x; x < stone.Position.x + size.Size.x; x++)
                            for (int y = stone.Position.y; y < stone.Position.y + size.Size.y; y++)
                                visited[new int2(x, y)] = 1;
                        }
                    }
                }

                expecting = true;

                job = new MarkFieldJob
                {
                    target = targets,
                    result = writePlant,
                    queue = q1,
                    queue2 = q2,
                    visited = visited,
                    worldSize = worldSize
                }.Schedule();

                job = targets.Dispose(job);
                job = visited.Dispose(job);
                job = q1.Dispose(job);
                job = q2.Dispose(job);
            }
            
            return job;
        }

        private void SwapIfNeeded()
        {
            // if we scheduled last time and now it is completed
            if (job.IsCompleted && expecting)
            {
                firstActive = !firstActive;
            }

            expecting = false;
        }

        public static int2 PathTo(int2 currentPosition, int2 worldSize, NativeArray<int> distFieldRead, out bool reached)
        {
            var current = GetDistanceFieldValue(currentPosition, worldSize, distFieldRead);

            reached = current == 0;
            if (reached)
            {
                return currentPosition;
            }
            
//            foreach (var dp in new[]
//            {
//                new int2(-1, 0), 
//                new int2(1, 0),
//                new int2(0, -1),
//                new int2(0, 1)
//            })
//            {
//                var newPos = dp + currentPosition;
//                var nextVal = GetDistanceFieldValue(newPos, worldSize, distFieldRead);
//                if (nextVal < current)
//                {
//                    reached = nextVal == 0;
//                    return newPos;
//                }
//            }
            
            // burst...
            {
                var newPos = new int2(-1, 0) + currentPosition;
                var nextVal = GetDistanceFieldValue(newPos, worldSize, distFieldRead);
                if (nextVal < current) {
                    reached = nextVal == 0;
                    return newPos;
                }
            }
            {
                var newPos = new int2(1, 0) + currentPosition;
                var nextVal = GetDistanceFieldValue(newPos, worldSize, distFieldRead);
                if (nextVal < current) {
                    reached = nextVal == 0;
                    return newPos;
                }
            }
            {
                var newPos = new int2(0, 1) + currentPosition;
                var nextVal = GetDistanceFieldValue(newPos, worldSize, distFieldRead);
                if (nextVal < current) {
                    reached = nextVal == 0;
                    return newPos;
                }
            }
            {
                var newPos = new int2(0, -1) + currentPosition;
                var nextVal = GetDistanceFieldValue(newPos, worldSize, distFieldRead);
                if (nextVal < current) {
                    reached = nextVal == 0;
                    return newPos;
                }
            }
            
            return currentPosition;
        }

        public static int GetDistanceFieldValue(int2 p, int2 worldSize, NativeArray<int> distFieldRead)
        {
            if (p.x >= 0 && p.x < worldSize.x)
            {
                if (p.y >= 0 && p.y < worldSize.y)
                {
                    return distFieldRead[p.y * worldSize.x + p.x];
                }
            }

            return int.MaxValue;
        }

        public int2 PathToPlant(int2 currentPosition, out bool reached) { return PathTo(currentPosition, worldSize, DistFieldRead, out reached); }

        public int GetDistanceFieldValue(int2 p) { return GetDistanceFieldValue(p, worldSize, DistFieldRead); }
        public void Complete() { job.Complete(); }

        public static void DebugLogAround(int2 p, int2 worldSize, NativeArray<int> distanceField)
        {
            var s = "";

            for (int x = p.x - 3; x <= p.x + 3; x++)
            {
                for (int y = p.y - 3; y <= p.y + 3; y++)
                {
                    var val = distanceField[y * worldSize.x + x];
                    var valStr = val == int.MaxValue ? "X" : val.ToString();

                    s += valStr + " ";
                }
                s += '\n';
            }
            Debug.Log(s);
        }
    }
}