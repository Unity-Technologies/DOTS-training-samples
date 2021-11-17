#define DEBUG_DOTS
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class Tornado : MonoBehaviour, IConvertGameObjectToEntity
{
    public Transform cam;
    public Mesh particleMesh;
    public Material particleMaterial;

    public bool simulate = true;
    [Range(0f, 2f)] public float speed = 0.5f;
    [Range(0f, 99f)] public float spinRate;
    [Range(0f, 99f)] public float upwardSpeed;
    [Range(2f, 30f)] public float initRange = 10f;

    [Range(0f, 1f)] public float force = 0.022f;
    [Range(2f, 99f)] public float maxForceDist = 32f;
    [Range(1f, 100f)] public float height = 50f;
    [Range(0f, 10f)] public float upForce = 1.4f;
    [Range(0f, 100f)] public float inwardForce = 9;

    internal float fader = 0f;
    internal float rotationModulation;
    Vector3 initialPosition;
    Vector3[] points;
    Matrix4x4[] matrices;
    MaterialPropertyBlock matProps;
    float[] radiusMults;
    MeshRenderer ground;
    Entity entity;

    public float x => transform.position.x;
    public float y => transform.position.z;

    internal void Start()
    {
        #if DEBUG_DOTS
        using (new DebugTimer("Generate Tornado", 500d))
        #endif
        { 
            Generate();
        }
    }

    void Generate()
    {
        var random = new Unity.Mathematics.Random(1234);
        int pointCount = System.Math.Min(Mathf.RoundToInt(force * 20000), 1023);
        var colors = new Vector4[pointCount];
        points = new Vector3[pointCount];
        matrices = new Matrix4x4[pointCount];
        radiusMults = new float[pointCount];

        ground = GetComponentInParent<MeshRenderer>();
        initialPosition = transform.position;
        rotationModulation = ground.bounds.extents.magnitude / 2f * random.NextFloat(-0.9f, 0.9f);

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3(random.NextFloat(-initRange, initRange), random.NextFloat(0f, height), random.NextFloat(-initRange, initRange));
            matrices[i] = Matrix4x4.TRS(points[i], Quaternion.identity, Vector3.one * random.NextFloat(.2f, .7f));
            radiusMults[i] = random.NextFloat();
            colors[i] = Color.white * random.NextFloat(.3f, .7f);
        }

        matProps = new MaterialPropertyBlock();
        matProps.SetVectorArray("_Color", colors);
    }

    public static float Sway(float y)
    {
        return Mathf.Sin(y / 5f + Time.time / 4f) * 3f;
    }

    internal void FixedUpdate()
    {
        if (!simulate)
            return;

        #if DEBUG_DOTS
        using (new DebugTimer("Simulate Tornado", 2d))
        #endif
        { 
            Simulate();
        }
    }

    internal void Update()
    {
        #if DEBUG_DOTS
        using (new DebugTimer("Render Tornado", 1d))
        #endif
        {
            Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, matrices, matrices.Length, matProps);
        }
    }

    void Simulate()
    {
        var tmod = Time.time / 6f * speed;
        fader = Mathf.Clamp01(fader + Time.deltaTime / 10f);
        transform.position = new Vector3(initialPosition.x + Mathf.Cos(tmod) * rotationModulation, transform.position.y, initialPosition.z + Mathf.Sin(tmod * 1.618f) * rotationModulation);

        Advance();

        if (cam != null)
            cam.position = new Vector3(transform.position.x, 10f, transform.position.z) - cam.forward * 60f;

        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        em.SetComponentData(entity, new TornadoFader { fader = fader });
        em.SetComponentData(entity, new Translation { Value = transform.position });
    }

    void Advance()
    {
        for (int i = 0; i < points.Length; i++)
        {
            var tornadoPos = new Vector3(x + Sway(points[i].y), points[i].y, y);
            var delta = tornadoPos - points[i];
            var dist = delta.magnitude;
            float inForce = dist - Mathf.Clamp01(tornadoPos.y / height) * maxForceDist * radiusMults[i] + 2f;

            delta /= dist;
            points[i] += new Vector3(-delta.z * spinRate + delta.x * inForce, upwardSpeed, delta.x * spinRate + delta.z * inForce) * Time.deltaTime;
            if (points[i].y > height)
                points[i] = new Vector3(points[i].x, 0f, points[i].z);

            Matrix4x4 matrix = matrices[i];
            matrix.m03 = points[i].x;
            matrix.m13 = points[i].y;
            matrix.m23 = points[i].z;
            matrices[i] = matrix;
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        dstManager.AddComponentData(entity, new TornadoFader { fader = fader });
        dstManager.AddComponentData(entity, new TornadoData()
        {
            force = force,
            height = height,
            inwardForce = inwardForce,
            maxForceDist = maxForceDist,
            upForce = upForce
        });
    }
}
