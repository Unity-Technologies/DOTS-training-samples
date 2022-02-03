using Unity.Mathematics;

static class Extensions 
{
    // WB: Created this class just to experiment with extension methods within Dots/ECS environment   
    public static float3 MoveTowards(this float3 current, float3 target, float maxDistanceDelta)
    {
        // Based of the classic static Vector3 function: Vector3.MoveTowards
        // does the exact same thing, just for float 3
        // call it like below:
        //      float3 currentPosition = currentPosition.MoveTowards(destination, 1f * time.deltaTime)

        float num1 = target.x - current.x;
        float num2 = target.y - current.y;
        float num3 = target.z - current.z;
        float num4 = (float)((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
        if ((double)num4 == 0.0 || (double)maxDistanceDelta >= 0.0 && (double)num4 <= (double)maxDistanceDelta * (double)maxDistanceDelta)
            return target;
        float num5 = (float)math.sqrt((double)num4);
        return new float3(current.x + num1 / num5 * maxDistanceDelta, current.y + num2 / num5 * maxDistanceDelta, current.z + num3 / num5 * maxDistanceDelta);
    }
    
    public static float DistanceToFloat(this float3 delta)
    {
        return math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
    }
}
