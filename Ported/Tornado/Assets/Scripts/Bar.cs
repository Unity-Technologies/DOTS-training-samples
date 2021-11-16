using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct Beam
{
    public readonly int point1;
    public readonly int point2;

    public readonly float thickness;

    public Beam(int point1, int point2)
    {
        this.point1 = point1;
        this.point2 = point2;
        thickness = Random.Range(.25f, .35f);
    }
}

struct Bars
{
    public Beam[] beams;
    public Vector3[] delta;
    public float[] length;
    public Matrix4x4[] matrix;

    public int count => beams.Length;

    public Bars(IList<Beam> beams)
    {
        this.beams = beams.ToArray();
        this.delta = new Vector3[beams.Count];
        this.length = new float[beams.Count];
        this.matrix = new Matrix4x4[beams.Count];
    }
}
