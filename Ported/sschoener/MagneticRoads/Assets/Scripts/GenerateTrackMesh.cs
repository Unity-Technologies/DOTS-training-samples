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
    public NativeArray<int> OutTriangles;

    [ReadOnly]
    public NativeList<TrackSpline> TrackSplines;

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

        var g = TrackSplines[index].Geometry;
        var b = TrackSplines[index].Bezier;
        byte twistMode = TrackSplines[index].TwistMode;

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

                var coord = TrackUtils.Extrude(b, g, twistMode, t);
                OutVertices[vertexIndex + 0] = coord.Base + coord.Right * p1.x + coord.Up * p1.y;
                OutVertices[vertexIndex + 1] = coord.Base + coord.Right * p2.x + coord.Up * p2.y;

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