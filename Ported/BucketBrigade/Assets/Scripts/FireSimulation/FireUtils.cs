class FireUtils
{
    public static int GridToArrayCoord(int row, int column, int rowCount)
    {
        return column * rowCount + row;
    }

    public static void ArrayToGridCoord(int index, int rowCount, out int row, out int column)
    {
        row = index % rowCount;
        column = UnityEngine.Mathf.FloorToInt(index / rowCount);
    }
}
