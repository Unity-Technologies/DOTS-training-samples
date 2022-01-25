using Onboarding.BezierPath;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplineMesh : MonoBehaviour
{
    public PathData pathData;
    public float segmentLength = 10;
    public float sectionWidth = 1;
    public bool looped;

    public Material materialToAssign;
    
    [Range(3, 16)]
    public int sectionVertexCount = 3;

    private void GenerateMesh( float startdistance, float enddistance, bool isLooping)
    {
        float length = (enddistance - startdistance);
        int segmentCount = Mathf.CeilToInt(length / segmentLength);
        float realSegmentLength = length / segmentCount;
        PathController.LookupCache Cache = new PathController.LookupCache();

        int sectionCount = segmentCount + 1;
        Vector3[] sectionShapeVerts = new Vector3[sectionVertexCount];

        // compute section 'circular shape' in plance XY
        for (int i = 0; i < sectionVertexCount; ++i)
        {
            float angle = Mathf.PI * 2 * i / sectionVertexCount;
            sectionShapeVerts[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * sectionWidth;
        }

        // First section and last section will have the same physical location but different UVs
        // if we don't loop the Bezier we need 2 more vertices to cap both ends
        int vCount = sectionVertexCount * sectionCount + (isLooping ? 0 : 2);
        Vector3[] splineMeshVerts = new Vector3[vCount];
        Vector2[] splineMeshUVs = new Vector2[vCount];
        int splineVertexIndex = 0;
        int lastSectionStartIndex = 0;

        // Create all vertices
        for (int i = 0; i < sectionCount; ++i)
        {
            bool isLastSection = (i == sectionCount - 1);

            float distanceFromSpline;
            if (!isLastSection)
            {
                distanceFromSpline = startdistance + Mathf.Min(i * realSegmentLength, length);
            }
            else
            {
                distanceFromSpline = isLooping ? startdistance : enddistance;
                lastSectionStartIndex = splineVertexIndex;
            }

            PathController.InterpolatePositionAndDirection(pathData, Cache, distanceFromSpline, out var pos, out var dir);
            //PathController.InterpolatePosition(pathData, 0, distanceFromSpline / length, out var pos);
            //PathController.InterpolateDirection(pathData, 0, distanceFromSpline / length, out var dir);

            Matrix4x4 mat = Matrix4x4.TRS(pos, Quaternion.LookRotation(dir), Vector3.one);
            float uv_v = distanceFromSpline / pathData.PathLength;
            for (int sectionVertexIndex = 0; sectionVertexIndex < sectionVertexCount; ++sectionVertexIndex)
            {
                splineMeshVerts[splineVertexIndex] = mat.MultiplyPoint(sectionShapeVerts[sectionVertexIndex]);
                splineMeshUVs[splineVertexIndex] = new Vector2((float)sectionVertexIndex / (sectionVertexCount - 1), uv_v);
                splineVertexIndex++;
            }
        }

        if (!isLooping)
        {
            PathController.InterpolatePosition(pathData, Cache, startdistance, out var origin);
            PathController.InterpolatePosition(pathData, Cache, enddistance, out var end);
            splineMeshVerts[splineVertexIndex] = origin;
            splineMeshUVs[splineVertexIndex] = new Vector2(0.5f, 0);
            splineVertexIndex++;
            splineMeshVerts[splineVertexIndex] = end;
            splineMeshUVs[splineVertexIndex] = new Vector2(0.5f, 1);
            splineVertexIndex++;
        }
        
        // Create all triangles
        int quadsCount = sectionVertexCount * segmentCount;
        int triCount = quadsCount * 2;

        int capTris = isLooping ? 0 : sectionVertexCount;
        triCount += capTris * 2;

        int[] triangleList = new int[triCount * 3];
        int triListIndex = 0;
        for (int i = 0; i < segmentCount; ++i)
        {
            int sourceSection = i;
            int destinationSection = i + 1;
            int sourceStartVertex = sourceSection * sectionVertexCount;
            int destinationStartVertex = destinationSection * sectionVertexCount;

            // Create quads
            for (int q = 0; q < sectionVertexCount; ++q)
            {
                int next_q = (q + 1) % sectionVertexCount;

                // First tri
                triangleList[triListIndex++] = sourceStartVertex + q;
                triangleList[triListIndex++] = destinationStartVertex + next_q;
                triangleList[triListIndex++] = destinationStartVertex + q;
                // Second tri
                triangleList[triListIndex++] = sourceStartVertex + q;
                triangleList[triListIndex++] = sourceStartVertex + next_q;
                triangleList[triListIndex++] = destinationStartVertex + next_q;
            }
        }

        if (!isLooping)
        {
            int splineStartVertexIndex = splineVertexIndex - 2;
            int splineEndVertexIndex = splineVertexIndex - 1;

            // Start cap
            for (int q = 0; q < sectionVertexCount; ++q)
            {
                triangleList[triListIndex++] = splineStartVertexIndex;
                triangleList[triListIndex++] = (q + 1) % sectionVertexCount;
                triangleList[triListIndex++] = q;
            }
            // End cap
            for (int q = 0; q < sectionVertexCount; ++q)
            {
                triangleList[triListIndex++] = splineEndVertexIndex;
                triangleList[triListIndex++] = lastSectionStartIndex + q;
                triangleList[triListIndex++] = lastSectionStartIndex + ((q + 1) % sectionVertexCount);
            }
        }

        // Create Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = splineMeshVerts;
        mesh.uv = splineMeshUVs;
        mesh.triangles = triangleList;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GameObject go = new GameObject();
        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = materialToAssign;
    }

    public void GenerateMesh()
    {
        GenerateMesh(0, pathData.PathLength, looped);
    }

    public void GenerateSplitMeshesPerCurve()
    {
        int splines = (pathData.m_BezierControlPoints.Length - 1) / 3;
        for (int i = 0; i < splines; ++i)
        {
            float start = pathData.m_DistanceToParametric.First(s => s.bezierIndex == i).start;
            float end = pathData.m_DistanceToParametric.Last(s => s.bezierIndex == i).end;
            GenerateMesh(start, end, false);
        }
    }
    public void DisplayCurvesLength()
    {
        int splines = (pathData.m_BezierControlPoints.Length - 1) / 3;
        for (int i = 0; i < splines; ++i)
        {
            float start = pathData.m_DistanceToParametric.First(s => s.bezierIndex == i).start;
            float end = pathData.m_DistanceToParametric.Last(s => s.bezierIndex == i).end;
            Debug.Log($"Spline {i} is {end- start} m");
        }
        Debug.Log($"Total :  {pathData.PathLength} m");
    }

    public void DebugLookup(float dist)
    {
        var fittingCurve = pathData.m_DistanceToParametric.First(s => s.start <= dist && s.end >= dist);
        
        var index = pathData.m_DistanceToParametric.ToList().IndexOf(fittingCurve);
        Debug.Log($"Entry #{index} contains {dist}, range [{fittingCurve.start},{fittingCurve.end}]");
        var s = Mathf.InverseLerp(fittingCurve.start, fittingCurve.end, dist);
        var t = fittingCurve.p0 + s * (fittingCurve.p1 + s * (fittingCurve.p2 + fittingCurve.p3 * s));
        Debug.Log($"s = {s}, t = {t}");
        int lastUsedCurveIndex = 0;
        PathController.InterpolatePosition(pathData, ref lastUsedCurveIndex, dist, out var position);
        Debug.Log($"position = {position}");
    }

}

