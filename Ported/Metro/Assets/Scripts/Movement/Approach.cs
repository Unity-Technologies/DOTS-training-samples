using Unity;
using Unity.Mathematics;

public class Approach {

    public static bool Apply(ref float current, ref float speed, float target, float acceleration, float arrivalThreshold, float friction)
    {
        speed *= friction;
        if (current < (target - arrivalThreshold))
        {
            speed += acceleration;
            current += speed;
            return false;
        }else if (current > (target + arrivalThreshold))
        {
            speed -= acceleration;
            current += speed;
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool Apply(ref float3 position, ref float3 speed, float3 destination, float acceleration,
        float arrivalThreshold, float friction)
    {
        bool arrivedX = Approach.Apply(ref position.x, ref speed.x, destination.x, acceleration, arrivalThreshold, friction);
        bool arrivedY = Approach.Apply(ref position.y, ref speed.y, destination.y, acceleration, arrivalThreshold, friction);
        bool arrivedZ = Approach.Apply(ref position.z, ref speed.z, destination.z, acceleration, arrivalThreshold, friction);
        
        return (arrivedX && arrivedY && arrivedZ);
    }
}