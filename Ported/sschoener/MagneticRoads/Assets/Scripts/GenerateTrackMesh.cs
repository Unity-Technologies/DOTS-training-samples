using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;



[BurstCompile]
struct GenerateTrackMeshes : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> OutVertices;
    [NativeDisableParallelForRestriction]
    public NativeArray<float2> OutUVs;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> OutTriangles;

    public NativeArray<byte> OutTwistMode;
    [ReadOnly]
    public NativeArray<CubicBezier> Bezier;
    [ReadOnly]
    public NativeArray<TrackGeometry> Geometry;

    float2 m_LocalPoint1;
    float2 m_LocalPoint2;
    float2 m_LocalPoint3;
    float2 m_LocalPoint4;
    int m_Resolution;

    public int VerticesPerSpline;
    public int IndicesPerSpline;
    public int SplinesPerMesh;

    public void Setup(int resolution, float trackRadius, float trackThickness)
    {
        m_Resolution = resolution;
        m_LocalPoint1 = new float2(-trackRadius, trackThickness * .5f);
        m_LocalPoint2 = new float2(trackRadius, trackThickness * .5f);
        m_LocalPoint3 = new float2(-trackRadius, -trackThickness * .5f);
        m_LocalPoint4 = new float2(trackRadius, -trackThickness * .5f);
    }

    public void Execute(int index)
    {
        int vertexIndex = index * VerticesPerSpline;
        int triIndex = index * IndicesPerSpline;

        int mesh = index / SplinesPerMesh;
        int relativeVertexIndex = vertexIndex - (mesh * SplinesPerMesh * VerticesPerSpline);

        byte twistMode = TrackUtils.SelectTwistMode(Bezier[index], Geometry[index], m_Resolution);
        OutTwistMode[index] = twistMode;

        // extrude our rectangle as four strips
        for (int i = 0; i < 4; i++)
        {
            float2 p1, p2;
            switch (i)
            {
                case 0:
                    // top strip
                    p1 = m_LocalPoint1;
                    p2 = m_LocalPoint2;
                    break;
                case 1:
                    // right strip
                    p1 = m_LocalPoint2;
                    p2 = m_LocalPoint4;
                    break;
                case 2:
                    // bottom strip
                    p1 = m_LocalPoint4;
                    p2 = m_LocalPoint3;
                    break;
                default:
                    // left strip
                    p1 = m_LocalPoint3;
                    p2 = m_LocalPoint1;
                    break;
            }

            for (int j = 0; j <= m_Resolution; j++)
            {
                float t = (float)j / m_Resolution;

                OutVertices[vertexIndex + 0] = TrackUtils.Extrude(Bezier[index], Geometry[index], twistMode, p1, t);
                OutVertices[vertexIndex + 1] = TrackUtils.Extrude(Bezier[index], Geometry[index], twistMode, p2, t);
                OutUVs[vertexIndex + 0] = new float2(0, t);
                OutUVs[vertexIndex + 1] = new float2(1, t);

                if (j < m_Resolution)
                {
                    OutTriangles[triIndex++] = relativeVertexIndex + 0;
                    OutTriangles[triIndex++] = relativeVertexIndex + 1;
                    OutTriangles[triIndex++] = relativeVertexIndex + 2;

                    OutTriangles[triIndex++] = relativeVertexIndex + 1;
                    OutTriangles[triIndex++] = relativeVertexIndex + 3;
                    OutTriangles[triIndex++] = relativeVertexIndex + 2;
                }

                vertexIndex += 2;
                relativeVertexIndex += 2;
            }
        }
    }
}