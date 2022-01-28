using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadGenerator:MonoBehaviour
{
	public static RoadGenerator Instance;
	
	public bool ready = false;
	
	public int voxelCount=60;
	public float voxelSize = 1f;
	public int trisPerMesh = 4000;
	public Material roadMaterial;
	public Mesh intersectionMesh;
	public Mesh intersectionPreviewMesh;
	public Mesh carMesh;
	public Material carMaterial;
	public float carSpeed=2f;

	bool[,,] trackVoxels;
	public List<Intersection> intersections;
	public List<TrackSpline> trackSplines;
	Intersection[,,] intersectionsGrid;
	// List<Car> cars;

	List<List<Matrix4x4>> carMatrices;

	Vector3Int[] dirs;
	Vector3Int[] fullDirs;

	public const float intersectionSize = .5f;
	public const float trackRadius = .2f;
	public const float trackThickness = .05f;
	public const int splineResolution=20;
	public const float carSpacing = .13f;

	const int instancesPerBatch=1023;


	// intersection pair:  two 32-bit IDs, packed together
	HashSet<long> intersectionPairs;

	List<Mesh> roadMeshes;
	List<List<Matrix4x4>> intersectionMatrices;

	MaterialPropertyBlock carMatProps;
	List<List<Vector4>> carColors;

	long HashIntersectionPair(Intersection a, Intersection b) {
		// pack two intersections' IDs into one int64
		int id1 = a.id;
		int id2 = b.id;

		return ((long)Mathf.Min(id1,id2) << 32) + Mathf.Max(id1,id2);
	}

	int Dot(Vector3Int a,Vector3Int b) {
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	bool GetVoxel(Vector3Int pos,bool outOfBoundsValue=true) {
		return GetVoxel(pos.x,pos.y,pos.z,outOfBoundsValue);
	}
	bool GetVoxel(int x,int y,int z,bool outOfBoundsValue=true) {
		if (x >= 0 && x < voxelCount && y >= 0 && y < voxelCount && z >= 0 && z < voxelCount) {
			return trackVoxels[x,y,z];
		} else {
			return outOfBoundsValue;
		}
	}

	int CountNeighbors(Vector3Int pos,bool includeDiagonal = false) {
		return CountNeighbors(pos.x,pos.y,pos.z,includeDiagonal);
	}
	int CountNeighbors(int x, int y, int z, bool includeDiagonal = false) {
		int neighborCount = 0;

		Vector3Int[] dirList = dirs;
		if (includeDiagonal) {
			dirList = fullDirs;
		}

		for (int k = 0; k < dirList.Length; k++) {
			Vector3Int dir = dirList[k];
			if (GetVoxel(x + dir.x,y + dir.y,z + dir.z)) {
				neighborCount++;
			}
		}
		return neighborCount;
	}

	Intersection FindFirstIntersection(Vector3Int pos, Vector3Int dir, out Vector3Int otherDirection) {
		// step along our voxel paths (before splines have been spawned),
		// starting at one intersection, and stopping when we reach another intersection
		while (true) {
			pos += dir;
			if (intersectionsGrid[pos.x,pos.y,pos.z]!=null) {
				otherDirection = dir*-1;
				return intersectionsGrid[pos.x,pos.y,pos.z];
			}
			if (GetVoxel(pos+dir,false)==false) {
				bool foundTurn = false;
				for (int i=0;i<dirs.Length;i++) {
					if (dirs[i]!=dir && dirs[i]!=(dir*-1)) {
						if (GetVoxel(pos+dirs[i],false)==true) {
							dir = dirs[i];
							foundTurn = true;
							break;
						}
					}
				}
				if (foundTurn==false) {
					// dead end
					otherDirection = Vector3Int.zero;
					return null;
				}
			}
		}
	}

	private void Awake()
	{
		Instance = this;
		ready = false;
	}

	void Start() {
		// cardinal directions:
		dirs = new Vector3Int[] { new Vector3Int(1,0,0),new Vector3Int(-1,0,0),new Vector3Int(0,1,0),new Vector3Int(0,-1,0),new Vector3Int(0,0,1),new Vector3Int(0,0,-1) };

		// cardinal directions + diagonals in 3D:
		fullDirs = new Vector3Int[26];
		int dirIndex = 0;
		for (int x=-1;x<=1;x++) {
			for (int y=-1;y<=1;y++) {
				for (int z=-1;z<=1;z++) {
					if (x!=0 || y!=0 || z!=0) {
						fullDirs[dirIndex] = new Vector3Int(x,y,z);
						dirIndex++;
					}
				}
			}
		}
		StartCoroutine(SpawnRoads());
	}

	IEnumerator SpawnRoads() {
		// first generation pass: plan roads as basic voxel data only
		TrackSpline.splineIdCount = 0;
		trackVoxels = new bool[voxelCount,voxelCount,voxelCount];
		List<Vector3Int> activeVoxels = new List<Vector3Int>();
		trackVoxels[voxelCount / 2,voxelCount / 2,voxelCount / 2] = true;
		activeVoxels.Add(new Vector3Int(voxelCount / 2,voxelCount / 2,voxelCount / 2));

		// after voxel generation, we'll convert our network into non-voxels
		intersections = new List<Intersection>();
		intersectionsGrid = new Intersection[voxelCount,voxelCount,voxelCount];
		intersectionPairs = new HashSet<long>();
		trackSplines = new List<TrackSpline>();
		roadMeshes = new List<Mesh>();
		intersectionMatrices = new List<List<Matrix4x4>>();

		// cars = new List<Car>();
		carMatrices = new List<List<Matrix4x4>>();
		carMatrices.Add(new List<Matrix4x4>());
		carColors = new List<List<Vector4>>();
		carColors.Add(new List<Vector4>());
		carMatProps = new MaterialPropertyBlock();
		carMatProps.SetVectorArray("_Color",new Vector4[instancesPerBatch]);


		// plan roads broadly: first, as a grid of true/false voxels
		int ticker = 0;
		while (activeVoxels.Count>0 && ticker<50000) {
			ticker++;
			int index = Random.Range(0,activeVoxels.Count);
			Vector3Int pos = activeVoxels[index];
			Vector3Int dir = dirs[Random.Range(0,dirs.Length)];
			Vector3Int pos2 = new Vector3Int(pos.x + dir.x,pos.y + dir.y,pos.z + dir.z);
			if (GetVoxel(pos2) == false) {
				// when placing a new voxel, it must have fewer than three
				// diagonal-or-cardinal neighbors.
				// (this blocks nonplanar intersections from forming)
				if (CountNeighbors(pos2,true) < 3) {
					activeVoxels.Add(pos2);
					trackVoxels[pos2.x,pos2.y,pos2.z] = true;
				}

			}

			int neighborCount = CountNeighbors(pos);
			if (neighborCount >= 3) {
				// no more than three cardinal neighbors for any voxel (no 4-way intersections allowed)
				// (really, this is to avoid nonplanar intersections)
				Intersection intersection = new Intersection(pos,(Vector3)pos*voxelSize,Vector3Int.zero);
				intersection.id = intersections.Count;
				intersections.Add(intersection);
				intersectionsGrid[pos.x,pos.y,pos.z] = intersection;
				activeVoxels.RemoveAt(index);
			}

			if (ticker%1000==0) {
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

		for (int i=0;i<intersections.Count;i++) {
			Intersection intersection = intersections[i];
			Vector3Int axesWithNeighbors = Vector3Int.zero;
			for (int j=0;j<dirs.Length;j++) {
				if (GetVoxel(intersection.index+dirs[j],false)) {
					axesWithNeighbors.x += Mathf.Abs(dirs[j].x);
					axesWithNeighbors.y += Mathf.Abs(dirs[j].y);
					axesWithNeighbors.z += Mathf.Abs(dirs[j].z);

					Vector3Int connectDir;
					Intersection neighbor = FindFirstIntersection(intersection.index,dirs[j],out connectDir);
					if (neighbor!=null && neighbor!=intersection) {
						// make sure we haven't already added the reverse-version of this spline
						long hash = HashIntersectionPair(intersection,neighbor);
						if (intersectionPairs.Contains(hash)==false) {
							intersectionPairs.Add(hash);

							TrackSpline spline = new TrackSpline(intersection,dirs[j],neighbor,connectDir);
							trackSplines.Add(spline);

							// intersection.neighbors.Add(neighbor);
							intersection.neighborSplines.Add(spline);
							// neighbor.neighbors.Add(intersection);
							neighbor.neighborSplines.Add(spline);
						}
					}
				}
			}

			// find this intersection's normal - it's the one axis
			// along which we have no neighbors
			for (int j=0;j<3;j++) {
				if (axesWithNeighbors[j]==0) {
					if (intersection.normal == Vector3Int.zero) {
						intersection.normal = new Vector3Int();
						intersection.normal[j] = -1+Random.Range(0,2)*2;
						//Debug.DrawRay(intersection.position,(Vector3)intersection.normal * .5f,Color.red,1000f);
					} else {
						Debug.LogError("a straight line has been marked as an intersection!");
					}
				}
			}
			if (intersection.normal==Vector3Int.zero) {
				Debug.LogError("nonplanar intersections are not allowed!");
			}

			// NOTE - if you investigate the above logic, you might be confused about how
			// dead-ends are given normals, since we're assuming that all intersections
			// have two axes with neighbors and only one axis without. dead-ends only have
			// one neighbor-axis...and somehow they still get a normal without a special case.
			//
			// the "gotcha" is that the visible dead-ends in the demo have three
			// neighbors during the voxel phase, with two of their neighbor chains leading
			// to nothing. these "hanging chains" are not included as splines, so the
			// dead-ends that we see are actually "T" shapes with the top two segments hidden.

			if (i%20==0) {
				yield return null;
			}
		}
		

		Debug.Log(trackSplines.Count + " road splines");
		// generate road meshes

		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> triangles = new List<int>();

		for (int i=0;i<trackSplines.Count;i++) {
			trackSplines[i].GenerateMesh(vertices,uvs,triangles);	
			
			vertices.Clear();
			uvs.Clear();
			triangles.Clear();

			if (i%10==0) {
				yield return null;
			}
		}



		// generate intersection matrices for batch-rendering
		int batch = 0;
		intersectionMatrices.Add(new List<Matrix4x4>());
		for (int i=0;i<intersections.Count;i++) {
			intersectionMatrices[batch].Add(intersections[i].GetMatrix());
			if (intersectionMatrices[batch].Count==instancesPerBatch) {
				batch++;
				intersectionMatrices.Add(new List<Matrix4x4>());
			}
		}

		// Debug.Log(triCount + " road triangles ("+roadMeshes.Count+" meshes)");

		ConvertToMultiSplinePerRoad();
		ready = true;
	}

	private void Update() {
		for (int i=0;i<intersectionMatrices.Count;i++) {
			Graphics.DrawMeshInstanced(intersectionMesh,0,roadMaterial,intersectionMatrices[i]);
		}
	}

	public SplineDef[] subSplines;
	public List<int>[] splineLinks;
	public int[] intersectionLinks;
	public int originIntersectionCount;
	
	private void ConvertToMultiSplinePerRoad()
	{
		var splineToMultiSpline = new SplineDef[trackSplines.Count][];
		splineLinks = new List<int>[trackSplines.Count*4];
		var subSplineId = 0;
		originIntersectionCount = intersections.Count;
		
		foreach (var spline in trackSplines)
		{
			splineToMultiSpline[spline.splineId] = new SplineDef[4];
			var splineDict = splineToMultiSpline[spline.splineId];
			splineDict[0] = ToSplineDef(spline, false, true, ref subSplineId);
			splineDict[1] = ToSplineDef(spline, true, true, ref subSplineId);
			splineDict[2] = ToSplineDef(spline, false, false, ref subSplineId);
			splineDict[3] = ToSplineDef(spline, true, false, ref subSplineId);
		}
		
		foreach (var spline in splineToMultiSpline)
		{
			foreach (var splineDef in spline)
			{
				splineLinks[splineDef.splineId] = new List<int>();
			}
		}
		
		var ssDirection = new Dictionary<int, int[]>();
		intersectionLinks = new int[trackSplines.Count*4];
		for (var i = 0; i < intersections.Count; i++)
		{
			var intersection = intersections[i];
				
			ssDirection.Clear();
			foreach (var spline in intersection.neighborSplines)
			{
				if ((intersection.position - spline.startPoint).sqrMagnitude >
				    (intersection.position - spline.endPoint).sqrMagnitude)
				{
					ssDirection.Add(spline.splineId, new []{0,1,2,3});
				}
				else
				{
					ssDirection.Add(spline.splineId, new []{1,0,3,2});
				}
			}
			
			if (intersection.neighborSplines.Count == 1)//one way, U-Turn
			{
				var mainSplineId0 = intersection.neighborSplines[0].splineId;
				var subSplines0 = splineToMultiSpline[mainSplineId0];
				var ssDirection0 = ssDirection[mainSplineId0];
				splineLinks[subSplines0[ssDirection0[0]].splineId].Add(subSplines0[ssDirection0[1]].splineId);
				splineLinks[subSplines0[ssDirection0[2]].splineId].Add(subSplines0[ssDirection0[3]].splineId);

				intersectionLinks[subSplines0[ssDirection0[0]].splineId] = i*2;
				intersectionLinks[subSplines0[ssDirection0[2]].splineId] = i*2 + 1;
			}
			else if (intersection.neighborSplines.Count == 2)//two way, no U-Turn
			{
				var mainSplineId0 = intersection.neighborSplines[0].splineId;
				var subSplines0 = splineToMultiSpline[mainSplineId0];
				var ssDirection0 = ssDirection[mainSplineId0];
				var mainSplineId1 = intersection.neighborSplines[1].splineId;
				var subSplines1 = splineToMultiSpline[mainSplineId1];
				var ssDirection1 = ssDirection[mainSplineId1];
				splineLinks[subSplines0[ssDirection0[0]].splineId].Add(subSplines1[ssDirection1[1]].splineId);
				splineLinks[subSplines0[ssDirection0[2]].splineId].Add(subSplines1[ssDirection1[3]].splineId);
				splineLinks[subSplines1[ssDirection1[0]].splineId].Add(subSplines0[ssDirection0[1]].splineId);
				splineLinks[subSplines1[ssDirection1[2]].splineId].Add(subSplines0[ssDirection0[3]].splineId);
				
				intersectionLinks[subSplines0[ssDirection0[0]].splineId] = i*2;
				intersectionLinks[subSplines0[ssDirection0[2]].splineId] = i*2 + 1;
				intersectionLinks[subSplines1[ssDirection1[0]].splineId] = i*2;
				intersectionLinks[subSplines1[ssDirection1[2]].splineId] = i*2 + 1;
			}
			else if (intersection.neighborSplines.Count == 3)//three way, no U-Turn
			{
				var mainSplineId0 = intersection.neighborSplines[0].splineId;
				var subSplines0 = splineToMultiSpline[mainSplineId0];
				var ssDirection0 = ssDirection[mainSplineId0];
				var mainSplineId1 = intersection.neighborSplines[1].splineId;
				var subSplines1 = splineToMultiSpline[mainSplineId1];
				var ssDirection1 = ssDirection[mainSplineId1];
				var mainSplineId2 = intersection.neighborSplines[2].splineId;
				var subSplines2 = splineToMultiSpline[mainSplineId2];
				var ssDirection2 = ssDirection[mainSplineId2];
				splineLinks[subSplines0[ssDirection0[0]].splineId].Add(subSplines1[ssDirection1[1]].splineId);
				splineLinks[subSplines0[ssDirection0[0]].splineId].Add(subSplines2[ssDirection2[1]].splineId);
				splineLinks[subSplines0[ssDirection0[2]].splineId].Add(subSplines1[ssDirection1[3]].splineId);
				splineLinks[subSplines0[ssDirection0[2]].splineId].Add(subSplines2[ssDirection2[3]].splineId);
				
				splineLinks[subSplines1[ssDirection1[0]].splineId].Add(subSplines0[ssDirection0[1]].splineId);
				splineLinks[subSplines1[ssDirection1[0]].splineId].Add(subSplines2[ssDirection2[1]].splineId);
				splineLinks[subSplines1[ssDirection1[2]].splineId].Add(subSplines0[ssDirection0[3]].splineId);
				splineLinks[subSplines1[ssDirection1[2]].splineId].Add(subSplines2[ssDirection2[3]].splineId);
				
				splineLinks[subSplines2[ssDirection2[0]].splineId].Add(subSplines1[ssDirection1[1]].splineId);
				splineLinks[subSplines2[ssDirection2[0]].splineId].Add(subSplines0[ssDirection0[1]].splineId);
				splineLinks[subSplines2[ssDirection2[2]].splineId].Add(subSplines1[ssDirection1[3]].splineId);
				splineLinks[subSplines2[ssDirection2[2]].splineId].Add(subSplines0[ssDirection0[3]].splineId);
				
				intersectionLinks[subSplines0[ssDirection0[0]].splineId] = i*2;
				intersectionLinks[subSplines0[ssDirection0[2]].splineId] = i*2 + 1;
				intersectionLinks[subSplines1[ssDirection1[0]].splineId] = i*2;
				intersectionLinks[subSplines1[ssDirection1[2]].splineId] = i*2 + 1;
				intersectionLinks[subSplines2[ssDirection2[0]].splineId] = i*2;
				intersectionLinks[subSplines2[ssDirection2[2]].splineId] = i*2 + 1;
			}
		}

		subSplines = splineToMultiSpline.SelectMany(s => s).OrderBy(s => s.splineId).ToArray();

		//Verification intersection links
		// for (var i = 0; i < intersectionCount*2; i++)
		// {
		// 	var count = 0;
		// 	foreach (var link in intersectionLinks)
		// 	{
		// 		if (link == i)
		// 			count++;
		// 	}
		//
		// 	if (count == 0 || count > 3)
		// 	{
		// 		Debug.LogError($"Intersection {i} => count == {count}");
		// 	}
		// }
		
		
	}

	private SplineDef ToSplineDef(TrackSpline spline, bool reversed, bool isTop, ref int subSplineId)
	{
		var def = new SplineDef
		{
			splineId = subSplineId,
			startPoint = reversed ? spline.endPoint : spline.startPoint,
			anchor1 = reversed ? spline.anchor2 : spline.anchor1,
			anchor2 = reversed ? spline.anchor1 : spline.anchor2,
			endPoint = reversed ? spline.startPoint : spline.endPoint,
			startNormal = reversed ? spline.endNormal.ToInt3() : spline.startNormal.ToInt3(),
			endNormal = reversed ? spline.startNormal.ToInt3() : spline.endNormal.ToInt3(),
			startTangent = reversed ? spline.endTangent.ToInt3() : spline.startTangent.ToInt3(),
			endTangent = reversed ? spline.startTangent.ToInt3() : spline.endTangent.ToInt3(),
			twistMode = spline.twistMode,
			offset = new float2(- RoadGenerator.trackRadius * 0.5f, (isTop ? 1f : -1f) * RoadGenerator.trackThickness* 1.75f),
			measuredLength = spline.measuredLength
		};
		subSplineId++;
		return def;
	}
	private static Vector3 Extrude(SplineDef splineDef, float t)
    {
        Vector3 tangent;
        Vector3 sample1 = Evaluate(t, splineDef);
        Vector3 sample2;

        float flipper = 1f;
        if (t + .01f < 1f)
        {
            sample2 = Evaluate(t + .01f, splineDef);
        }
        else
        {
            sample2 = Evaluate(t - .01f, splineDef);
            flipper = -1f;
        }

        tangent = (sample2 - sample1).normalized * flipper;
        tangent.Normalize();

        // each spline uses one out of three possible twisting methods:
        Quaternion fromTo = Quaternion.identity;
        if (splineDef.twistMode == 0)
        {
            // method 1 - rotate startNormal around our current tangent
            float angle = Vector3.SignedAngle(splineDef.startNormal.ToVector3(),
                splineDef.endNormal.ToVector3(), tangent);
            fromTo = Quaternion.AngleAxis(angle, tangent);
        }
        else if (splineDef.twistMode == 1)
        {
            // method 2 - rotate startNormal toward endNormal
            fromTo = Quaternion.FromToRotation(splineDef.startNormal.ToVector3(),
                splineDef.endNormal.ToVector3());
        }
        else if (splineDef.twistMode == 2)
        {
            // method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
            Quaternion startRotation = Quaternion.LookRotation(splineDef.startTangent.ToVector3(),
                splineDef.startNormal.ToVector3());
            Quaternion endRotation = Quaternion.LookRotation(splineDef.endTangent.ToVector3() * -1,
                splineDef.endNormal.ToVector3());
            fromTo = endRotation * Quaternion.Inverse(startRotation);
        }
        // other twisting methods can be added, but they need to
        // respect the relationship between startNormal and endNormal.
        // for example: if startNormal and endNormal are equal, the road
        // can twist 0 or 360 degrees, but NOT 180.

        float smoothT = Mathf.SmoothStep(0f, 1f, t * 1.02f - .01f);

        Vector3 up = Quaternion.Slerp(Quaternion.identity, fromTo, smoothT) * splineDef.startNormal.ToVector3();
        Vector3 right = Vector3.Cross(tangent, up);


        return right * splineDef.offset.x + up * splineDef.offset.y;
    }

	private static Vector3 Evaluate(float t, SplineDef splineDef) {
		// cubic bezier

		t = Mathf.Clamp01(t);
		return splineDef.startPoint * (1f - t) * (1f - t) * (1f - t) + 3f * splineDef.anchor1 * (1f - t) * (1f - t) * t + 3f * splineDef.anchor2 * (1f - t) * t * t + splineDef.endPoint * t * t * t;
	}
}

