using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public int voxelCount = 60;
    public float voxelSize = 1f;
    public int tickerSteps = 50000;
    public int trisPerMesh = 4000;
    public Material roadMaterial;
    public Mesh intersectionMesh;
    public Mesh intersectionPreviewMesh;

    public int numCars;
    public Mesh carMesh;
    public Material carMaterial;
    public float carSpeed = 2f;

    bool[,,] trackVoxels;
    List<Intersection> intersections;
    List<TrackSpline> trackSplines;
    Intersection[,,] intersectionsGrid;
    List<Car> cars;

    List<List<Matrix4x4>> carMatrices;

    Vector3Int[] dirs;
    Vector3Int[] fullDirs;

    public const float intersectionSize = .5f;
    public const float trackRadius = .2f;
    public const float trackThickness = .05f;
    public const int splineResolution = 20;
    public const float carSpacing = .13f;

    const int instancesPerBatch = 1023;


    // intersection pair:  two 32-bit IDs, packed together
    HashSet<long> intersectionPairs;

    List<Mesh> roadMeshes;
    List<List<Matrix4x4>> intersectionMatrices;

    MaterialPropertyBlock carMatProps;
    List<List<Vector4>> carColors;

    public GeneratedIntersectionDataObject intersectionDataObject;

    long HashIntersectionPair(Intersection a, Intersection b)
    {
        // pack two intersections' IDs into one int64
        int id1 = a.id;
        int id2 = b.id;

        return ((long) Mathf.Min(id1, id2) << 32) + Mathf.Max(id1, id2);
    }

    int Dot(Vector3Int a, Vector3Int b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    bool GetVoxel(Vector3Int pos, bool outOfBoundsValue = true)
    {
        return GetVoxel(pos.x, pos.y, pos.z, outOfBoundsValue);
    }

    bool GetVoxel(int x, int y, int z, bool outOfBoundsValue = true)
    {
        if (x >= 0 && x < voxelCount && y >= 0 && y < voxelCount && z >= 0 && z < voxelCount)
        {
            return trackVoxels[x, y, z];
        }
        else
        {
            return outOfBoundsValue;
        }
    }

    int CountNeighbors(Vector3Int pos, bool includeDiagonal = false)
    {
        return CountNeighbors(pos.x, pos.y, pos.z, includeDiagonal);
    }

    int CountNeighbors(int x, int y, int z, bool includeDiagonal = false)
    {
        int neighborCount = 0;

        Vector3Int[] dirList = dirs;
        if (includeDiagonal)
        {
            dirList = fullDirs;
        }

        for (int k = 0; k < dirList.Length; k++)
        {
            Vector3Int dir = dirList[k];
            if (GetVoxel(x + dir.x, y + dir.y, z + dir.z))
            {
                neighborCount++;
            }
        }

        return neighborCount;
    }

    Intersection FindFirstIntersection(Vector3Int pos, Vector3Int dir, out Vector3Int otherDirection)
    {
        // step along our voxel paths (before splines have been spawned),
        // starting at one intersection, and stopping when we reach another intersection
        while (true)
        {
            pos += dir;
            if (intersectionsGrid[pos.x, pos.y, pos.z] != null)
            {
                otherDirection = dir * -1;
                return intersectionsGrid[pos.x, pos.y, pos.z];
            }

            if (GetVoxel(pos + dir, false) == false)
            {
                bool foundTurn = false;
                for (int i = 0; i < dirs.Length; i++)
                {
                    if (dirs[i] != dir && dirs[i] != (dir * -1))
                    {
                        if (GetVoxel(pos + dirs[i], false) == true)
                        {
                            dir = dirs[i];
                            foundTurn = true;
                            break;
                        }
                    }
                }

                if (foundTurn == false)
                {
                    // dead end
                    otherDirection = Vector3Int.zero;
                    return null;
                }
            }
        }
    }

    void Start()
    {
        // cardinal directions:
        dirs = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)

            //new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, 0)
        };

        // cardinal directions + diagonals in 3D:
        fullDirs = new Vector3Int[26];
        int dirIndex = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x != 0 || y != 0 || z != 0)
                    {
                        fullDirs[dirIndex] = new Vector3Int(x, y, z);
                        dirIndex++;
                    }
                }
            }
        }

        StartCoroutine(SpawnRoads());
    }

    IEnumerator SpawnRoads()
    {
        // first generation pass: plan roads as basic voxel data only
        trackVoxels = new bool[voxelCount, voxelCount, voxelCount];
        List<Vector3Int> activeVoxels = new List<Vector3Int>();
        trackVoxels[voxelCount / 2, voxelCount / 2, voxelCount / 2] = true;
        activeVoxels.Add(new Vector3Int(voxelCount / 2, voxelCount / 2, voxelCount / 2));

        // after voxel generation, we'll convert our network into non-voxels
        intersections = new List<Intersection>();
        intersectionsGrid = new Intersection[voxelCount, voxelCount, voxelCount];
        intersectionPairs = new HashSet<long>();
        trackSplines = new List<TrackSpline>();
        roadMeshes = new List<Mesh>();
        intersectionMatrices = new List<List<Matrix4x4>>();

        cars = new List<Car>();
        carMatrices = new List<List<Matrix4x4>>();
        carMatrices.Add(new List<Matrix4x4>());
        carColors = new List<List<Vector4>>();
        carColors.Add(new List<Vector4>());
        carMatProps = new MaterialPropertyBlock();
        carMatProps.SetVectorArray("_Color", new Vector4[instancesPerBatch]);


        // plan roads broadly: first, as a grid of true/false voxels
        int ticker = 0;
        while (activeVoxels.Count > 0 && ticker < tickerSteps)
        {
            ticker++;
            int index = Random.Range(0, activeVoxels.Count);
            Vector3Int pos = activeVoxels[index];
            Vector3Int dir = dirs[Random.Range(0, dirs.Length)];
            Vector3Int pos2 = new Vector3Int(pos.x + dir.x, pos.y + dir.y, pos.z + dir.z);
            if (GetVoxel(pos2) == false)
            {
                // when placing a new voxel, it must have fewer than three
                // diagonal-or-cardinal Neighbors.
                // (this blocks nonplanar intersections from forming)
                if (CountNeighbors(pos2, true) < 3)
                {
                    activeVoxels.Add(pos2);
                    trackVoxels[pos2.x, pos2.y, pos2.z] = true;
                }
            }

            int neighborCount = CountNeighbors(pos);
            if (neighborCount >= 3)
            {
                // no more than three cardinal Neighbors for any voxel (no 4-way intersections allowed)
                // (really, this is to avoid nonplanar intersections)
                Intersection intersection = new Intersection(pos, (Vector3) pos * voxelSize, Vector3Int.zero);
                intersection.id = intersections.Count;
                intersections.Add(intersection);
                intersectionsGrid[pos.x, pos.y, pos.z] = intersection;
                activeVoxels.RemoveAt(index);
            }

            if (ticker % 1000 == 0)
            {
                yield return null;
            }
        }

        Debug.Log(intersections.Count + " intersections");

        // at this point, we've generated our full layout, but everything
        // is voxels, and we've identified which voxels are intersections.
        // next, we'll reinterpret our voxels as a network of intersections:
        // we'll find all "neighboring intersections" in our voxel map
        // (neighboring intersections are connected by a chain of voxels,
        // which we'll replace with splines)

        // RICARDO: Create all intersection objects
        intersectionDataObject.intersections = new List<GeneratedIntersectionData>();
        intersectionDataObject.splines = new List<GeneratedSplineData>();

        for (int i = 0; i < intersections.Count; i++)
        {
            GeneratedIntersectionData intersectionData = new GeneratedIntersectionData();
            intersectionData.id =
                intersectionDataObject.intersections.Count; // Should be equal to i && to intersection.id
            intersectionDataObject.intersections.Add(intersectionData);
        }


        for (int i = 0; i < intersections.Count; i++)
        {
            Intersection intersection = intersections[i];
            Vector3Int axesWithNeighbors = Vector3Int.zero;
            for (int j = 0; j < dirs.Length; j++)
            {
                if (GetVoxel(intersection.index + dirs[j], false))
                {
                    axesWithNeighbors.x += Mathf.Abs(dirs[j].x);
                    axesWithNeighbors.y += Mathf.Abs(dirs[j].y);
                    axesWithNeighbors.z += Mathf.Abs(dirs[j].z);

                    Vector3Int connectDir;
                    Intersection neighbor = FindFirstIntersection(intersection.index, dirs[j], out connectDir);
                    if (neighbor != null && neighbor != intersection)
                    {
                        // make sure we haven't already added the reverse-version of this spline
                        long hash = HashIntersectionPair(intersection, neighbor);
                        if (intersectionPairs.Contains(hash) == false)
                        {
                            intersectionPairs.Add(hash);

                            TrackSpline spline = new TrackSpline(intersection, dirs[j], neighbor, connectDir);
                            trackSplines.Add(spline);

                            intersection.neighbors.Add(neighbor);
                            intersection.neighborSplines.Add(spline);
                            neighbor.neighbors.Add(intersection);
                            neighbor.neighborSplines.Add(spline);

                            // RICARDO
                            // Create the forward direction spline
                            GeneratedSplineData splineDataForwardDirection = new GeneratedSplineData();
                            splineDataForwardDirection.id = intersectionDataObject.splines.Count;

                            splineDataForwardDirection.startIntersectionId = intersection.id;
                            splineDataForwardDirection.endIntersectionId = neighbor.id;
                            splineDataForwardDirection.startPoint = spline.startPoint;
                            splineDataForwardDirection.endPoint = spline.endPoint;
                            splineDataForwardDirection.anchor1 = spline.anchor1;
                            splineDataForwardDirection.anchor2 = spline.anchor2;
                            splineDataForwardDirection.startNormal = spline.startNormal;
                            splineDataForwardDirection.endNormal = spline.endNormal;
                            splineDataForwardDirection.startTangent = spline.startTangent;
                            splineDataForwardDirection.endTangent = spline.endTangent;
                            splineDataForwardDirection.measuredLength = spline.measuredLength;
                            splineDataForwardDirection.carQueueSize = spline.carQueueSize;
                            splineDataForwardDirection.maxCarCount = spline.maxCarCount;

                            intersectionDataObject.splines.Add(
                                splineDataForwardDirection); // Add the spline to the DB of splines

                            // Reverse the forward direction spline
                            GeneratedSplineData splineDataReverseDirection = new GeneratedSplineData();
                            splineDataReverseDirection.id = intersectionDataObject.splines.Count;

                            splineDataReverseDirection.startIntersectionId = neighbor.id;
                            splineDataReverseDirection.endIntersectionId = intersection.id;
                            splineDataReverseDirection.startPoint = spline.endPoint;
                            splineDataReverseDirection.endPoint = spline.startPoint;
                            splineDataReverseDirection.anchor1 = spline.anchor2;
                            splineDataReverseDirection.anchor2 = spline.anchor1;
                            splineDataReverseDirection.startNormal = spline.endNormal;
                            splineDataReverseDirection.endNormal = spline.startNormal;
                            splineDataReverseDirection.startTangent = spline.endTangent;
                            splineDataReverseDirection.endTangent = spline.startTangent;
                            splineDataReverseDirection.measuredLength = spline.measuredLength;
                            splineDataReverseDirection.carQueueSize = spline.carQueueSize;
                            splineDataReverseDirection.maxCarCount = spline.maxCarCount;

                            intersectionDataObject.splines.Add(splineDataReverseDirection);

                            // Reference the spline from the intersection data
                            GeneratedIntersectionData startIntersectionData =
                                intersectionDataObject.intersections[intersection.id];
                            if (startIntersectionData.splineCount == 0)
                                startIntersectionData.splineData1 = splineDataForwardDirection.id;
                            if (startIntersectionData.splineCount == 1)
                                startIntersectionData.splineData2 = splineDataForwardDirection.id;
                            if (startIntersectionData.splineCount == 2)
                                startIntersectionData.splineData3 = splineDataForwardDirection.id;
                            startIntersectionData.splineCount++;
                            intersectionDataObject.intersections[intersection.id] = startIntersectionData;

                            GeneratedIntersectionData endIntersectionData =
                                intersectionDataObject.intersections[neighbor.id];
                            if (endIntersectionData.splineCount == 0)
                                endIntersectionData.splineData1 = splineDataReverseDirection.id;
                            if (endIntersectionData.splineCount == 1)
                                endIntersectionData.splineData2 = splineDataReverseDirection.id;
                            if (endIntersectionData.splineCount == 2)
                                endIntersectionData.splineData3 = splineDataReverseDirection.id;
                            endIntersectionData.splineCount++;
                            intersectionDataObject.intersections[neighbor.id] = endIntersectionData;
                        }
                    }
                }
            }

            // find this intersection's normal - it's the one axis
            // along which we have no Neighbors
            for (int j = 0; j < 3; j++)
            {
                if (axesWithNeighbors[j] == 0)
                {
                    if (intersection.normal == Vector3Int.zero)
                    {
                        intersection.normal = new Vector3Int();
                        intersection.normal[j] = -1 + Random.Range(0, 2) * 2;
                        //Debug.DrawRay(intersection.Position,(Vector3)intersection.normal * .5f,Color.red,1000f);
                    }
                    else
                    {
                        Debug.LogError("a straight line has been marked as an intersection!");
                    }
                }
            }

            if (intersection.normal == Vector3Int.zero)
            {
                Debug.LogError("nonplanar intersections are not allowed!");
            }

            // NOTE - if you investigate the above logic, you might be confused about how
            // dead-ends are given normals, since we're assuming that all intersections
            // have two axes with Neighbors and only one axis without. dead-ends only have
            // one neighbor-axis...and somehow they still get a normal without a special case.
            //
            // the "gotcha" is that the visible dead-ends in the demo have three
            // Neighbors during the voxel phase, with two of their neighbor chains leading
            // to nothing. these "hanging chains" are not included as splines, so the
            // dead-ends that we see are actually "T" shapes with the top two segments hidden.

            GeneratedIntersectionData intersectionData = intersectionDataObject.intersections[intersection.id];
            intersectionData.normal = intersection.normal;
            intersectionData.position = intersection.position;
            intersectionDataObject.intersections[intersectionData.id] = intersectionData;

            if (i % 20 == 0)
            {
                yield return null;
            }
        }

        //for (int i = 0; i < intersectionDataObject.splines.Count; i++)
        //{
        //    for (int j = 0; j < intersectionDataObject.intersections.Count; j++)
        //    {
        //        if (intersectionDataObject.splines[i].startIntersectionId == intersectionDataObject.intersections[j].id)
        //        {
        //            GeneratedSplineData data = intersectionDataObject.splines[i];
        //            data.startNormal = new Vector3Int((int)intersectionDataObject.intersections[j].normal.x, (int)intersectionDataObject.intersections[j].normal.y, (int)intersectionDataObject.intersections[j].normal.z);
        //            intersectionDataObject.splines[i] = data;
        //        }

        //        if (intersectionDataObject.splines[i].endIntersectionId == intersectionDataObject.intersections[j].id)
        //        {
        //            GeneratedSplineData data = intersectionDataObject.splines[i];
        //            data.endNormal = new Vector3Int((int)intersectionDataObject.intersections[j].normal.x, (int)intersectionDataObject.intersections[j].normal.y, (int)intersectionDataObject.intersections[j].normal.z);
        //            intersectionDataObject.splines[i] = data;
        //        }
        //    }
        //}

        Debug.Log(trackSplines.Count + " road splines");


        // generate road meshes

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int triCount = 0;

        for (int i = 0; i < trackSplines.Count; i++)
        {
            trackSplines[i].GenerateMesh(vertices, uvs, triangles);

            if (triangles.Count / 3 > trisPerMesh || i == trackSplines.Count - 1)
            {
                // our current mesh data is ready to go!
                if (triangles.Count > 0)
                {
                    Mesh mesh = new Mesh();
                    mesh.name = "Generated Road Mesh";
                    mesh.SetVertices(vertices);
                    mesh.SetUVs(0, uvs);
                    mesh.SetTriangles(triangles, 0);
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();
                    roadMeshes.Add(mesh);
                    triCount += triangles.Count / 3;

//                    RoadMesh = mesh;
                }

                vertices.Clear();
                uvs.Clear();
                triangles.Clear();
            }

            if (i % 10 == 0)
            {
                yield return null;
            }
        }

        // generate intersection matrices for batch-rendering
        int batch = 0;
        intersectionMatrices.Add(new List<Matrix4x4>());
        for (int i = 0; i < intersections.Count; i++)
        {
            intersectionMatrices[batch].Add(intersections[i].GetMatrix());
            if (intersectionMatrices[batch].Count == instancesPerBatch)
            {
                batch++;
                intersectionMatrices.Add(new List<Matrix4x4>());
            }
        }

        Debug.Log(triCount + " road triangles (" + roadMeshes.Count + " meshes)");


        // spawn cars

        batch = 0;
        for (int i = 0; i < numCars; i++)
        {
            Car car = new Car();
            car.maxSpeed = carSpeed;
            car.roadSpline = trackSplines[Random.Range(0, trackSplines.Count)];
            car.splineTimer = 1f;
            car.splineDirection = -1 + Random.Range(0, 2) * 2;
            car.splineSide = -1 + Random.Range(0, 2) * 2;

            car.roadSpline.GetQueue(car.splineDirection, car.splineSide).Add(car);

            cars.Add(car);
            carMatrices[batch].Add(Matrix4x4.identity);
            carColors[batch].Add(Random.ColorHSV());
            if (carMatrices[batch].Count == instancesPerBatch)
            {
                carMatrices.Add(new List<Matrix4x4>());
                carColors.Add(new List<Vector4>());
                batch++;
            }
        }

        //Save Meshes

        if (SaveMeshes)
        {
            var roads = new GameObject("Roads", typeof(ConvertToEntity));
            roads.GetComponent<ConvertToEntity>().ConversionMode = ConvertToEntity.Mode.ConvertAndDestroy;

            var meshFilter = roads.AddComponent<MeshFilter>();
            var meshRenderer = roads.AddComponent<MeshRenderer>();

            var combinedRoadInstances = new List<CombineInstance>();

            for (int i = 0; i < roadMeshes.Count; i++)
            {
                var ci = new CombineInstance { mesh = roadMeshes[i], transform = Matrix4x4.identity};
                combinedRoadInstances.Add(ci);
            }
            
            var roadMesh = new Mesh();
            roadMesh.CombineMeshes(combinedRoadInstances.ToArray());
            
            meshFilter.mesh = roadMesh;
            meshRenderer.material = roadMaterial;
            
            AssetDatabase.CreateAsset(roadMesh, "Assets/GeneratedRoads/RoadMeshes/RoadMesh.asset");
            
            var combinedIntersectionsInstances = new List<CombineInstance>();

            foreach (var intersectionMatrix in intersectionMatrices)
            {
                for (int i = 0; i < intersectionMatrix.Count; i++)
                {
                    var ci = new CombineInstance { mesh = intersectionMesh, transform = intersectionMatrix[i]};
                    combinedIntersectionsInstances.Add(ci);
                }
                break;
            }
            
            var intersectionsMesh = new Mesh();
            intersectionsMesh.CombineMeshes(combinedIntersectionsInstances.ToArray());

            var intersectionChild = new GameObject("Intersections");
            var iFilter = intersectionChild.AddComponent<MeshFilter>();
            var iMesh = intersectionChild.AddComponent<MeshRenderer>();

            iFilter.mesh = intersectionsMesh;
            iMesh.material = roadMaterial;
            
            intersectionChild.transform.SetParent(roads.transform);
            
            AssetDatabase.CreateAsset(intersectionsMesh, "Assets/GeneratedRoads/RoadMeshes/IntersectionMeshes.asset");
            
            PrefabUtility.SaveAsPrefabAssetAndConnect(roads, "Assets/GeneratedRoads/Roads.prefab",
                InteractionMode.AutomatedAction);
            
            Debug.Log("Saved the mesh!");
        }
    }

    public bool SaveMeshes;

    private void Update()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].Update();
            carMatrices[i / instancesPerBatch][i % instancesPerBatch] = cars[i].matrix;
        }

        /*for (int i = 0; i < roadMeshes.Count; i++)
        {
            Graphics.DrawMesh(roadMeshes[i], Matrix4x4.identity, roadMaterial, 0);
        }*/

        for (int i = 0; i < intersectionMatrices.Count; i++)
        {
            Graphics.DrawMeshInstanced(intersectionMesh, 0, roadMaterial, intersectionMatrices[i]);
        }

        for (int i = 0; i < carMatrices.Count; i++)
        {
            if (carMatrices[i].Count > 0)
            {
                carMatProps.SetVectorArray("_Color", carColors[i]);
                Graphics.DrawMeshInstanced(carMesh, 0, carMaterial, carMatrices[i], carMatProps);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (trackVoxels != null && intersectionPairs.Count == 0)
        {
            // visualize voxel generation during generation
            for (int x = 0; x < voxelCount; x++)
            {
                for (int y = 0; y < voxelCount; y++)
                {
                    for (int z = 0; z < voxelCount; z++)
                    {
                        if (trackVoxels[x, y, z])
                        {
                            Gizmos.DrawWireCube(new Vector3(x, y, z) * voxelSize,
                                new Vector3(.9f, .9f, .9f) * voxelSize);
                        }
                    }
                }
            }
        }

        //if (roadMeshes != null && roadMeshes.Count == 0)
        {
            // visualize splines before road meshes have spawned
            if (intersections != null)
            {
                Gizmos.color = new Color(.2f, .2f, 1f);
                for (int i = 0; i < intersections.Count; i++)
                {
                    if (intersections[i].normal != Vector3Int.zero)
                    {
                        Gizmos.DrawWireMesh(intersectionPreviewMesh, 0, intersections[i].position,
                            Quaternion.LookRotation(intersections[i].normal),
                            new Vector3(intersectionSize, intersectionSize, 0f));
                    }
                }
            }

            if (trackSplines != null)
            {
                for (int i = 0; i < trackSplines.Count; i++)
                {
                    trackSplines[i].DrawGizmos();
                }
            }
        }
    }
}