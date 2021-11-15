using UnityEngine;

public class Tornado : MonoBehaviour
{
    public Transform cam;
    public Mesh particleMesh;
    public Material particleMaterial;

    public bool active = false;
    public bool simulate = true;

    public float spinRate;
    public float upwardSpeed;
    public float initRange = 10f;

    [Range(0f, 1f)] public float force = 0.022f;
    public float maxForceDist = 30f;
    public float height = 50f;
    public float upForce = 1.4f;
    public float inwardForce = 9;

    public float x => transform.position.x;
    public float y => transform.position.z;

    Vector3[] points;
    Matrix4x4[] matrices;
    MaterialPropertyBlock matProps;
    float[] radiusMults;

    private void Start()
    {
        points = new Vector3[1000];
        matrices = new Matrix4x4[1000];
        radiusMults = new float[1000];
        var colors = new Vector4[1000];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3(Random.Range(-initRange, initRange), Random.Range(0f, height), Random.Range(-initRange, initRange));
            matrices[i] = Matrix4x4.TRS(points[i], Quaternion.identity, Vector3.one * Random.Range(.2f, .7f));
            radiusMults[i] = Random.value;
            colors[i] = Color.white * Random.Range(.3f, .7f);
        }
        SimulateFrame();
        matProps = new MaterialPropertyBlock();
        matProps.SetVectorArray("_Color", colors);
    }

    void SimulateFrame()
    {
        for (int i = 0; i < points.Length; i++)
        {
            var tornadoPos = new Vector3(x + PointManager.TornadoSway(points[i].y), points[i].y, y);
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

    void FixedUpdate()
    {
        if (!simulate || !active)
            return;

        transform.position = new Vector3(Mathf.Cos(Time.time / 6f) * maxForceDist, transform.position.y, Mathf.Sin(Time.time / 6f * 1.618f) * maxForceDist);

        SimulateFrame();

        cam.position = new Vector3(transform.position.x, 10f, transform.position.z) - cam.forward * 60f;
    }

    internal void Update()
    {
        Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, matrices, matrices.Length, matProps);
    }
}
