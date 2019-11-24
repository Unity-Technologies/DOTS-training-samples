using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public struct Bar
{
    public int point1;
    public int point2;
    public float length;
    public Matrix4x4 matrix;
    public float3 oldDelta;
    public Color color;
    public float thickness;

    public void AssignPoints(int p1, float3 pos1, int p2, float3 pos2)
    {
        point1 = p1;
        point2 = p2;
        var delta = pos1 - pos2;
        length = math.length(delta);

        thickness = Random.Range(.25f, .35f);

        var pos = (pos1 + pos2) / 2;
        var rot = quaternion.LookRotation(delta, new float3(0, 1, 0));
        var scale = new float3(thickness, thickness, length);
        matrix = float4x4.TRS(pos, rot, scale);

        var proj = math.dot(new float3(0, 1, 0), delta / length);
        float upDot = math.acos(math.abs(proj)) / math.PI;
        color = Color.white * upDot * Random.Range(.7f, 1f);
    }
}
