using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// This is an in-place value type array of 11 Matrix4x4 structs, one for each Bone of a Runner.
// It's used for rendering Cube meshes for each of the 11 Bones.
// Once C# improves a little, we'll be able to simply say:
//
// public fixed Matrix4x4 buffer[Length];
//
// ...but for now, this is the best we can do.
//
public unsafe struct BoneMatrices : IComponentData
{
	public const int Length = 11;		
	private fixed float buffer[16 * Length];
	public ref Matrix4x4 this[int i] 
	{
		get
		{
			fixed (float* b = buffer)
				return ref *((Matrix4x4*)b + i);
		}
	}	
}

// This bootstraps the Entity World, but *also* handles and buffers FixedUpdates,
// which is necessary because when we simulate physics, we do multiple fixed updates.
public class RunnerManager : MonoBehaviour{
	public static RunnerManager Instance;
	public Mesh cubeMesh;
	public Material cubeMaterial;
	public float spawnSpacing;
	public float spawnDistanceFromPit;

	public Globals globals;	

	float spawnAngle = 0f;

	private Unity.Mathematics.Random random;

	float RandomRange(float lo, float hi)
	{
		return lo + (hi - lo) * random.NextFloat();
	}
	
	void SpawnARunner(int frame) {
		float spawnRadius = PitGenerator.pitRadius + spawnDistanceFromPit;
		float3 pos = new float3(math.cos(spawnAngle) * spawnRadius,0f,math.sin(spawnAngle) * spawnRadius);

		Runner runner = new Runner();
		BoneColor boneColor = new BoneColor();
		
		Runner.Init(ref runner, ref boneColor, pos, random.NextUInt(), globals, frame);

		int mode = Mathf.FloorToInt(Time.time / 8f);
		if (mode % 2 == 0) {
			spawnAngle = random.NextFloat() * 2f * (float)math.PI;
			spawnSpacing = RandomRange(.04f,(float)math.PI*.4f);
		} else {
			spawnAngle += spawnSpacing;
		}

		var entity = manager.CreateEntity(archetype);
		manager.SetComponentData(entity, runner);
		manager.SetComponentData(entity, boneColor);
	}

	private EntityManager manager;
	private EntityArchetype archetype;
	
	void Start ()
	{
		Instance = this;
		globals = new Globals();
		globals.bones = new Bones();
		globals.bones[ 0] = new Bone{point0=0, point1= 1}; //thigh 1
		globals.bones[ 1] = new Bone{point0=1, point1= 2}; //shin 1
		globals.bones[ 2] = new Bone{point0=0, point1= 3}; //thigh 2
		globals.bones[ 3] = new Bone{point0=3, point1= 4}; //shin 2
		globals.bones[ 4] = new Bone{point0=0, point1= 5}; //lower spine
		globals.bones[ 5] = new Bone{point0=5, point1= 6}; //upper spine
		globals.bones[ 6] = new Bone{point0=6, point1= 7}; //bicep 1
		globals.bones[ 7] = new Bone{point0=7, point1= 8}; //forearm 1
		globals.bones[ 8] = new Bone{point0=6, point1= 9}; //bicep 2
		globals.bones[ 9] = new Bone{point0=9, point1=10}; //forearm 2
		globals.bones[10] = new Bone{point0=6, point1=11}; //head
		globals.runSpeed = 10f;
		globals.pitRadius = PitGenerator.pitRadius;
		
		random = new Unity.Mathematics.Random(0xBAD5EED);

		manager = World.Active.GetOrCreateManager<EntityManager>();
		archetype = manager.CreateArchetype(typeof(Runner), typeof(BoneMatrices), typeof(BoneColor));
	}

	private void FixedUpdate()
	{
		if (globals.fixedUpdates < FixedUpdates.Length)
		{
			FixedUpdate f = new FixedUpdate();
			f.time = Time.time;
			f.deltaTime = Time.deltaTime;
			f.fixedDeltaTime = Time.fixedDeltaTime;
			f.runDirSway = Mathf.Sin(Time.time * .5f) * .5f;
			globals.fixedUpdate[globals.fixedUpdates] = f;
			++globals.fixedUpdates;
		}
	}
	
	void Update () {
		for (int frame = 0; frame < globals.fixedUpdates; ++frame)
			for (int i = 0; i < 2; i++)
				SpawnARunner(frame);
	}
}

public class RunnerBarrier : BarrierSystem {}

// This runs multiple fixed timeslice physics updates to Runner components,
// which can be in either an animated or a ragdoll state. Because this state
// can change in the middle of a set of N concurrently-simulated timeslices,
// it's not simple/performant to add a tag for the state, and run each state's update as a separate job.
[UpdateBefore(typeof(RunnerMatrixSystem))]
public class RunnerUpdateSystem : JobComponentSystem
{	
	private ComponentGroup componentGroup;
	[Inject] private RunnerBarrier runnerBarrier;

	public override void OnCreateManager()
	{
		componentGroup = GetComponentGroup(typeof(Runner));
	}
	
	[BurstCompile]
	struct RunnerUpdateJob : IJobProcessComponentDataWithEntity<Runner>
	{
		public EntityCommandBuffer.Concurrent ecb;
		public Globals globals;
		public void Execute(Entity entity, int index, ref Runner runner)
		{
			Runner.UpdateMultipleTicks(ref runner, globals);
			if (runner.dead != 0)
				ecb.DestroyEntity(index, entity);
		}
	};
	
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{		
		var ecb = runnerBarrier.CreateCommandBuffer();

		var updateRunners = new RunnerUpdateJob
		{
			ecb = ecb.ToConcurrent(),
			globals = RunnerManager.Instance.globals
		};
		RunnerManager.Instance.globals.fixedUpdates = 0;
		return updateRunners.ScheduleGroup(componentGroup, inputDeps);
	}
}

// This generates one Matrix4x4 for each of the 11 Bones of each of the Runners,
// So that we can later use the Matrix4x4 to render a Cube mesh.
[UpdateAfter(typeof(RunnerUpdateSystem))]
[UpdateBefore(typeof(RunnerRenderSystem))]
public class RunnerMatrixSystem : JobComponentSystem
{	
	private ComponentGroup componentGroup;

	public override void OnCreateManager()
	{
		componentGroup = GetComponentGroup(typeof(Runner), typeof(BoneMatrices));
	}
	
	[BurstCompile]
	struct RunnerMatrixJob : IJobProcessComponentData<Runner, BoneMatrices>
	{
		public Bones bones;
		public float time;
		public float fixedTime;
		public float fixedDeltaTime;

		public void Execute(ref Runner runner, ref BoneMatrices boneMatrices)
		{
			for (int j = 0; j < Bones.Length; j++)
			{
				float3 point1 = runner.points[bones[j].point0];
				float3 point2 = runner.points[bones[j].point1];
				float3 oldPoint1 = runner.prevPoints[bones[j].point0];
				float3 oldPoint2 = runner.prevPoints[bones[j].point1];

				float t = (time - fixedTime) / fixedDeltaTime;
				point1 += (point1 - oldPoint1) * t;
				point2 += (point2 - oldPoint2) * t;

				float3 delta = point2 - point1;
				float3 position = (point1 + point2) * .5f;
				quaternion rotation = Quaternion.LookRotation(delta);
				float3 scale = new float3(runner.boneThicknesses[j] * runner.timeSinceSpawn,
					runner.boneThicknesses[j] * runner.timeSinceSpawn,
					math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z) * runner.timeSinceSpawn);
				boneMatrices[j] = Matrix4x4.TRS(position, rotation, scale);
			}
		}
	};

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{				
		var renderRunners = new RunnerMatrixJob
		{
			bones = RunnerManager.Instance.globals.bones,
			time = Time.time,
			fixedTime = Time.fixedTime,
			fixedDeltaTime = Time.fixedDeltaTime,
		};
		return renderRunners.ScheduleGroup(componentGroup, inputDeps);
	}
}

// This renders each of the 11 Bones of a Runner as a Cube Mesh. We couldn't use the TransformSystem etc.
// machinery here, because each Runner outputs 11 meshes, and none of these meshes has an independent Position.
[UpdateAfter(typeof(RunnerMatrixSystem))]
public unsafe class RunnerRenderSystem : ComponentSystem
{
	public MaterialPropertyBlock matProps= new MaterialPropertyBlock();
	Matrix4x4[] matrices = new Matrix4x4[instancesPerBatch];
	Vector4[] colors= new Vector4[instancesPerBatch];
	const int instancesPerBatch=1023;
	private ComponentGroup componentGroup;

	public override void OnCreateManager()
	{
		componentGroup = GetComponentGroup(typeof(BoneMatrices), typeof(BoneColor));
	}
	
	protected override void OnUpdate()
	{
		var cubeMesh = RunnerManager.Instance.cubeMesh;
		var cubeMaterial = RunnerManager.Instance.cubeMaterial;
		using (var chunks = componentGroup.CreateArchetypeChunkArray(Allocator.TempJob))
		{
			ArchetypeChunkComponentType<BoneMatrices> BoneMatricesType = GetArchetypeChunkComponentType<BoneMatrices>();
			ArchetypeChunkComponentType<BoneColor> BoneColorType = GetArchetypeChunkComponentType<BoneColor>();
			int written = 0;
			int toWrite = instancesPerBatch;
			for (var i = 0; i < chunks.Length; ++i)
			{
				var chunk = chunks[i];
				var nativematrices = (Matrix4x4*) chunk.GetNativeArray(BoneMatricesType).GetUnsafePtr();
				var nativecolors = (Vector4*) chunk.GetNativeArray(BoneColorType).GetUnsafePtr();
				int read = 0;
				int toRead = chunk.Count * BoneMatrices.Length;
				while (toRead > 0)
				{
					var toCopy = toRead < toWrite ? toRead : toWrite;
					fixed (Vector4* c = colors)
						for(var j = 0; j < toCopy; ++j)
							c[written + j] = nativecolors[(read + j) / BoneMatrices.Length];
					fixed (Matrix4x4* m = matrices)
						UnsafeUtility.MemCpy(m + written, nativematrices + read, sizeof(Matrix4x4) * toCopy);
					read += toCopy;
					written += toCopy;
					toRead -= toCopy;
					toWrite -= toCopy;
					if (toWrite == 0)
					{
						matProps.SetVectorArray("_Color", colors);
						Graphics.DrawMeshInstanced(cubeMesh, 0, cubeMaterial, matrices, written, matProps);
						written = 0;
						toWrite = instancesPerBatch;
					}
				}
			}

			if (written != 0)
			{
				matProps.SetVectorArray("_Color", colors);
				Graphics.DrawMeshInstanced(cubeMesh, 0, cubeMaterial, matrices, written, matProps);
			}
		}
	}
}