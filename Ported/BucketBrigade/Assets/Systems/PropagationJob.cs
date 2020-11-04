using System;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public struct PropagationJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float> InputCells;
    public NativeArray<float> OutputCells;

    public int PropagationRadius;
    public int FireGridDimension;
    public float HeatTransfer;

    public void Execute(int index)
    {
        Assert.IsTrue(index < FireGridDimension * FireGridDimension);
        float Temperature = 0;
        // Calculate the two dimensions indices for this cel
        int2 cell = new int2(index / FireGridDimension, index % FireGridDimension);

        var radius = PropagationRadius;
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            { 
                int2 neighbour = new int2(cell.x + i, cell.y + j);
                if (neighbour.x < 0 || neighbour.x + i > FireGridDimension - 1 ||
                    neighbour.y < 0 || neighbour.y > FireGridDimension - 1)
                    continue;
                Temperature += (HeatTransfer * InputCells[neighbour.x * FireGridDimension + neighbour.y]);
            }
        }
        Temperature += InputCells[index] * (1.0f - HeatTransfer);
        OutputCells[cell.x * FireGridDimension + cell.y] = (Temperature > 1 ? 1 : Temperature);
    }
}