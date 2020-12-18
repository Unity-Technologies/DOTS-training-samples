using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

public class PlantGenerator : SystemBase
{
	public static Dictionary<int, Mesh> meshLookup;

	protected override void OnCreate()
	{
		base.OnCreate();
		if (meshLookup == null)
		{
			meshLookup = new Dictionary<int, Mesh>();
		}
	}

	protected override void OnUpdate()
	{
		var commonSettings = GetSingleton<CommonSettings>();
		var insideUnitSphere = (float3)UnityEngine.Random.insideUnitSphere;

		/*Entities.WithAll<Plant>()
			.ForEach((Entity entity, in RenderMesh mesh, in Translation position, in Rotation rotation) =>
			{
				float3 worldPos = new float3(position.Value.x + .5f, 0f, position.Value.y + .5f);
				var matrix = float4x4.TRS(worldPos, rotation.Value, new float3(1));

				var linearIndex = position.Value.x + position.Value.y * commonSettings.GridSize.x;
				var newMesh = GenerateMesh((int)linearIndex);

                if (newMesh)
                {
                    mesh.mesh.vertices = newMesh.vertices;
                    mesh.mesh.triangles = newMesh.triangles;
                }
			}).WithoutBurst().Run();*/
    }

	//[MenuItem("APIExamples/SaveAssets")]
	public static Mesh GenerateMesh(int seed)
	{
		//var seed = System.DateTime.UtcNow.Millisecond;
		var random = new Unity.Mathematics.Random((uint)seed);
		var oldRandState = random.state;
		random.InitState((uint)seed);

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Color> colors = new List<Color>();
		List<float2> uv = new List<float2>();

		Color color1 = new Color(0f, 1f, .5f, .8f);
		Color color2 = new Color(0f, 1f, .5f, .8f);

		float height = random.NextFloat(.4f, 1.4f);

		float angle = random.NextFloat() * math.PI * 2f;
		float armLength1 = random.NextFloat() * .4f + .1f;
		float armLength2 = random.NextFloat() * .4f + .1f;
		float armRaise1 = random.NextFloat() * .3f;
		float armRaise2 = random.NextFloat() * .6f - .3f;
		float armWidth1 = random.NextFloat() * .5f + .2f;
		float armWidth2 = random.NextFloat() * .5f + .2f;
		float armJitter1 = random.NextFloat() * .3f;
		float armJitter2 = random.NextFloat() * .3f;
		float stalkWaveStr = random.NextFloat() * .5f;
		float stalkWaveFreq = random.NextFloat(.25f, 1f);
		float stalkWaveOffset = random.NextFloat() * math.PI * 2f;

		int triCount = random.NextInt(15, 35);

		for (int i = 0; i < triCount; i++)
		{
			// front face
			triangles.Add(vertices.Count);
			triangles.Add(vertices.Count + 1);
			triangles.Add(vertices.Count + 2);

			// back face
			triangles.Add(vertices.Count + 1);
			triangles.Add(vertices.Count);
			triangles.Add(vertices.Count + 2);

			float t = i / (triCount - 1f);
			float armLength = Mathf.Lerp(armLength1, armLength2, t);
			float armRaise = Mathf.Lerp(armRaise1, armRaise2, t);
			float armWidth = Mathf.Lerp(armWidth1, armWidth2, t);
			float armJitter = Mathf.Lerp(armJitter1, armJitter2, t);
			float stalkWave = Mathf.Sin(t * stalkWaveFreq * 2f * Mathf.PI + stalkWaveOffset) * stalkWaveStr;

			float y = t * height;
			vertices.Add(new float3(stalkWave, y, 0f));
			var armPos = new float3(stalkWave + math.cos(angle) * armLength, y + armRaise, math.sin(angle) * armLength);
			vertices.Add(armPos + (float3)UnityEngine.Random.insideUnitSphere * armJitter);
			armPos = new float3(stalkWave + math.cos(angle + armWidth) * armLength, y + armRaise, math.sin(angle + armWidth) * armLength);
			vertices.Add(armPos + (float3)UnityEngine.Random.insideUnitSphere * armJitter);

			colors.Add(color1);
			colors.Add(color2);
			colors.Add(color2);
			uv.Add(float2.zero);
			uv.Add(new float2(1,0));
			uv.Add(new float2(1,0));

			// golden angle in radians
			angle += 2.4f;
		}

		var outputMesh = new Mesh();
		outputMesh.name = "Generated Plant (" + seed + ")";

		outputMesh.SetVertices(vertices);
		outputMesh.SetColors(colors);
		outputMesh.SetTriangles(triangles, 0);
		outputMesh.RecalculateNormals();

		random.state = oldRandState;

		return outputMesh;
		//AssetDatabase.CreateAsset(outputMesh, "Assets/Meshes/plantmesh" + seed + ".mesh" );
	}
}