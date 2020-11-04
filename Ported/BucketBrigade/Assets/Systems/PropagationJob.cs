using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public struct PropagationJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<byte> InputCells;
    public NativeArray<byte> OutputCells;

    public int PropagationRadius;
    public int FireGridDimension;
    public float HeatTransfer;

    public void Execute(int index)
    {
        // Calculate the two dimensions indices for this cel
        int2 cell = new int2(index / FireGridDimension, index % FireGridDimension);

        var radius = PropagationRadius / 2;
        for (int i = -radius; i < radius; i++)
        {
            for (int j = -radius; j < radius; j++)
            {
                int2 neighbour = new int2(cell.x + i, cell.y + j);
                if (neighbour.x < 0 || neighbour.x + i > FireGridDimension ||
                    neighbour.y < 0 || neighbour.y > FireGridDimension)
                    continue;

                OutputCells[cell.x * FireGridDimension + cell.y] += (byte)(HeatTransfer * InputCells[neighbour.x * FireGridDimension + neighbour.y]);
            }
        }
    }
}