using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.Burst;

public class BoardGenerationSystem : JobComponentSystem
{
    private static readonly byte[] DirList =
    {
        0x1B,
        0x5B,
        0x1F,
        0x5F,
        0x18,
        0x5A,
        0x14,
        0x55,
        0x2B,
        0xEB,
        0x0F,
        0xFF,
        0x28,
        0xAA,
        0x00,
        0x00, // TODO: Verify this
    };

    private const short kHoleFlag = 0x100;
    private const short kHomebaseFlag = 0x800;
    // public const short kPlayer1Flag = 0x000;
    private const short kPlayer2Flag = 0x200;
    private const short kPlayer3Flag = 0x400;
    private const short kPlayer4Flag = 0x600;

    const byte kVertialFlag     = 0x1;
    const byte kHorizonalFlag   = 0x2;

    private NativeHashMap<int2, Entity> m_FloorMap;
    private NativeHashMap<int2, byte> m_WallMap;
    private NativeHashMap<int2, byte> m_HomebaseMap;
    private NativeList<short> m_DirectionMap;

    private EntityCommandBufferSystem m_Barrier;
    private EntityQuery m_GeneratorQuery;

    private Random m_Random;

    protected override void OnCreate()
    {
        var desc = new EntityQueryDesc()
        {
            None = new ComponentType[] { ComponentType.ReadOnly<LbDisabled>() },
            All = new ComponentType [] { typeof(LbBoardGenerator) }
        };
        m_GeneratorQuery = GetEntityQuery(desc);

        m_FloorMap = new NativeHashMap<int2, Entity>(1, Allocator.Persistent);
        m_WallMap = new NativeHashMap<int2, byte>(1, Allocator.Persistent);
        m_HomebaseMap = new NativeHashMap<int2, byte>(4, Allocator.Persistent);
        m_DirectionMap = new NativeList<short>(1, Allocator.Persistent);

        m_Barrier = World.GetOrCreateSystem<LbSimulationBarrier>();
        m_Random = new Random(50);
    }

    protected override void OnDestroy()
    {
        m_FloorMap.Dispose();
        m_WallMap.Dispose();
        m_HomebaseMap.Dispose();
        m_DirectionMap.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //var count = m_GeneratorQuery.CalculateEntityCount();
        //if (count <= 0)
        //    return inputDeps;

        // Get the generator data
        var generator = m_GeneratorQuery.GetSingleton<LbBoardGenerator>();

        // Clean up our structs
        m_FloorMap.Clear();
        m_WallMap.Clear();
        m_HomebaseMap.Clear();
        m_DirectionMap.Clear();

        // Resize them
        var size = generator.SizeX * generator.SizeY;
        m_FloorMap.Capacity = size;

        m_DirectionMap.Capacity = size; 
        m_DirectionMap.Resize(size, NativeArrayOptions.UninitializedMemory);

        m_WallMap.Capacity = (generator.SizeX + 1) * (generator.SizeY + 1);

        //
        // Cell, Homebases and holes

        var floorCommandBuffer = m_Barrier.CreateCommandBuffer();
        var floorJobHandle = new GenerateFloorJob()
        {
            Generator = generator,
            Seed = m_Random.NextUInt(),
            CommandBuffer = floorCommandBuffer,

            FloorMap = m_FloorMap,
        }.Schedule(inputDeps);

        var homebaseHandle = new GenerateHomebasesJob()
        {
            Generator = generator,
            Seed = m_Random.NextUInt(),
            CommandBuffer = m_Barrier.CreateCommandBuffer(),

            HomebaseMap = m_HomebaseMap,
        }.Schedule(inputDeps);

        var holeJobHandle = new GenerateHoleJob()
        {
            Generator = generator,
            Seed = m_Random.NextUInt(),
            CommandBuffer = floorCommandBuffer,

            FloorMap = m_FloorMap,
            HomebaseMap = m_HomebaseMap,
        }.Schedule(JobHandle.CombineDependencies(floorJobHandle, homebaseHandle));

        //
        // Walls 

        var wallJobHandle = new GenerateWallsJob()
        {
            Generator = generator,
            Seed = m_Random.NextUInt(),
            CommandBuffer = m_Barrier.CreateCommandBuffer(),

            WallMap = m_WallMap,
        }.Schedule(inputDeps);

        // 
        // DirectionMap and Board 

        var directionMapJob = new DirectionMapJob()
        {
            Generator = generator,

            FloorMap = m_FloorMap,
            WallMap = m_WallMap,
            HomebaseMap = m_HomebaseMap,

            DirectionMap = m_DirectionMap.AsParallelWriter(),

        }.Schedule(generator.SizeY, 1, JobHandle.CombineDependencies(holeJobHandle, wallJobHandle));

        var boardJob = new BoardBuilderJob()
        {
            Generator = generator,
            CommandBuffer = m_Barrier.CreateCommandBuffer(),
            DirectionMap = m_DirectionMap,
        }.Schedule(directionMapJob);

        //
        // Clean UP

        var cleanUpHandle = new CleanUpGenerationJob()
        {
            CommandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent(),
        }.Schedule(this, boardJob);

        m_Barrier.AddJobHandleForProducer(cleanUpHandle);

        return cleanUpHandle;
    }

    struct GenerateFloorJob : IJob
    {
        [ReadOnly] public LbBoardGenerator Generator;
        [ReadOnly] public uint Seed;

        public EntityCommandBuffer CommandBuffer;

        public NativeHashMap<int2, Entity> FloorMap;

        public void Execute()
        {
            var random = new Random();
            random.InitState(Seed);

            for (int y = 0; y < Generator.SizeY; ++y)
            {
                for (int x = 0; x < Generator.SizeX; ++x)
                {
                    var cellEntity = CommandBuffer.Instantiate(Generator.CellPrefab);
                    CommandBuffer.AddComponent(cellEntity, new LbMap());

                    FloorMap.TryAdd(new int2(x,y), cellEntity);

                    var randomHeight = random.NextFloat(0.0f, 1.0f) * Generator.YNoise;
                    var translation = new Translation() { Value = new float3(x,  randomHeight, y) };
                    CommandBuffer.SetComponent(cellEntity, translation);
                }
            }
        }
    }

    struct GenerateWallsJob : IJob
    {
        [ReadOnly] public LbBoardGenerator Generator;
        [ReadOnly] public uint Seed;

        public EntityCommandBuffer CommandBuffer;

        public NativeHashMap<int2, byte> WallMap;

        public void Execute()
        {
            // Generate all borders 
            // Horizontal walls
            for (int i = 0; i < Generator.SizeX; ++i)
            {
                var coord = new int2(i, 0);
                var wallEntity = PlaceWall(ref CommandBuffer, Generator.WallPrefab, Directions.South, coord);

                byte currentValue = 0x0;
                if (WallMap.ContainsKey(coord))
                    currentValue = WallMap[coord];
                currentValue |= kHorizonalFlag;
                WallMap[coord] = currentValue;

                coord = new int2(i, Generator.SizeY);
                wallEntity = PlaceWall(ref CommandBuffer, Generator.WallPrefab, Directions.North, coord);

                currentValue = 0x0;
                if (WallMap.ContainsKey(coord))
                    currentValue = WallMap[coord];
                currentValue |= kHorizonalFlag;
                WallMap[coord] = currentValue;
            }
            // Vertical walls
            for (int i = 0; i < Generator.SizeY; ++i)
            {
                var coord = new int2(0, i);
                PlaceWall(ref CommandBuffer, Generator.WallPrefab, Directions.West, coord);

                byte currentValue = 0x0;
                if (WallMap.ContainsKey(coord))
                    currentValue = WallMap[coord];
                currentValue |= kVertialFlag;
                WallMap[coord] = currentValue;

                coord = new int2(Generator.SizeX, i);
                PlaceWall(ref CommandBuffer, Generator.WallPrefab, Directions.East, coord);

                currentValue = 0x0;
                if (WallMap.ContainsKey(coord))
                    currentValue = WallMap[coord];
                currentValue |= kVertialFlag;
                WallMap[coord] = currentValue;
            }

            // Generate all other random walls
            var random = new Random();
            random.InitState(Seed);

            var wallCount = (int)(Generator.SizeX * Generator.SizeY * 0.2f);
            for (int i=0; i<wallCount; ++i)
            {
                var coord = new int2(random.NextInt(0, Generator.SizeX), random.NextInt(0, Generator.SizeY));
                var direction = GetRandomDirection(random.NextInt(0, 4));
                var directionFlag = (direction == Directions.North || direction == Directions.South) ? kHorizonalFlag : kVertialFlag;

                // Do not add existing walls
                if ((coord.x == 0 && directionFlag == kVertialFlag) || (coord.y == 0 && directionFlag == kVertialFlag))
                {
                    i--;
                    continue;
                }

                byte currentValue = 0x0;
                if (WallMap.ContainsKey(coord))
                    currentValue = WallMap[coord];

                // Do not add existing walls
                if (currentValue != 0x0 && (currentValue & directionFlag) == directionFlag)
                {
                    i--;
                    continue;
                }

                int count = 0;
                if (direction != Directions.North && HasWall(WallMap, coord, Directions.North))
                    count++;
                if (direction != Directions.East && HasWall(WallMap, coord, Directions.East))
                    count++;
                if (direction != Directions.South && HasWall(WallMap, coord, Directions.South))
                    count++;
                if (direction != Directions.West && HasWall(WallMap, coord, Directions.West))
                    count++;

                // Avoid closed cells
                if (count >= 3)
                {
                    i--;
                    continue;
                }

                PlaceWall(ref CommandBuffer, Generator.WallPrefab, direction, coord);
                currentValue |= directionFlag;
                WallMap[coord] = currentValue;
            }
        }
    }

    struct GenerateHomebasesJob : IJob
    {
        [ReadOnly] public LbBoardGenerator Generator;
        [ReadOnly] public uint Seed;

        public EntityCommandBuffer CommandBuffer;

        public NativeHashMap<int2, byte> HomebaseMap;

        public void Execute()
        {
            // Setup home bases
            var offset = 1f / 3f;
            var placeX = (int)(Generator.SizeX * offset);
            var placeY = (int)(Generator.SizeY * offset);

            var coord = new int2(placeX, placeY);
            PlaceHomebase(ref CommandBuffer, coord, Generator.Player1Homebase);
            HomebaseMap[coord] = (byte)Players.Player1;

            coord = new int2(placeX * 2, placeY);
            PlaceHomebase(ref CommandBuffer, coord, Generator.Player2Homebase);
            HomebaseMap[coord] = (byte)Players.Player2;

            coord = new int2(placeX * 2, placeY * 2);
            PlaceHomebase(ref CommandBuffer, coord, Generator.Player3Homebase);
            HomebaseMap[coord] = (byte)Players.Player3;

            coord = new int2(placeX, placeY * 2);
            PlaceHomebase(ref CommandBuffer, coord, Generator.Player4Homebase);
            HomebaseMap[coord] = (byte)Players.Player4;
        }
    }

    struct GenerateHoleJob : IJob
    {
        [ReadOnly] public LbBoardGenerator Generator;
        [ReadOnly] public uint Seed;

        public EntityCommandBuffer CommandBuffer;

        public NativeHashMap<int2, Entity> FloorMap;
        [ReadOnly] public NativeHashMap<int2, byte> HomebaseMap;

        public void Execute()
        {
            var random = new Random();
            random.InitState(Seed);

            var holeCount = (int)(Generator.SizeX * Generator.SizeY * 0.05f);
            for (int i=0; i<holeCount; ++i)
            {
                var coord = new int2(random.NextInt(Generator.SizeX), random.NextInt(Generator.SizeY));
                
                // Do not spawn holes in the 4 corners
                if ((coord.x == 0 && coord.y == 0)
                    || (coord.x == 0 && coord.y == Generator.SizeY - 1)
                    || (coord.x == Generator.SizeX - 1 && coord.y == 0)
                    || (coord.x == Generator.SizeX - 1 && coord.y == Generator.SizeY - 1))
                {
                    continue;
                }

                // Do not place holes under homebases
                if (HomebaseMap.ContainsKey(coord))
                {
                    continue;
                }

                if (FloorMap.ContainsKey(coord))
                {
                    CommandBuffer.DestroyEntity(FloorMap[coord]);
                    FloorMap.Remove(coord);
                }
            }
        }
    }

    [BurstCompile]
    struct DirectionMapJob : IJobParallelFor
    {
        [ReadOnly] public LbBoardGenerator Generator;

        [ReadOnly] public NativeHashMap<int2, Entity> FloorMap;
        [ReadOnly] public NativeHashMap<int2, byte> WallMap;
        [ReadOnly] public NativeHashMap<int2, byte> HomebaseMap;

        [NativeDisableParallelForRestriction]
        public NativeArray<short> DirectionMap;

        public void Execute(int jobIndex)
        {
            var offset = jobIndex * Generator.SizeY;
            for (int i=0; i < Generator.SizeX; ++i)
            {
                var index = offset + i;
                var coord = new int2(i, jobIndex);

                var bitIndex = 0x0;
                if (HasWall(WallMap, coord, Directions.North))
                    bitIndex |= 0x1;
                if (HasWall(WallMap, coord, Directions.South))
                    bitIndex |= 0x2;
                if (HasWall(WallMap, coord, Directions.West))
                    bitIndex |= 0x4;
                if (HasWall(WallMap, coord, Directions.East))
                    bitIndex |= 0x8;

                short bufferValue = DirList[bitIndex];

                if (!FloorMap.ContainsKey(coord))
                    bufferValue |= kHoleFlag;

                if (HomebaseMap.ContainsKey(coord))
                {
                    switch ((Players)HomebaseMap[coord])
                    {
                        case Players.Player2:
                            bufferValue |= kPlayer2Flag;
                            break;

                        case Players.Player3:
                            bufferValue |= kPlayer3Flag;
                            break;

                        case Players.Player4:
                            bufferValue |= kPlayer4Flag;
                            break;
                    }

                    bufferValue |= kHomebaseFlag;
                }

                DirectionMap[index] = bufferValue;
            }
        }
    }

    struct BoardBuilderJob : IJob
    {
        [ReadOnly] public LbBoardGenerator Generator;

        public EntityCommandBuffer CommandBuffer;

        [ReadOnly] public NativeList<short> DirectionMap;

        public void Execute()
        {
            var entity = CommandBuffer.CreateEntity();
            CommandBuffer.AddComponent(entity, new LbMap());

            var board = new LbBoard()
            {
                SizeX = Generator.SizeX,
                SizeY = Generator.SizeY,
            };
            CommandBuffer.AddComponent(entity, board);

            var dirMapBuffer = CommandBuffer.AddBuffer<LbDirectionMap>(entity);
            for (int i = 0; i < DirectionMap.Length; ++i)
                dirMapBuffer.Add(DirectionMap[i]);

            var arrowDirbuffer = CommandBuffer.AddBuffer<LbArrowDirectionMap>(entity);
            for (int i = 0; i < Generator.SizeX * Generator.SizeY; ++i)
                arrowDirbuffer.Add(new LbArrowDirectionMap());

            var catMapbuffer = CommandBuffer.AddBuffer<LbCatMap>(entity);

            var bitsInWord = sizeof(int) * 8;
            var bufferCountInWords = (Generator.SizeX * Generator.SizeY + (bitsInWord - 1)) / bitsInWord;
            for (int i = 0; i < bufferCountInWords; i++)
            {
                catMapbuffer.Add(new LbCatMap());
            }
        }
    }

    struct CleanUpGenerationJob : IJobForEachWithEntity<LbBoardGenerator>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, ref LbBoardGenerator generator)
        {
            CommandBuffer.AddComponent(index, entity, new LbDisabled());

            var startEntity = CommandBuffer.CreateEntity(index);
            CommandBuffer.AddComponent(index, startEntity, new LbGameStart());
        }
    }

    #region HELPERS
    /// <summary>
    /// Return true if there is a wall in the given direction starting in the given location
    /// </summary>
    public static bool HasWall(NativeHashMap<int2, byte> WallMap, int2 coord, Directions direction)
    {
        switch (direction)
        {
            case Directions.North:
                coord += new int2(0, 1);
                break;

            case Directions.East:
                coord += new int2(1, 0);
                break;
        }

        if (WallMap.ContainsKey(coord))
        {
            var flags = WallMap[coord];
            if ((flags & kHorizonalFlag) == kHorizonalFlag && (direction == Directions.North || direction == Directions.South))
            {
                return true;
            }
            else if ((flags & kVertialFlag) == kVertialFlag && (direction == Directions.West || direction == Directions.East))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get a random direction 
    /// </summary>
    /// <returns></returns>
    public static Directions GetRandomDirection(int value)
    {
        switch (value)
        {
            case 0:
                return Directions.North;

            case 1:
                return Directions.East;

            case 2:
                return Directions.South;

            default:
                return Directions.West;
        }
    }

    public static Entity PlaceHomebase(ref EntityCommandBuffer buffer, int2 coord, Entity prefab)
    {
        var entity = buffer.Instantiate(prefab);

        var center = new Vector3(
            coord.x,
            0.0f,                   // Change when we have a height variable
            coord.y);

        buffer.AddComponent(entity, new LbMap());
        buffer.SetComponent(entity, new Translation() { Value = center });

        return entity;
    }

    public static Entity PlaceWall(ref EntityCommandBuffer buffer, Entity prefab, Directions direction, int2 coord)
    {
        var entity = buffer.Instantiate(prefab);

        var halfBoardWidth = 0.5f;
        var halfWallWidth = 0.025f;

        var center = new float3(coord.x, 0.75f, coord.y);

        var offset = float3.zero;
        switch(direction)
        {
            case Directions.North:
                offset = new float3(0.0f, 0.0f, -1.0f * (halfBoardWidth + halfWallWidth));
                break;

            case Directions.East:
                offset = new float3(-1.0f * (halfWallWidth + halfBoardWidth), 0.0f, 0.0f);
                break;

            case Directions.West:
                offset = new float3(halfWallWidth - halfBoardWidth, 0.0f, 0.0f);
                break;

            case Directions.South:
                offset = new float3(0.0f, 0.0f, halfWallWidth - halfBoardWidth);
                break;
        }

        buffer.SetComponent(entity, new Translation() { Value = center + offset });
        if (direction == Directions.North || direction == Directions.South)
        {
            buffer.SetComponent(entity, new Rotation() { Value = quaternion.EulerXYZ(0.0f, math.PI * 0.5f, 0.0f) });
        }
        else
        {
            buffer.SetComponent(entity, new Rotation() { Value = quaternion.EulerXYZ(0.0f, math.PI, 0.0f) });
        }

        buffer.AddComponent(entity, new LbMap());

        return entity;
    }
    #endregion
}
