using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[ExecuteAlways]
public class GridCreationSystem : SystemBase
{
    public NativeArray<CellInfo> Cells { get; private set; }

    private EntityCommandBufferSystem m_commandBuffer;

    protected override void OnCreate()
    {
        m_commandBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();

        base.OnCreate();
    }

    protected override void OnDestroy()
    {
        if (Cells.IsCreated)
        {
            Cells.Dispose();
        }

        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        var constantData = ConstantData.Instance;

        if(!Cells.IsCreated && constantData != null)
        {
            // Ugly Simon's code, if it works, don't change it!
            unsafe
            {
                Unity.Physics.BoxCollider* boardCollider = (Unity.Physics.BoxCollider*)GetSingleton<Unity.Physics.PhysicsCollider>().ColliderPtr;
                var geometry = boardCollider->Geometry;
                geometry.Size = new float3(constantData.BoardDimensions.x, 1, constantData.BoardDimensions.y);
                geometry.Center = new float3(constantData.BoardDimensions.x / 2, -1, constantData.BoardDimensions.y / 2);
                boardCollider->Geometry = geometry;
            }

            Cells = new NativeArray<CellInfo>(constantData.BoardDimensions.x * constantData.BoardDimensions.y, Allocator.Persistent);

            var cellsarray = Cells;

            int width = constantData.BoardDimensions.x;
            int height = constantData.BoardDimensions.y;
            int bottomRight = (width * height) - 1;

            Job.WithCode(() => 
            {
                int bottomLeft = width * (height - 1);

                cellsarray[0] = cellsarray[0].SetTravelDirections(GridDirection.NORTH | GridDirection.EAST);

                cellsarray[width - 1] = cellsarray[width - 1].SetTravelDirections(GridDirection.NORTH | GridDirection.WEST);
                cellsarray[bottomLeft] = cellsarray[bottomLeft].SetTravelDirections(GridDirection.SOUTH | GridDirection.EAST);
                cellsarray[bottomRight] = cellsarray[(width * height) - 1].SetTravelDirections(GridDirection.SOUTH | GridDirection.WEST);

                GridDirection fromNorth = GridDirection.SOUTH | GridDirection.EAST | GridDirection.WEST;
                GridDirection fromSouth = GridDirection.NORTH | GridDirection.EAST | GridDirection.WEST;
                GridDirection fromWest = GridDirection.NORTH | GridDirection.SOUTH | GridDirection.EAST;
                GridDirection fromEast = GridDirection.NORTH | GridDirection.SOUTH | GridDirection.WEST;

                for (int i = 1; i < (width - 1); i++)
                {
                    cellsarray[i] = cellsarray[i].SetTravelDirections(fromSouth);
                    cellsarray[bottomLeft + i] = cellsarray[bottomLeft + i].SetTravelDirections(fromNorth);
                }

                for (int i = 1; i < (height - 1); i++)
                {
                    cellsarray[width * i] = cellsarray[width * i].SetTravelDirections(fromWest);
                    cellsarray[(width * (i + 1)) - 1] = cellsarray[(width * (i + 1)) - 1].SetTravelDirections(fromEast);
                }

                cellsarray[width + 1] = cellsarray[width + 1].SetIsHole();
                
            }).Schedule();

            var ecb = m_commandBuffer.CreateCommandBuffer();
            var cellSize = constantData.CellSize;

            Entities.ForEach((in PrefabReferenceComponent prefabs) => {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if(!cellsarray[(width * y) + x].IsHole())
                        {
                            var entity = ecb.Instantiate(prefabs.CellPrefab);

                            if (entity != Entity.Null)
                            {
                                ecb.SetComponent(entity, new Position2D { Value = Utility.GridCoordinatesToWorldPos(new int2(x, y), cellSize) });
                            }
                        }
                    }
                }

                var spawner = ecb.Instantiate(prefabs.MouseSpawnerPrefab);
                if(spawner != Entity.Null)
                {
                    ecb.SetComponent(spawner, new Position2D { Value = Utility.GridCoordinatesToWorldPos(new int2(0, 0), cellSize) });
                    ecb.SetComponent(spawner, new Direction2D { Value = GridDirection.NORTH });
                }

                spawner = ecb.Instantiate(prefabs.MouseSpawnerPrefab);
                if (spawner != Entity.Null)
                {
                    ecb.SetComponent(spawner, new Position2D { Value = Utility.GridCoordinatesToWorldPos(new int2(width - 1, height - 1), cellSize) });
                    ecb.SetComponent(spawner, new Direction2D { Value = GridDirection.SOUTH });
                }

                spawner = ecb.Instantiate(prefabs.CatSpawnerPrefab);
                if (spawner != Entity.Null)
                {
                    ecb.SetComponent(spawner, new Position2D { Value = Utility.GridCoordinatesToWorldPos(new int2(width - 1, 0), cellSize) });
                    ecb.SetComponent(spawner, new Direction2D { Value = GridDirection.WEST });
                }

                spawner = ecb.Instantiate(prefabs.CatSpawnerPrefab);
                if (spawner != Entity.Null)
                {
                    ecb.SetComponent(spawner, new Position2D { Value = Utility.GridCoordinatesToWorldPos(new int2(0, height - 1), cellSize) });
                    ecb.SetComponent(spawner, new Direction2D { Value = GridDirection.EAST });
                }

            }).Schedule();

            m_commandBuffer.AddJobHandleForProducer(Dependency);

            /*for (int i = 0; i <= bottomRight; i++)
            {
                var directions = i +" can travel: ";
                if (cellsarray[i].CanTravel(GridDirection.NORTH)) directions += "N";
                if (cellsarray[i].CanTravel(GridDirection.EAST)) directions += "E";
                if (cellsarray[i].CanTravel(GridDirection.SOUTH)) directions += "S";
                if (cellsarray[i].CanTravel(GridDirection.WEST)) directions += "W";

                Debug.Log(directions);
                Debug.Log(i + " is hole: " + cellsarray[i].IsHole());
            }

            Debug.Log("cell infos created"); */
        }
    }
}
