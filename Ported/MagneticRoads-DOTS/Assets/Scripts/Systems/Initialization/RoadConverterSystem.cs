using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class RoadConverterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!RoadGenerator.Instance.ready || RoadGenerator.Instance.trackSplines.Count == 0)
            return;
        
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        var roadEntityBuffer = new Entity[RoadGenerator.Instance.trackSplines.Count*4];
        //TODO use a job for each spline
        foreach (var splineDef in RoadGenerator.Instance.subSplines)
        {
            roadEntityBuffer[splineDef.splineId] = CreateRoadEntity(ecb, splineDef);
        }

        var intersectionEntityBuffer = new Entity[RoadGenerator.Instance.originIntersectionCount*2];
        for (var i = 0; i < RoadGenerator.Instance.originIntersectionCount*2; i++)
        {
            var intersectionEntity = ecb.CreateEntity();
            ecb.AddBuffer<CarQueue>(intersectionEntity);

            ecb.AddComponent<Translation>(intersectionEntity);
            ecb.SetComponent(intersectionEntity, new Translation{Value = RoadGenerator.Instance.intersections[i/2].position});
            
            intersectionEntityBuffer[i] = intersectionEntity;
        }
        var material = Resources.Load("Road", typeof(Material)) as Material;
        MeshData meshData = new MeshData();
        meshData.init();
        foreach (var spline in RoadGenerator.Instance.trackSplines)
        {
	        meshData.Clear();
		
	        GenerateMesh(spline, meshData.vertices, meshData.uvs, meshData.indices);
	        
	        var entity = ecb.CreateEntity();
	        ecb.SetName(entity, "Road Mesh");
	        ecb.AddComponent(entity, typeof(Translation));
	        ecb.AddComponent(entity, typeof(RenderMesh));
	        ecb.AddComponent(entity, typeof(Rotation));
	        ecb.AddComponent(entity, typeof(LocalToWorld));
	        ecb.AddComponent(entity, typeof(RenderBounds));
	        
	        Mesh mesh = new Mesh();
	        mesh.SetVertices(meshData.vertices.ToArray());
	        mesh.SetUVs(0, meshData.uvs.ToArray());
	        mesh.SetTriangles(meshData.indices.ToArray(), 0);
	        mesh.RecalculateNormals();
	        mesh.RecalculateBounds();
	        
	        var desc = new RenderMeshDescription(
		        mesh,
		        material);
	        
			RenderMeshUtility.AddComponents(entity, ecb, desc);
        }

        meshData.Dispose();

        
        //Add singleton containing dynamic buffer of spline def, spline links, spline to road and spline to intersection
        var wayfinderSingleton = ecb.CreateEntity();
        ecb.SetName(wayfinderSingleton, "SplineHolder");
        var splineBuffer = ecb.AddBuffer<SplineDefArrayElement>(wayfinderSingleton);
        foreach (var splineDef in RoadGenerator.Instance.subSplines)
        {
            splineBuffer.Add(new SplineDefArrayElement {Value = splineDef});
        }
        
        var splineLinkBuffer = ecb.AddBuffer<SplineLink>(wayfinderSingleton);
        foreach (var link in RoadGenerator.Instance.splineLinks)
        {
            splineLinkBuffer.Add(new SplineLink {Value = LinkToInt2(link)});
        }
        
        var splineIdToRoadBuffer = ecb.AddBuffer<SplineIdToRoad>(wayfinderSingleton);
        foreach (var entity in roadEntityBuffer)
        {
            splineIdToRoadBuffer.Add(new SplineIdToRoad {Value = entity});
        }
        
        var splineToIntersection = ecb.AddBuffer<IntersectionArrayElement>(wayfinderSingleton);
        for (var i = 0; i < RoadGenerator.Instance.subSplines.Length; i++)
        {
            splineToIntersection.Add(new IntersectionArrayElement {Value = intersectionEntityBuffer[RoadGenerator.Instance.intersectionLinks[i]]});
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        Enabled = false;

        RoadGenerator.Instance.intersectionLinks = null;
        RoadGenerator.Instance.splineLinks = null;
        RoadGenerator.Instance.subSplines = null;
        RoadGenerator.Instance.trackSplines = null;
        
        World.GetOrCreateSystem<CarSpawnerSystem>().Enabled = true;
    }

    private int2 LinkToInt2(List<int> link)
    {
        return link.Count == 1 ? new int2(link[0], int.MinValue) : new int2(link[0], link[1]);
    }
    
    struct MeshData
    {
	    public NativeList<Vector3> vertices;
	    public NativeList<Vector2> uvs;
	    public NativeList<int> indices;

	    public void init()
	    {
		    vertices = new NativeList<Vector3>(0, Allocator.Temp);
		    uvs = new NativeList<Vector2>(0, Allocator.Temp);
		    indices = new NativeList<int>(0, Allocator.Temp);
	    }

	    public void Clear()
	    {
		    vertices.Clear();
		    uvs.Clear();
		    indices.Clear();
	    }
	    
	    public void Dispose()
	    {
		    vertices.Dispose();
		    uvs.Dispose();
		    indices.Dispose();
	    }
    }
    
    private Entity CreateRoadEntity(EntityCommandBuffer ecb, SplineDef splineDef)
    {
        var entity = ecb.CreateEntity();
        ecb.SetName(entity, "Road");

        ecb.AddBuffer<CarQueue>(entity);
        ecb.AddComponent(entity, new CarQueueMaxLength
        {
            length = Mathf.Max(1, (int)(splineDef.measuredLength / .3f)) //TODO add car length to const comp
        });
        
        return entity;
    }
    
    static void GenerateMesh(TrackSpline spline, NativeList<Vector3> vertices, NativeList<Vector2> uvs,
		NativeList<int> indices)
	{
		spline.startNormal = spline.startIntersection.normal;
		spline.endNormal = spline.endIntersection.normal;
		
		// test three possible twisting modes to see which is best-suited
		// to this particular spline
		int minErrors = int.MaxValue;
		int bestTwistMode = 0;
		for (int i = 0; i < 3; i++)
		{
			spline.twistMode = i;
			spline.errorCount = 0;
			for (int j = 0; j <= RoadGenerator.splineResolution; j++)
			{
				Extrude(spline, Vector2.zero, (float)j / RoadGenerator.splineResolution);
			}

			if (spline.errorCount < minErrors)
			{
				minErrors = spline.errorCount;
				bestTwistMode = i;
			}
		}

		spline.twistMode = bestTwistMode;

		// a road segment is a rectangle extruded along a spline - here's the rectangle:
		Vector2 localPoint1 = new Vector2(-RoadGenerator.trackRadius, RoadGenerator.trackThickness * .5f);
		Vector2 localPoint2 = new Vector2(RoadGenerator.trackRadius, RoadGenerator.trackThickness * .5f);
		Vector2 localPoint3 = new Vector2(-RoadGenerator.trackRadius, -RoadGenerator.trackThickness * .5f);
		Vector2 localPoint4 = new Vector2(RoadGenerator.trackRadius, -RoadGenerator.trackThickness * .5f);

		// extrude our rectangle as four strips
		for (int i = 0; i < 4; i++)
		{
			Vector3 p1, p2;
			if (i == 0)
			{
				// top strip
				p1 = localPoint1;
				p2 = localPoint2;
			}
			else if (i == 1)
			{
				// right strip
				p1 = localPoint2;
				p2 = localPoint4;
			}
			else if (i == 2)
			{
				// bottom strip
				p1 = localPoint4;
				p2 = localPoint3;
			}
			else
			{
				// left strip
				p1 = localPoint3;
				p2 = localPoint1;
			}


			for (int j = 0; j <= RoadGenerator.splineResolution; j++)
			{
				float t = (float)j / RoadGenerator.splineResolution;

				Vector3 point1 = Extrude(spline, p1, t);
				Vector3 point2 = Extrude(spline, p2, t);

				int index = vertices.Length;

				vertices.Add(point1);
				vertices.Add(point2);
				uvs.Add(new Vector2(0f, t));
				uvs.Add(new Vector2(1f, t));
				if (j < RoadGenerator.splineResolution)
				{
					indices.Add(index + 0);
					indices.Add(index + 1);
					indices.Add(index + 2);

					indices.Add(index + 1);
					indices.Add(index + 3);
					indices.Add(index + 2);
				}
			}
		}
	}

	public static Vector3 Evaluate(TrackSpline spline, float t) {
		// cubic bezier

		t = Mathf.Clamp01(t);
		return spline.startPoint * (1f - t) * (1f - t) * (1f - t) + 3f * spline.anchor1 * (1f - t) * (1f - t) * t + 3f * spline.anchor2 * (1f - t) * t * t + spline.endPoint * t * t * t;
	}

	public static Vector3 Extrude(TrackSpline spline, Vector2 point, float t) {
		Vector3 tangent,up;
		return Extrude(spline, point,t,out tangent,out up);
	}

	public static Vector3 Extrude(TrackSpline spline, Vector2 point, float t, out Vector3 tangent, out Vector3 up) {
		Vector3 sample1 = Evaluate(spline, t);
		Vector3 sample2;

		float flipper = 1f;
		if (t+.01f<1f) {
			sample2 = Evaluate(spline, t + .01f);
		} else {
			sample2 = Evaluate(spline, t - .01f);
			flipper = -1f;
		}
		
		tangent = (sample2 - sample1).normalized * flipper;
		tangent.Normalize();

		// each spline uses one out of three possible twisting methods:
		Quaternion fromTo=Quaternion.identity;
		if (spline.twistMode==0) {
			// method 1 - rotate startNormal around our current tangent
			float angle = Vector3.SignedAngle(spline.startNormal,spline.endNormal,tangent);
			fromTo = Quaternion.AngleAxis(angle,tangent);
		} else if (spline.twistMode==1) {
			// method 2 - rotate startNormal toward endNormal
			fromTo = Quaternion.FromToRotation(spline.startNormal,spline.endNormal);
		} else if (spline.twistMode==2) {
			// method 3 - rotate startNormal by "startOrientation-to-endOrientation" rotation
			Quaternion startRotation = Quaternion.LookRotation(spline.startTangent,spline.startNormal);
			Quaternion endRotation = Quaternion.LookRotation(spline.endTangent * -1,spline.endNormal);
			fromTo = endRotation* Quaternion.Inverse(startRotation);
		}
		// other twisting methods can be added, but they need to
		// respect the relationship between startNormal and endNormal.
		// for example: if startNormal and endNormal are equal, the road
		// can twist 0 or 360 degrees, but NOT 180.

		float smoothT = Mathf.SmoothStep(0f,1f,t * 1.02f - .01f);
		
		up = Quaternion.Slerp(Quaternion.identity,fromTo,smoothT) * spline.startNormal;
		Vector3 right = Vector3.Cross(tangent,up);

		// measure twisting errors:
		// we have three possible spline-twisting methods, and
		// we test each spline with all three to find the best pick
		if (up.magnitude < .5f || right.magnitude < .5f) {
			spline.errorCount++;
		}

		return sample1 + right * point.x + up * point.y;
	}
}

