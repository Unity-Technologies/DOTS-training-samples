using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class FarmerSystem : SystemBase
{
    readonly static int[] dirsX = new int[] { 1,-1,0,0 };
	readonly static int[] dirsY = new int[] { 0,0,1,-1 };

    [BurstCompile]
    struct FindTarget : IJobChunk
    {
        public Unity.Mathematics.Random random;

        public FarmConfig farmConfig;
        public FarmerConfig farmerConfig;
        
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<byte> groundTiles;
        
        public BufferTypeHandle<Path> pathTypeHandle;
        public ComponentTypeHandle<Farmer> farmerTypeHandle;
        public ComponentTypeHandle<Translation> translationTypeHandle;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        { 
            BufferAccessor<Path> paths = chunk.GetBufferAccessor(pathTypeHandle);
            NativeArray<Farmer> farmers = chunk.GetNativeArray(farmerTypeHandle);
            NativeArray<Translation> translations = chunk.GetNativeArray(translationTypeHandle); 

            var visitedTiles = new NativeArray<int>(farmConfig.MapSizeX * farmConfig.MapSizeY, Allocator.Temp);
            var activeTiles = new NativeList<int>(Allocator.Temp);
            var nextTiles = new NativeList<int>(Allocator.Temp);

            int numTiles = farmConfig.MapSizeX * farmConfig.MapSizeY;
            for (int i = 0; i < chunk.Count; i++)
            {
                DynamicBuffer<Path> path = paths[i];
                if (path.Length > 0)
                    continue;

                Farmer farmer = farmers[i];
                Translation translation = translations[i];
                int posX = (int)translation.Value.x;
                int posY = (int)translation.Value.z;
                for (int j = 0; j < numTiles; j++)
                {
                    visitedTiles[j] = -1;
                }
                activeTiles.Clear();
                nextTiles.Clear();

                //float4 tillableZone = GenerateTillableZone(posX, posY, random, farmConfig);
                //if (IsTillableZoneBlocked(tillableZone, groundTiles, farmConfig.MapSizeX))
                //    continue;    
                //farmer.TillableZone = tillableZone;

                float2 location = Locate(posX, posY, visitedTiles, activeTiles, nextTiles);
                
                if (!location.Equals(new float2(posX, posY)))
                {
                    ConstructPath(path, (int)location.x, (int)location.y, visitedTiles);
                }

                farmers[i] = farmer;
            }

            visitedTiles.Dispose();
            activeTiles.Dispose();
            nextTiles.Dispose();
        }

        private float4 GenerateTillableZone(int x, int y, Unity.Mathematics.Random random, FarmConfig config)
        {
			int width = random.NextInt(1,8);
			int height = random.NextInt(1,8);
			int minX = x + random.NextInt(-10,10 - width);
			int minY = y + random.NextInt(-10,10 - height);
			if (minX < 0) minX = 0;
			if (minY < 0) minY = 0;
			if (minX + width >= config.MapSizeX) minX = config.MapSizeX - 1 - width;
			if (minY + height >= config.MapSizeY) minY = config.MapSizeY - 1 - height;
            return new float4(minX, minX + width, minY, minY + height);
        }

        private bool IsTillableZoneBlocked(float4 zone, NativeArray<byte> groundTiles, int mapWidth)
        {
            for (int x = (int)zone.w; x <= (int)zone.x; x++) 
            {
				for (int y = (int)zone.y; y <= (int)zone.z; y++)
                {
                    int hash = PathUtility.Hash(x, y, mapWidth);
                    if (groundTiles[hash] == byte.MaxValue)
                        return true;
                }
            }
            return false;				
        }

        private float2 Locate(int startX, int startY, NativeArray<int> visitedTiles, NativeList<int> activeTiles, NativeList<int> nextTiles)
        {
            int steps = 0;
            int x = startX;
            int y = startY;

            int hash = PathUtility.Hash(x, y, farmConfig.MapSizeX);
		    visitedTiles[hash] = 0;
            nextTiles.Add(hash);

            while (nextTiles.Length > 0 && steps < farmerConfig.FarmerRange)
            {
                NativeList<int> temp = activeTiles;
			    activeTiles = nextTiles;
			    nextTiles = temp;
			    nextTiles.Clear();

                steps++;
			    for (int i = 0; i < activeTiles.Length; i++)
                { 
				    PathUtility.Unhash(activeTiles[i], farmConfig.MapSizeX, farmConfig.MapSizeY, out x, out y);
                    
                    for (int j = 0; j < dirsX.Length; j++)
                    {
                        int x2 = x + dirsX[j];
                        int y2 = y + dirsY[j];

                        if (x2 < 0 || y2 < 0 || x2 >= farmConfig.MapSizeX || y2 >= farmConfig.MapSizeY)
                            continue;
                        
                        hash = PathUtility.Hash(x2, y2, farmConfig.MapSizeX);
                        if (visitedTiles[hash] == -1 || visitedTiles[hash] > steps)
                        {
                            visitedTiles[hash] = steps;
                            nextTiles.Add(hash);
                            if (groundTiles[hash] == 0)
                            {
                                return new float2(x2, y2);
                            }
                        }
                    }
                }
            }
            return new float2(startX, startY);
        }

        private void ConstructPath(DynamicBuffer<Path> path, int endX, int endY, NativeArray<int> visitedTiles)
        {
            int x = endX;
            int y = endY;
            path.Add(new Path { Position = new float2(x, y) });

            int dist = int.MaxValue;
            while (dist > 0)
            {
                int minNeighborDist = int.MaxValue;
                int bestNewX = x;
                int bestNewY = y;
                for (int i = 0; i < dirsX.Length; i++)
                {
                    int x2 = x + dirsX[i];
                    int y2 = y + dirsY[i];
                    if (x2 < 0 || y2 < 0 || x2 >= farmConfig.MapSizeX || y2 >= farmConfig.MapSizeY)
                        continue;

                    int hash = PathUtility.Hash(x2, y2, farmConfig.MapSizeX);
                    int newDist = visitedTiles[hash];
                    if (newDist != -1 && newDist < minNeighborDist)
                    {
                        minNeighborDist = newDist;
                        bestNewX = x2;
                        bestNewY = y2;
                    }
                }
                x = bestNewX;
                y = bestNewY;
                dist = minNeighborDist;
                path.Add(new Path { Position = new float2(x, y) });
            }
        }
    }

    [BurstCompile]
    struct MoveAlongPath : IJobChunk
    {
        public Unity.Mathematics.Random random;

        public float deltaTime;
        public FarmConfig farmConfig;
        public FarmerConfig farmerConfig;
        
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<byte> groundTiles;

        public BufferTypeHandle<Path> pathTypeHandle;
        public ComponentTypeHandle<Farmer> farmerTypeHandle;
        public ComponentTypeHandle<Translation> translationTypeHandle;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        { 
            BufferAccessor<Path> paths = chunk.GetBufferAccessor(pathTypeHandle);
            NativeArray<Farmer> farmers = chunk.GetNativeArray(farmerTypeHandle);
            NativeArray<Translation> translations = chunk.GetNativeArray(translationTypeHandle); 

            for (int i = 0; i < chunk.Count; i++)
            {
                DynamicBuffer<Path> path = paths[i];
                Farmer farmer = farmers[i];
                Translation translation = translations[i];

                for (int j = 0; j < path.Length - 1; j++) 
                {
				    Debug.DrawLine(new Vector3(path[j].Position.x, 1f, path[j].Position.y), new Vector3(path[j+1].Position.x, 1f, path[j+1].Position.y), Color.red);
			    }

                if (path.Length > 0)
                {
                    float nextTileX = path[path.Length-1].Position.x;
			        float nextTileY = path[path.Length-1].Position.y;

					Vector3 targetPos = new Vector3(nextTileX, translation.Value.y, nextTileY);

                    
			        //Debug.DrawLine(new Vector3(farmer.TillableZone.x, 1f, farmer.TillableZone.z),new Vector3(farmer.TillableZone.y, 1f, farmer.TillableZone.z), Color.green);
			        //Debug.DrawLine(new Vector3(farmer.TillableZone.y, 1f, farmer.TillableZone.z),new Vector3(farmer.TillableZone.y, 1f, farmer.TillableZone.w), Color.green);
			        //Debug.DrawLine(new Vector3(farmer.TillableZone.y, 1f, farmer.TillableZone.w),new Vector3(farmer.TillableZone.x, 1f, farmer.TillableZone.w), Color.green);
			        //Debug.DrawLine(new Vector3(farmer.TillableZone.x, 1f, farmer.TillableZone.w),new Vector3(farmer.TillableZone.x, 1f, farmer.TillableZone.z), Color.green);
			        
                    //if (IsTillableInZone(posX, posY, farmer.TillableZone))
                    //{
                    //    TillGround(posX, posY, groundTiles, farmConfig.MapSizeX);
                    //}

                    if (math.distance(translation.Value, targetPos) < 0.01f)
                    {
                        translation.Value = targetPos;
                        path.RemoveAt(path.Length-1);
                        
                        if(path.Length == 0)
                        {
                            int posX = (int)translation.Value.x;
                            int posY = (int)translation.Value.z;
                            TillGround(posX, posY, groundTiles, farmConfig.MapSizeX);
                        }
                    }
                    else
                    {
					    targetPos = new Vector3(nextTileX, translation.Value.y, nextTileY);
			            Vector3 position = Vector3.MoveTowards(translation.Value, targetPos, farmerConfig.WalkSpeed * deltaTime);
			            float smooth = 1f - Mathf.Pow(farmerConfig.MoveSmooth, deltaTime);
		                translation.Value = Vector3.Lerp(translation.Value, position, smooth);
                    }
                }

                farmers[i] = farmer;
                translations[i] = translation;
            }
        }
	    private bool IsTillableInZone(int x, int y, float4 tillingZone) 
        {
		    return x >= tillingZone.x && x <= tillingZone.y &&
			    y >= tillingZone.z && y <= tillingZone.w;
	    }
        
	    private void TillGround(int x, int y, NativeArray<byte> groundTiles, int mapWidth) 
        {
            int hash = PathUtility.Hash(x, y, mapWidth);
		    groundTiles[hash] = byte.MaxValue;
	    }
    }
	
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<FarmerConfig>();
        RequireSingletonForUpdate<FarmConfig>();
        RequireSingletonForUpdate<GridData>();
    }

    protected override void OnUpdate()
    {
        var farmConfig = GetSingleton<FarmConfig>();
        var farmerConfig = GetSingleton<FarmerConfig>();
        var gridData = this.GetSingleton<GridData>();

        float deltaTime = Time.DeltaTime;
        
        var farmerQuery = GetEntityQuery(typeof(Farmer));
        var pathTypeHandle = GetBufferTypeHandle<Path>();
        var farmerTypeHandle = GetComponentTypeHandle<Farmer>();
        var translationTypeHandle = GetComponentTypeHandle<Translation>();
        
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);

        JobHandle handleA = new FindTarget
        {
            random = random,
            farmConfig = farmConfig,
            farmerConfig = farmerConfig,
            groundTiles = gridData.groundTiles,
            pathTypeHandle = pathTypeHandle,
            farmerTypeHandle = farmerTypeHandle,
            translationTypeHandle = translationTypeHandle
        }.Schedule(farmerQuery, Dependency);

        JobHandle handleB = new MoveAlongPath
        {
            random = random,
            deltaTime = deltaTime,
            farmConfig = farmConfig,
            farmerConfig = farmerConfig,
            groundTiles = gridData.groundTiles,
            pathTypeHandle = pathTypeHandle,
            farmerTypeHandle = farmerTypeHandle,
            translationTypeHandle = translationTypeHandle
        }.Schedule(farmerQuery, handleA);

        Dependency = handleB;
    }
}