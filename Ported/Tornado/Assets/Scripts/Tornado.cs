using UnityEngine;

public class Tornado : MonoBehaviour
{
    public Transform cam;
    public Mesh particleMesh;
    public Material particleMaterial;

    public bool simulate = true;
    public float speed = 0.5f;
    public float spinRate;
    public float upwardSpeed;
    public float initRange = 10f;

    [Range(0f, 1f)] public float force = 0.022f;
    public float maxForceDist = 32f;
    public float height = 50f;
    public float upForce = 1.4f;
    public float inwardForce = 9;
    public float fader = 0f;

    public float x => transform.position.x;
    public float y => transform.position.z;

    Vector3 initialPosition;
    Vector3[] points;
    Matrix4x4[] matrices;
    MaterialPropertyBlock matProps;
    float[] radiusMults;
    float rotationModulation;
    MeshRenderer ground;

    internal void Start()
    {
        int pointCount = System.Math.Min(Mathf.RoundToInt(force * 20000), 1023);
        var colors = new Vector4[pointCount];
        points = new Vector3[pointCount];
        matrices = new Matrix4x4[pointCount];
        radiusMults = new float[pointCount];

        ground = FindObjectOfType<Ground>().gameObject.GetComponent<MeshRenderer>();
        initialPosition = transform.position;
        rotationModulation = ground.bounds.extents.magnitude / 2f * Random.Range(-0.9f, 0.9f);

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3(Random.Range(-initRange, initRange), Random.Range(0f, height), Random.Range(-initRange, initRange));
            matrices[i] = Matrix4x4.TRS(points[i], Quaternion.identity, Vector3.one * Random.Range(.2f, .7f));
            radiusMults[i] = Random.value;
            colors[i] = Color.white * Random.Range(.3f, .7f);
        }

        matProps = new MaterialPropertyBlock();
        matProps.SetVectorArray("_Color", colors);
    }

    public static float Sway(float y)
    {
        return Mathf.Sin(y / 5f + Time.time / 4f) * 3f;
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

    internal void FixedUpdate()
    {
        if (!simulate)
            return;

        var tmod = Time.time / 6f * speed;
        fader = Mathf.Clamp01(fader + Time.deltaTime / 10f);
        transform.position = new Vector3(initialPosition.x + Mathf.Cos(tmod) * rotationModulation, transform.position.y, initialPosition.z + Mathf.Sin(tmod * 1.618f) * rotationModulation);

        Advance();

        if (cam != null)
            cam.position = new Vector3(transform.position.x, 10f, transform.position.z) - cam.forward * 60f;
    }

    internal void Update()
    {
        Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, matrices, matrices.Length, matProps);
    }
}
