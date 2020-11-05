using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public static class MapUtil
{
	public static void SetTile(ref DynamicBuffer<MapCell> buffer, MapCell mapCell, int2 coordinates, int width)
	{
		buffer[coordinates.x + coordinates.y * width] = mapCell;
	}

	public static MapCell GetTile(ref DynamicBuffer<MapCell> buffer, int2 coordinates, int width)
	{
		return buffer[coordinates.x + coordinates.y * width];
	}

	public static bool IsVisited(MapCell mapCell)
	{
		return (mapCell.Value & (byte)WallBits.Visited) != 0;
	}

	public static void Visit(ref DynamicBuffer<MapCell> buffer, int2 coordinates, int width)
	{
		var mapCell = GetTile(ref buffer, coordinates, width);
		mapCell.Value |= (byte)WallBits.Visited;
		SetTile(ref buffer, mapCell, coordinates, width);
	}
	
	public static void ClearVisited(ref DynamicBuffer<MapCell> buffer, int2 coordinates, int width)
	{
		var mapCell = GetTile(ref buffer, coordinates, width);
		mapCell.Value &= (byte)~WallBits.Visited;
		SetTile(ref buffer, mapCell, coordinates, width);
	}

	public static void SetWall(ref DynamicBuffer<MapCell> buffer, WallBits wall, int2 coordinates, int width)
	{
		var mapCell = GetTile(ref buffer, coordinates, width);
		mapCell.Value |= (byte)wall;
		SetTile(ref buffer, mapCell, coordinates, width);
	}
	
	public static void ClearWall(ref DynamicBuffer<MapCell> buffer, WallBits wall, int2 coordinates, int width)
	{
		var mapCell = GetTile(ref buffer, coordinates, width);
		mapCell.Value &= (byte)~wall;
		SetTile(ref buffer, mapCell, coordinates, width);
	}
}

public class MazeGenerator : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    private static void spawnLeftWall(Entity prefab, int2 position, int2 halfSize, EntityCommandBuffer ecb)
    {
	    var spawnedTile = ecb.Instantiate(prefab);
	    ecb.SetComponent(spawnedTile, new Translation {Value = new float3(position.x-halfSize.x - 0.5f, 0,  position.y -halfSize.y)});
	    ecb.SetComponent(spawnedTile, new Rotation{Value = quaternion.Euler(0,math.PI/2, 0 )});
    }
    
    private static void spawnBottomWall(Entity prefab, int2 position, int2 halfSize, EntityCommandBuffer ecb)
    {
	    var spawnedTile = ecb.Instantiate(prefab);
	    ecb.SetComponent(spawnedTile, new Translation {Value = new float3(position.x - halfSize.x, 0,  position.y - halfSize.y - 0.5f)});
    }
    
    private static void spawnTopWall(Entity prefab, int2 position, int2 halfSize, EntityCommandBuffer ecb)
    {
	    var spawnedTile = ecb.Instantiate(prefab);
	    ecb.SetComponent(spawnedTile, new Translation {Value = new float3(position.x - halfSize.x, 0,  position.y-halfSize.y + 0.5f)});
    }
    
    private static void spawnRightWall(Entity prefab, int2 position, int2 halfSize, EntityCommandBuffer ecb)
    {
	    var spawnedTile = ecb.Instantiate(prefab);
	    ecb.SetComponent(spawnedTile, new Translation {Value = new float3(position.x-halfSize.x + 0.5f, 0,  position.y -halfSize.y)});
	    ecb.SetComponent(spawnedTile, new Rotation{Value = quaternion.Euler(0,math.PI/2, 0 )});
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var stack = new NativeList<int2>(Allocator.TempJob);
        var unvisitedNeighbors = new NativeList<int2>(Allocator.TempJob);

        var jobHandle = Entities.WithDisposeOnCompletion(stack).WithDisposeOnCompletion(unvisitedNeighbors).ForEach((Entity entity, ref DynamicBuffer<MapCell> tiles, ref Random random, in MazeSpawner mazeSpawner, in Spawner spawner) => {
            int2 halfSize = (int2)spawner.MazeSize / 2;
            
            // top and bottom walls
            for (var i = 0; i < spawner.MazeSize.x; i++)
            {
	            spawnTopWall(spawner.Prefab, new int2(i, 0), halfSize, ecb);
	            spawnBottomWall(spawner.Prefab, new int2(i, (int)spawner.MazeSize.y), halfSize, ecb);
            }
            
            // side walls
            for (var i = 0; i < spawner.MazeSize.y; i++)
            {
	            spawnRightWall(spawner.Prefab, new int2(0, i), halfSize, ecb);
	            spawnLeftWall(spawner.Prefab, new int2((int)spawner.MazeSize.x, i), halfSize, ecb);
            }
            
            //maze interior
            // generating maze with recursive backtracker algorithm - https://en.wikipedia.org/wiki/Maze_generation_algorithm
            int width = spawner.MazeSize.x;
            int length = spawner.MazeSize.y;
            var numTiles = width * length;
            int2 current = random.Value.NextInt2(new int2(0, width), new int2(0, length));
			
			MapCell temp = MapUtil.GetTile(ref tiles, current, width);
			temp.Value |= (byte)WallBits.Visited;
			tiles[current.x + current.y * width] = temp;
			int numVisited = 1;

			while (numVisited < numTiles) {
				// choose random adjacent unvisited tile
				unvisitedNeighbors.Clear();
				if (current.x > 0 && !MapUtil.IsVisited(
					MapUtil.GetTile(ref tiles, new int2(current.x - 1, current.y), width)))
					unvisitedNeighbors.Add(new int2(current.x - 1, current.y));
				if (current.y > 0 && !MapUtil.IsVisited(
					MapUtil.GetTile(ref tiles, new int2(current.x, current.y - 1), width)))
					unvisitedNeighbors.Add(new int2(current.x, current.y - 1));
				if (current.x < width - 1 && !MapUtil.IsVisited(
					MapUtil.GetTile(ref tiles, new int2(current.x + 1, current.y), width)))
					unvisitedNeighbors.Add(new int2(current.x + 1, current.y));
				if (current.y < length - 1 && !MapUtil.IsVisited(
					MapUtil.GetTile(ref tiles, new int2(current.x, current.y + 1), width)))
					unvisitedNeighbors.Add(new int2(current.x, current.y + 1));

				if (unvisitedNeighbors.Length > 0) {
					// visit neighbor
					int2 next = unvisitedNeighbors[random.Value.NextInt(0,unvisitedNeighbors.Length)];
					stack.Add(current);
					// remove wall between tiles
					if (next.x > current.x) {
						MapUtil.ClearWall(ref tiles, WallBits.Right, current, width );
						MapUtil.ClearWall(ref tiles, WallBits.Left, next, width );
					} else if (next.y > current.y) {
						MapUtil.ClearWall(ref tiles, WallBits.Top, current, width );
						MapUtil.ClearWall(ref tiles, WallBits.Bottom, next, width );
					} else if (next.x < current.x) {
						MapUtil.ClearWall(ref tiles, WallBits.Left, current, width );
						MapUtil.ClearWall(ref tiles, WallBits.Right, next, width );
					} else {
						MapUtil.ClearWall(ref tiles, WallBits.Bottom, current, width );
						MapUtil.ClearWall(ref tiles, WallBits.Top, next, width );
					}
					MapUtil.Visit(ref tiles, next, width);
					numVisited++;
					current = next;
				} else {
					// backtrack if no unvisited neighboring tiles
					if (stack.Length > 0)
					{
						current = stack[stack.Length - 1];
						stack.RemoveAtSwapBack(stack.Length - 1);;
					}
				}
			}

			//could probably parallelize this
			for (var i = 0; i < spawner.MazeSize.y; i++)
			for (var j = 0; j < spawner.MazeSize.x; j++)
			{
				var position = new int2(j,i);
				var mapCell = MapUtil.GetTile(ref tiles, position, width);
				if((mapCell.Value & (byte)WallBits.Bottom) != 0)
					spawnBottomWall(spawner.Prefab, position, halfSize, ecb);
				if((mapCell.Value & (byte)WallBits.Top) != 0)
					spawnTopWall(spawner.Prefab, position, halfSize, ecb);
				if((mapCell.Value & (byte)WallBits.Right) != 0)
					spawnRightWall(spawner.Prefab, position, halfSize, ecb);
				if((mapCell.Value & (byte)WallBits.Left) != 0)
					spawnLeftWall(spawner.Prefab, position, halfSize, ecb);
			}
			ecb.RemoveComponent<MazeSpawner>(entity);
			ecb.RemoveComponent<Spawner>(entity);
        }).Schedule(Dependency);
        
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        
        jobHandle.Complete();
    }
}
