using UnityEngine;

class Bar
{
    public Point point1;
    public Point point2;
    public Vector3 delta;

    public Vector3 min;
    public Vector3 max;
    public Matrix4x4 matrix;

    public readonly Color color;
    public readonly float thickness;
    public readonly float length;

    public Bar(in Point a, in Point b)
    {
        point1 = a;
        point2 = b;

        delta = point2.position - point1.position;
        length = delta.magnitude;

        thickness = Random.Range(.25f, .35f);

        var pos = (point1.position + point2.position) * .5f;
        var rot = Quaternion.LookRotation(delta);
        var scale = new Vector3(thickness, thickness, length);
        matrix = Matrix4x4.TRS(pos, rot, scale);

        min = new Vector3(Mathf.Min(point1.x, point2.x), Mathf.Min(point1.y, point2.y), Mathf.Min(point1.z, point2.z));
        max = new Vector3(Mathf.Max(point1.x, point2.x), Mathf.Max(point1.y, point2.y), Mathf.Max(point1.z, point2.z));

        float upDot = Mathf.Acos(Mathf.Abs(Vector3.Dot(Vector3.up, delta.normalized))) / Mathf.PI;
        color = Color.white * upDot * Random.Range(.7f, 1f);
    }
}
