using Unity.Mathematics;
namespace Extensions
{
    public static class QuaternionExtensions
    {
        public static float ComputeXAngle(this quaternion q)
        {
            float sinr_cosp = 2 * (q.value.w * q.value.x + q.value.y * q.value.z);
            float cosr_cosp = 1 - 2 * (q.value.x * q.value.x + q.value.y * q.value.y);
            return math.atan2(sinr_cosp, cosr_cosp);
        }

        public static float ComputeYAngle(this quaternion q)
        {
            float sinp = 2 * (q.value.w * q.value.y - q.value.z * q.value.x);
            if (math.abs(sinp) >= 1)
                return math.PI / 2 * math.sign(sinp); // use 90 degrees if out of range
            else
                return math.asin(sinp);
        }

        public static float ComputeZAngle(this quaternion q)
        {
            float siny_cosp = 2 * (q.value.w * q.value.z + q.value.x * q.value.y);
            float cosy_cosp = 1 - 2 * (q.value.y * q.value.y + q.value.z * q.value.z);
            return math.atan2(siny_cosp, cosy_cosp);
        }

        public static float3 ComputeAngles(this quaternion q)
        {
            return new float3(ComputeXAngle(q), ComputeYAngle(q), ComputeZAngle(q));
        }

        public static quaternion FromAngles(float3 angles)
        {

            float cy = math.cos(angles.z * 0.5f);
            float sy = math.sin(angles.z * 0.5f);
            float cp = math.cos(angles.y * 0.5f);
            float sp = math.sin(angles.y * 0.5f);
            float cr = math.cos(angles.x * 0.5f);
            float sr = math.sin(angles.x * 0.5f);

            float4 q;
            q.w = cr * cp * cy + sr * sp * sy;
            q.x = sr * cp * cy - cr * sp * sy;
            q.y = cr * sp * cy + sr * cp * sy;
            q.z = cr * cp * sy - sr * sp * cy;

            return q;

        }
    }
}
