using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class DijkstraGeneratorSystem : SystemBase
{   
    const int kWidth  = 10;
    const int kHeight = 10;
 
    protected override void OnCreate()
    {
        var e = EntityManager.CreateEntity(typeof(DijkstraMap));
#if UNITY_EDITOR
        EntityManager.SetName(e, "DijkstraMap");
#endif
        EntityManager.AddBuffer<MapCell>(e);
        EntityManager.AddBuffer<DistCell>(e);
        EntityManager.SetComponentData(e, new DijkstraMap(kWidth, kHeight));
    }

    protected override void OnUpdate()
    {
        int2 position = new int2(0, 0);

        Entities.ForEach((Entity e, ref DijkstraMap map, ref DynamicBuffer<MapCell> wallMap, ref DynamicBuffer<DistCell> distMap) => 
        {
            var length = map.Width * map.Height;
            if (distMap.Length != length)
                distMap.ResizeUninitialized(length);

            var positions = new NativeList<int2>(length, Allocator.Temp);

            for (var i = 0; i < distMap.Length; ++i)
                distMap[i] = new DistCell(){ Value = int.MaxValue };

            var sourceIndex = position.y * map.Width + position.x;
            distMap[sourceIndex] = new DistCell(){ Value = 0 };
            positions.AddNoResize(position);

            while (positions.Length > 0)
            {
                var last = positions.Length - 1;
                var current = positions[last];
                positions.RemoveAt(last);

                var index = current.y * map.Width + current.x;
                var dist = 1 + distMap[index].Value;

                VisitNeighbor(current.x - 1, current.y - 1, dist, ref map, ref distMap, ref positions);
				VisitNeighbor(current.x - 1, current.y + 0, dist, ref map, ref distMap, ref positions);
				VisitNeighbor(current.x - 1, current.y + 1, dist, ref map, ref distMap, ref positions);
				VisitNeighbor(current.x,     current.y - 1, dist, ref map, ref distMap, ref positions);
				VisitNeighbor(current.x,     current.y + 1, dist, ref map, ref distMap, ref positions);
				VisitNeighbor(current.x + 1, current.y - 1, dist, ref map, ref distMap, ref positions);
				VisitNeighbor(current.x + 1, current.y + 0, dist, ref map, ref distMap, ref positions);
				VisitNeighbor(current.x + 1, current.y + 1, dist, ref map, ref distMap, ref positions);
            }
        })
        .Schedule();
    }

    static void VisitNeighbor(int x, int y, int distance, ref DijkstraMap map, ref DynamicBuffer<DistCell> distMap, ref NativeList<int2> positions)
    {
        if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
        {
            var index = y * map.Width + x;
            if (distMap[index].Value == int.MaxValue)
            {
                distMap[index] = new DistCell(){ Value = distance };
                positions.AddNoResize(new int2(x, y));
            }
        }
    }
}
