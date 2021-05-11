
using Unity.Mathematics;

public static class Colorize
{
    public static float4 MIN_HEIGHT_COLOR = new float4(0F, 1F, 0F, 1F);
    public static float4 MAX_HEIGHT_COLOR = new float4(0.388F, 0.184F, 0F, 1F);

    public static float4 Platform(float height, float minHeight, float maxHeight)
    {
        if (height - 0.0001 <= minHeight)
        {
            return MIN_HEIGHT_COLOR;
        }
        else
        {
            return math.lerp(MIN_HEIGHT_COLOR, MAX_HEIGHT_COLOR, (height - minHeight) / (maxHeight - minHeight));
        }
    }
}
