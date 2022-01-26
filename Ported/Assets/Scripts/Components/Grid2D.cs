public readonly struct Grid2D : Unity.Entities.IComponentData 
{
    public readonly int rowLength { get; }
    public readonly int columnLength { get; }

    public Grid2D(in int rowLength, in int columnLength)
    {
        this.rowLength = rowLength;
        this.columnLength = columnLength;
    }
}