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

    protected override void OnCreate()
    {
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
            int bottomLeft = width * (height - 1);
            int bottomRight = (width * height) - 1;

            cellsarray[0] = cellsarray[0].SetTravelDirections(GridDirection.SOUTH | GridDirection.EAST);

            cellsarray[width - 1] = cellsarray[width - 1].SetTravelDirections(GridDirection.SOUTH | GridDirection.WEST);
            cellsarray[bottomLeft] = cellsarray[bottomLeft].SetTravelDirections(GridDirection.NORTH | GridDirection.EAST);
            cellsarray[bottomRight] = cellsarray[(width * height) - 1].SetTravelDirections(GridDirection.NORTH | GridDirection.WEST);

            GridDirection fromNorth = GridDirection.SOUTH | GridDirection.EAST | GridDirection.WEST;
            GridDirection fromSouth = GridDirection.NORTH | GridDirection.EAST | GridDirection.WEST;
            GridDirection fromWest = GridDirection.NORTH | GridDirection.SOUTH | GridDirection.WEST;
            GridDirection fromEast = GridDirection.NORTH | GridDirection.SOUTH | GridDirection.EAST;
            
            for (int i = 1; i < width; i++)
            {
                cellsarray[i] = cellsarray[i].SetTravelDirections(fromNorth);
                cellsarray[bottomLeft + i] = cellsarray[bottomLeft + i].SetTravelDirections(fromSouth);
            }

            for (int i = 1; i < height; i++)
            {
                cellsarray[width * i] = cellsarray[width * i].SetTravelDirections(fromWest);
                cellsarray[(width * (i + 1)) - 1] = cellsarray[(width * (i + 1)) - 1].SetTravelDirections(fromEast);
            }

            /*for (int i = 0; i <= bottomRight; i++)
            {
                Debug.Log(i + " can travel south: " + cellsarray[i].CanTravel(GridDirection.SOUTH));
                Debug.Log(i + " can travel north: " + cellsarray[i].CanTravel(GridDirection.NORTH));
                Debug.Log(i + " can travel east: " + cellsarray[i].CanTravel(GridDirection.EAST));
                Debug.Log(i + " can travel west: " + cellsarray[i].CanTravel(GridDirection.WEST));
            }

            Debug.Log("cell infos created");*/
        }
    }
}
