using Unity.Mathematics;

public struct DistanceFieldMathUtil
{
    // Smooth-Minimum, from Media Molecule's "Dreams"
    static public float SmoothMin(float a, float b, float radius)
    {
        float e = math.max(radius - math.abs(a - b), 0);
        return math.min(a, b) - e * e * 0.25f / radius;
    }

    static public float Sphere(float x, float y, float z, float radius)
    {
        return math.sqrt(x * x + y * y + z * z) - radius;
    }

    static public float GetDistance(float x, float y, float z, float time, DistanceFieldModel model, out float3 normal)
    {
        float distance = float.MaxValue;
        normal = float3.zero;
        if (model == DistanceFieldModel.Metaballs)
        {
            for (int i = 0; i < 5; i++)
            {
                float orbitRadius = i * .5f + 2f;
                float angle1 = time * 4f * (1f + i * .1f);
                float angle2 = time * 4f * (1.2f + i * .117f);
                float angle3 = time * 4f * (1.3f + i * .1618f);
                float cx = math.cos(angle1) * orbitRadius;
                float cy = math.sin(angle2) * orbitRadius;
                float cz = math.sin(angle3) * orbitRadius;

                float newDist = SmoothMin(distance, Sphere(x - cx, y - cy, z - cz, 2f), 2f);
                if (newDist < distance)
                {
                    normal = new float3(x - cx, y - cy, z - cz);
                    distance = newDist;
                }
            }
        }
        else if (model == DistanceFieldModel.SpinMixer)
        {
            for (int i = 0; i < 6; i++)
            {
                float orbitRadius = (i / 2 + 2) * 2;
                float angle = time * 20f * (1f + i * .1f);
                float cx = math.cos(angle) * orbitRadius;
                float cy = math.sin(angle);
                float cz = math.sin(angle) * orbitRadius;

                float newDist = Sphere(x - cx, y - cy, z - cz, 2f);
                if (newDist < distance)
                {
                    normal = new float3(x - cx, y - cy, z - cz);
                    distance = newDist;
                }
            }
        }
        else if (model == DistanceFieldModel.SpherePlane)
        {
            float sphereDist = Sphere(x, y, z, 5f);
            float3 sphereNormal = math.normalize(new float3(x, y, z));

            float planeDist = y;
            float3 planeNormal = new float3(0f, 1f, 0f);

            float t = math.sin(time * 8f) * .4f + .4f;
            distance = math.lerp(sphereDist, planeDist, t);
            normal = math.lerp(sphereNormal, planeNormal, t);
        }
        else if (model == DistanceFieldModel.SphereField)
        {
            float spacing = 5f + math.sin(time * 5f) * 2f;
            x += spacing * .5f;
            y += spacing * .5f;
            z += spacing * .5f;
            x -= math.floor(x / spacing) * spacing;
            y -= math.floor(y / spacing) * spacing;
            z -= math.floor(z / spacing) * spacing;
            x -= spacing * .5f;
            y -= spacing * .5f;
            z -= spacing * .5f;
            distance = Sphere(x, y, z, 5f);
            normal = new float3(x, y, z);
        }
        else if (model == DistanceFieldModel.FigureEight)
        {
            float ringRadius = 4f;
            float flipper = 1f;
            if (z < 0f)
            {
                z = -z;
                flipper = -1f;
            }
            float3 point = math.normalize(new float3(x, 0f, z - ringRadius)) * ringRadius;
            float angle = math.atan2(point.z, point.x) + time * 8f;
            point += new float3(0, 0, 1) * ringRadius;
            normal = new float3(x - point.x, y - point.y, (z - point.z) * flipper);
            float wave = math.cos(angle * flipper * 3f) * .5f + .5f;
            wave *= wave * .5f;
            distance = math.sqrt(normal.x * normal.x + normal.y * normal.y + normal.z * normal.z) - (.5f + wave);
        }
        else if (model == DistanceFieldModel.PerlinNoise)
        {
            float perlin = noise.cnoise(new float2(x * .2f, z * .2f));
            distance = y - perlin * 6f;
            normal = math.up();
        }

        return distance;
    }
}
