using Unity.Entities;

public static class Utils
{
    public static bool IsTopOfStack(ResourceGridParams resGridParams, DynamicBuffer<StackHeightParams> stackHeights,
                                int gridX, int gridY, int stackIndex, bool stacked)
    {
        if (stacked)
        {
            int index = resGridParams.gridCounts.x * gridX + gridY;
            if (index == stackHeights[index].Value - 1)
            {
                return true;
            }
        }
        return false;

    }

    public static void UpdateStackHeights(ResourceGridParams resGridParams, DynamicBuffer<StackHeightParams> stackHeights, 
                                            int gridX, int gridY, bool stacked, int updateValue)
    {
        if (stacked)
        {
            int index = resGridParams.gridCounts.x * gridX + gridY;

            var element = stackHeights[index];
            element.Value += updateValue;
            stackHeights[index] = element;
        }
    }
}