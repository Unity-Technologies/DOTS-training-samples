using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections.Generic;

// Smooth-Minimum, from Media Molecule's "Dreams"
/*/
static float SmoothMin(float a, float b, float radius)
{
    float e = Mathf.Max(radius - Mathf.Abs(a - b), 0);
    return Mathf.Min(a, b) - e * e * 0.25f / radius;
}

static float Sphere(float x, float y, float z, float radius)
{
    return Mathf.Sqrt(x * x + y * y + z * z) - radius;
}

// what's the shortest distance from a given point to the isosurface?
public static float GetDistance(float x, float y, float z, out Vector3 normal)
{
    float distance = float.MaxValue;
    normal = Vector3.zero;
    if (instance.model == DistanceFieldModel.Metaballs)
    {
        for (int i = 0; i < 5; i++)
        {
            float orbitRadius = i * .5f + 2f;
            float angle1 = time * 4f * (1f + i * .1f);
            float angle2 = time * 4f * (1.2f + i * .117f);
            float angle3 = time * 4f * (1.3f + i * .1618f);
            float cx = Mathf.Cos(angle1) * orbitRadius;
            float cy = Mathf.Sin(angle2) * orbitRadius;
            float cz = Mathf.Sin(angle3) * orbitRadius;

            float newDist = SmoothMin(distance, Sphere(x - cx, y - cy, z - cz, 2f), 2f);
            if (newDist < distance)
            {
                normal = new Vector3(x - cx, y - cy, z - cz);
                distance = newDist;
            }
        }
    }
    else if (instance.model == DistanceFieldModel.SpinMixer)
    {
        for (int i = 0; i < 6; i++)
        {
            float orbitRadius = (i / 2 + 2) * 2;
            float angle = time * 20f * (1f + i * .1f);
            float cx = Mathf.Cos(angle) * orbitRadius;
            float cy = Mathf.Sin(angle);
            float cz = Mathf.Sin(angle) * orbitRadius;

            float newDist = Sphere(x - cx, y - cy, z - cz, 2f);
            if (newDist < distance)
            {
                normal = new Vector3(x - cx, y - cy, z - cz);
                distance = newDist;
            }
        }
    }
    else if (instance.model == DistanceFieldModel.SpherePlane)
    {
        float sphereDist = Sphere(x, y, z, 5f);
        Vector3 sphereNormal = new Vector3(x, y, z).normalized;

        float planeDist = y;
        Vector3 planeNormal = new Vector3(0f, 1f, 0f);

        float t = Mathf.Sin(time * 8f) * .4f + .4f;
        distance = Mathf.Lerp(sphereDist, planeDist, t);
        normal = Vector3.Lerp(sphereNormal, planeNormal, t);
    }
    else if (instance.model == DistanceFieldModel.SphereField)
    {WorldDetails
        float spacing = 5f + Mathf.Sin(time * 5f) * 2f;
        x += spacing * .5f;
        y += spacing * .5f;
        z += spacing * .5f;
        x -= Mathf.Floor(x / spacing) * spacing;
        y -= Mathf.Floor(y / spacing) * spacing;
        z -= Mathf.Floor(z / spacing) * spacing;
        x -= spacing * .5f;
        y -= spacing * .5f;
        z -= spacing * .5f;
        distance = Sphere(x, y, z, 5f);
        normal = new Vector3(x, y, z);
    }
    else if (instance.model == DistanceFieldModel.FigureEight)
    {
        float ringRadius = 4f;
        float flipper = 1f;
        if (z < 0f)
        {
            z = -z;
            flipper = -1f;
        }
        Vector3 point = new Vector3(x, 0f, z - ringRadius).normalized * ringRadius;
        float angle = Mathf.Atan2(point.z, point.x) + time * 8f;
        point += Vector3.forward * ringRadius;
        normal = new Vector3(x - point.x, y - point.y, (z - point.z) * flipper);
        float wave = Mathf.Cos(angle * flipper * 3f) * .5f + .5f;
        wave *= wave * .5f;
        distance = Mathf.Sqrt(normal.x * normal.x + normal.y * normal.y + normal.z * normal.z) - (.5f + wave);
    }
    else if (instance.model == DistanceFieldModel.PerlinNoise)
    {
        float perlin = Mathf.PerlinNoise(x * .2f, z * .2f);
        distance = y - perlin * 6f;
        normal = Vector3.up;
    }

    return distance;
}

// what's the slope of the distance field at a given point?
/*public static Vector3 GetGradient(float x, float y, float z,float baseValue) {
    float eps = 0.01f;
    float dx = GetDistance(x + eps,y,z) - baseValue;
    float dy = GetDistance(x,y + eps,z) - baseValue;
    float dz = GetDistance(x,y,z + eps) - baseValue;

    return new Vector3(dx / eps,dy / eps,dz / eps);
}

private void Start()
{
    instance = this;
    modelCount = System.Enum.GetValues(typeof(DistanceFieldModel)).Length;
}

private void FixedUpdate()
{
    time = Time.time * .1f;
}

private void Update()
{
    switchTimer += Time.deltaTime * .1f;
    if (switchTimer > 1f)
    {
        switchTimer -= 1f;
        int newModel = Random.Range(0, modelCount - 1);
        if (newModel >= (int)model)
        {
            newModel++;
        }
        model = (DistanceFieldModel)newModel;
    }

    if (preview)
    {
        Vector3 normal;
        for (int i = 0; i < 3000; i++)
        {
            float x = Random.Range(-10f, 10f);
            float y = Random.Range(-10f, 10f);
            float z = Random.Range(-10f, 10f);
            float dist = GetDistance(x, y, z, out normal);
            if (dist < 0f)
            {
                Debug.DrawRay(new Vector3(x, y, z), Vector3.up * .1f, Color.red, 1f);
            }
        }
    }
}
*/

//public class DistanceFieldSystem : JobComponentSystem
public class DistanceFieldSystem : JobComponentSystem
{
    static float SmoothMin(float a, float b, float radius)
    {
        float e = math.max(radius - math.abs(a - b), 0);
        return math.min(a, b) - e * e * 0.25f / radius;
    }

    static float Sphere(float3 pos, float radius)
    {
        return math.dot(pos, pos) - radius;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        var rng = new Random();
        var props = GetSingleton<EmitterPropetiers>();

        rng.InitState();

        var jobHandle = Entities
            //.WithSharedComponentFilter(props)
            .ForEach((ref ParticlePosition position, ref ParticleVelocity velocity, ref LocalToWorld localToWorld, ref ParticleColor color) =>
            {
                float sphereDist = Sphere(position.value, 5f);
                float3 sphereNormal = math.normalize(position.value);

                float planeDist = position.value.y;
                float3 planeNormal = new float3(0f, 1f, 0f);

                float t = math.sin(deltaTime * 8f) * .4f + .4f;
                float dist = math.lerp(sphereDist, planeDist, t);
                float3 normal = math.normalize(math.lerp(sphereNormal, planeNormal, t));

                //float dist = DistanceField.GetDistance(pos.position.x, orbiter.position.y, orbiter.position.z, out normal);
                velocity.value -= normal * props.Attraction * math.clamp(dist, -1f, 1f);
                velocity.value += rng.NextFloat3Direction() * props.Jitter;
                //velocity.value *= .99f;
                position.value += velocity.value;
                float4 targetColor;

                dist = math.abs(dist);
                targetColor = math.lerp(props.SurfaceColor, props.ExteriorColor, dist / props.ExteriorColorDist);

                color.value = math.lerp(color.value, targetColor, deltaTime * props.ColorStiffness);

                localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(
                     new float3(localToWorld.Position + velocity.value),
                     quaternion.LookRotationSafe(velocity.value, math.up()),
                     new float3(.1f, .01f, math.max(.1f, math.length(velocity.value) * props.SpeedStretch)))
                };

            }).Schedule(inputDeps);

        return jobHandle;
    }
}
