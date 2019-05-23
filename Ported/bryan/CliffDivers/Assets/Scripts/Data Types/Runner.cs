using Unity.Entities;
using Unity.Mathematics;

// This component says what color Cube Mesh to render a Runner Bone as.
public struct BoneColor : IComponentData
{
	public float4 Value;
}

// A Runner is made of 11 Bones, and each refers to two Points.
public struct Bone
{
	public int point0;
	public int point1;
}
// A Runner has two Feet, and each has this struct.
public struct Foot
{
	public float3 target;
	public float3 stepStartPosition;
	public float animTimer;
	public int animating;	
}

// Once C# improves a little, we'll be able to simply say:
//
// public fixed float boneThicknesses[11]; // without needing fixed() blocks
//
// ...but for now, this is the best we can do.
//
public unsafe struct BoneThicknesses : IComponentData
{
	public const int Length = 11;		
	private fixed float buffer[Length];
	public ref float this[int i] 
	{
		get
		{
			fixed (float* b = buffer)
				return ref b[i];
		}
	}	
}

// Once C# improves a little, we'll be able to simply say:
//
// public fixed float boneLengths[11]; // without needing fixed() blocks
//
// ...but for now, this is the best we can do.
//
public unsafe struct BoneLengths : IComponentData
{
	public const int Length = 11;		
	private fixed float buffer[Length];
	public ref float this[int i] 
	{
		get
		{
			fixed (float* b = buffer)
				return ref b[i];
		}
	}	
}

// Once C# improves a little, we'll be able to simply say:
//
// public fixed Point points[12];
//
// ...but for now, this is the best we can do.
//
public unsafe struct Points : IComponentData
{
	public const int Length = 12;		
	private fixed uint buffer[(12 * Length + 3) / 4];
	public ref float3 this[int i] 
	{
		get
		{
			fixed (uint* b = buffer)
				return ref *(float3*)((byte*)b + sizeof(float3) * i);
		}
	}
}

// Once C# improves a little, we'll be able to simply say:
//
// public fixed Point prevPoints[12];
//
// ...but for now, this is the best we can do.
//
public unsafe struct PrevPoints : IComponentData
{
	public const int Length = 12;		
	private fixed uint buffer[(12 * Length + 3) / 4];
	public ref float3 this[int i] 
	{
		get
		{
			fixed (uint* b = buffer)
				return ref *(float3*)((byte*)b + sizeof(float3) * i);
		}
	}
}

// Once C# improves a little, we'll be able to simply say:
//
// public fixed Bone bones[11];
//
// ...but for now, this is the best we can do.
//
public unsafe struct Bones : IComponentData
{
	public const int Length = 11;
	private fixed uint buffer[(16 * Length + 3) / 4];
	public ref Bone this[int i] 
	{
		get
		{
			fixed (uint* b = buffer)
				return ref *(Bone*)((byte*)b + sizeof(Bone) * i);
		}
	}	
}

// Once C# improves a little, we'll be able to simply say:
//
// public fixed Foot foot[2]; 
//
// ...but for now, this is the best we can do.
//
public unsafe struct Feet : IComponentData
{
	public const int Length = 2;
	private fixed uint buffer[(32 * Length + 3) / 4];
	public ref Foot this[int i] 
	{
		get
		{
			fixed (uint* b = buffer)
				return ref *(Foot*)((byte*)b + sizeof(Foot) * i);
		}
	}
}

// Each time the Editor does a fixed timestep physics update, we calculate and buffer
// these values, for use in a later job that performs the physics updates as a batch.
public struct FixedUpdate
{
	public float time;
	public float runDirSway;
	public float deltaTime;
	public float fixedDeltaTime;
}

// Once C# improves a little, we'll be able to simply say:
//
// public fixed FixedUpdate fixedUpdate[4]; 
//
// ...but for now, this is the best we can do.
//
public unsafe struct FixedUpdates
{
	public const int Length = 4;
	private fixed uint buffer[4 * Length];
	public ref FixedUpdate this[int index]
	{
		get
		{
			fixed (uint* b = buffer)
				return ref *((FixedUpdate*) b + index);
		}
	}
}

// Certain global values that are uniform across all Runners, regardless of the number of runners,
// go here and are passed around into jobs.
public struct Globals
{
	public Bones bones;
	public FixedUpdates fixedUpdate;
	public int fixedUpdates;
	public float pitRadius;
	public float runSpeed;
}

// A running guy made of 11 Cube Meshes, which runs madly at the cliff of a hole in the ground,
// and then falls to his death.
public struct Runner : IComponentData {	
	public int ragdollMode;
	public int dead;
	public Unity.Transforms.Position position;

	public Points points;
	public PrevPoints prevPoints;
	public BoneThicknesses boneThicknesses;
	public BoneLengths boneLengths;
	public Feet foot;

	float hipHeight;
	public float shoulderHeight;
	float stanceWidth;
	float stepDuration;
	float legLength;
	float xzDamping;
	float spreadForce;

	public float timeSinceSpawn;
	
	public Random random;

	float RandomRange(float lo, float hi)
	{
		return lo + (hi - lo) * random.NextFloat();
	}
	
	float4 RandomColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax)
	{
		var hue        = RandomRange(hueMin, hueMax);
		var saturation = RandomRange(saturationMin, saturationMax);
		var value      = RandomRange(valueMin, valueMax);
		var color = UnityEngine.Color.HSVToRGB(hue, saturation, value, true);
		color.a = 1;
		return new float4(color.r,color.g,color.b,color.a);
	}
	
	// Initialize a Runner and BoneColor component, to make the runner appear far away from the sinkhole, facing at the
	// sinkhole, and running towards the sinkhole, with a random bone color.
	public static void Init(ref Runner runner, ref BoneColor boneColor, float3 pos, uint seed, Globals globals, int frame)
	{
		runner.random = new Random(seed);
		runner.stanceWidth = 0.35f;
		
		runner.position.Value = pos;
		runner.ragdollMode = 0;
		runner.dead = 0;

		float3 hsv = math.sin(globals.fixedUpdate[frame].time * new float3(1, 1 / 1.37f, 1 / 1.618f))*.5f+.5f;
		boneColor.Value = runner.RandomColorHSV(hsv.x,hsv.x,hsv.y*.2f+.1f,hsv.y*.4f+.15f,hsv.z*.15f+.25f,hsv.z*.35f+.25f);

		//hipHeight = Random.Range(1.5f,2.5f);
		//shoulderHeight = hipHeight + Random.Range(1.2f,1.8f);
		runner.hipHeight = 1.8f;
		runner.shoulderHeight = 3.5f;
		runner.stepDuration = runner.RandomRange(.25f,.33f);
		runner.xzDamping = runner.random.NextFloat()*.02f+.002f;
		runner.spreadForce = runner.RandomRange(.0005f,.0015f);

		runner.timeSinceSpawn = 0f;
		
		for (int i = 0; i < Bones.Length - 1; i++) {
			runner.boneThicknesses[i] = .2f;
		}
		runner.boneThicknesses[Bones.Length - 1] = .4f;

		runner.foot[0].animTimer = runner.random.NextFloat();
		runner.foot[1].animTimer = runner.random.NextFloat();
		runner.foot[0].animating = 1;
		runner.foot[1].animating = 1;

		runner.legLength = math.sqrt(runner.hipHeight * runner.hipHeight + runner.stanceWidth * runner.stanceWidth)*1.1f;

		UpdateOneTick(ref runner, globals, frame);
	}

	static void UpdateLimb(ref Points points, int index1, int index2, int jointIndex, float length, float3 perp) {
		float3 point1 = points[index1];
		float3 point2 = points[index2];
		float dx = point2.x - point1.x;
		float dy = point2.y - point1.y;
		float dz = point2.z - point1.z;
		float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
		float lengthError = dist - length;
		if (lengthError > 0f) {
			// requested limb is too long: clamp it

			length /= dist;
			points[index2] = new float3(point1.x + dx * length,
										 point1.y + dy * length,
										 point1.z + dz * length);
			points[jointIndex] = new float3(point1.x + dx * length*.5f,
											 point1.y + dy * length*.5f,
											 point1.z + dz * length*.5f);
		} else {
			// requested limb is too short: bend it

			lengthError *= .5f;
			dx *= lengthError;
			dy *= lengthError;
			dz *= lengthError;

			// cross product of (dx,dy,dz) and (perp)
			float3 bend = new float3(dy * perp.z - dz * perp.y,dz * perp.x - dx * perp.z,dx * perp.y - dy * perp.x);

			points[jointIndex] = new float3((point1.x + point2.x) * .5f+bend.x,
											 (point1.y + point2.y) * .5f+bend.y,
											 (point1.z + point2.z) * .5f+bend.z);
		}
	}
	
	// The Runner is not in the RagDoll state (arms and legs are pumping as he runs.)
	// Perform appropriate physics updates for a fixed timestep here.
	public static void UpdateOneTickNonRagDoll(ref Runner runner, Globals globals, int frame)
	{
		for (int i = 0; i < Points.Length; i++)
		{
			runner.prevPoints[i] = runner.points[i];
		}

		if (math.length(runner.position.Value) < globals.pitRadius + 1.5f)
		{
			runner.ragdollMode = 1;
			for (int i = 0; i < Bones.Length; i++)
			{
				runner.boneLengths[i] = math.length(runner.points[globals.bones[i].point0] - runner.points[globals.bones[i].point1]);
			}
		}

		float3 runDir = -runner.position.Value;
		runDir += math.cross(runDir, new float3(0,1,0)) * globals.fixedUpdate[frame].runDirSway;
		runDir = math.normalize(runDir);
		runner.position.Value += runDir * globals.runSpeed * globals.fixedUpdate[frame].fixedDeltaTime;
		float3 perp = new float3(-runDir.z, 0f, runDir.x);

		// hip
		runner.points[0] = new float3(runner.position.Value.x, runner.position.Value.y + runner.hipHeight, runner.position.Value.z);

		// feet
		float3 stanceOffset = new float3(perp.x * runner.stanceWidth, perp.y * runner.stanceWidth, perp.z * runner.stanceWidth);
		runner.foot[0].target = runner.position.Value - stanceOffset + runDir * (globals.runSpeed * .1f);
		runner.foot[1].target = runner.position.Value + stanceOffset + runDir * (globals.runSpeed * .1f);
		for (int i = 0; i < 2; i++)
		{
			int pointIndex = 2 + i * 2;
			float3 delta = runner.foot[i].target - runner.points[pointIndex];
			if (math.lengthsq(delta) > .25f)
			{
				if (runner.foot[i].animating == 0 && (runner.foot[1 - i].animating == 0 || runner.foot[1 - i].animTimer > .9f))
				{
					runner.foot[i].animating = 1;
					runner.foot[i].animTimer = 0f;
					runner.foot[i].stepStartPosition = runner.points[pointIndex];
				}
			}

			if (runner.foot[i].animating != 0)
			{
				runner.foot[i].animTimer = math.saturate(runner.foot[i].animTimer + globals.fixedUpdate[frame].fixedDeltaTime / runner.stepDuration);
				float timer = runner.foot[i].animTimer;
				runner.points[pointIndex] = math.lerp(runner.foot[i].stepStartPosition, runner.foot[i].target, timer);
				float step = 1f - 4f * (timer - .5f) * (timer - .5f);
				runner.points[pointIndex].y += step;
				if (runner.foot[i].animTimer >= 1f)
				{
					runner.foot[i].animating = 0;
				}
			}
		}

		// knees
		UpdateLimb(ref runner.points, 0, 2, 1, runner.legLength, perp);
		UpdateLimb(ref runner.points,0, 4, 3, runner.legLength, perp);

		// shoulders
		runner.points[6] = new float3(runner.position.Value.x + runDir.x * globals.runSpeed * .075f,
			runner.position.Value.y + runner.shoulderHeight,
			runner.position.Value.z + runDir.z * globals.runSpeed * .075f);

		// spine
		UpdateLimb(ref runner.points,0, 6, 5, runner.shoulderHeight - runner.hipHeight, perp);

		// hands
		for (int i = 0; i < 2; i++)
		{
			float3 oppositeFootOffset = runner.points[4 - 2 * i] - runner.points[0];
			oppositeFootOffset.y = oppositeFootOffset.y * (-.5f) - 1.7f;
			runner.points[8 + i * 2] = runner.points[0] - oppositeFootOffset * .65f -
									  perp * (.8f * (-1f + i * 2f)) + runDir * (globals.runSpeed * .05f);

			// elbows
			UpdateLimb(ref runner.points,6, 8 + i * 2, 7 + i * 2, runner.legLength * .9f, new float3(0f, -1f + i * 2f, 0f));
		}

		// head
		runner.points[11] = runner.points[6] + math.normalize(runner.position.Value) * -.1f + new float3(0f, .4f, 0f);

		// final frame of animated mode - prepare point velocities:
		if (runner.ragdollMode != 0)
		{
			for (int i = 0; i < Points.Length; i++)
			{
				runner.prevPoints[i] = runner.prevPoints[i] * .5f +
									  (runner.points[i] - runDir * globals.runSpeed * globals.fixedUpdate[frame].fixedDeltaTime *
									   (.5f + runner.points[i].y * .5f / runner.shoulderHeight)) * .5f;

				// jump
				if (i == 0 || i > 4)
				{
					runner.points[i] -= new float3(0f, runner.RandomRange(.05f, .15f), 0f);
				}
			}
		}
	}
	
	// The Runner is in the RagDoll state (arms and legs are flailing as he falls.)
	// Perform appropriate physics updates for a fixed timestep here.
	public static void UpdateOneTickRagDoll(ref Runner runner, Globals globals, int frame)
	{
		if (runner.dead != 0)
			return;

		// ragdoll mode

		float averageX = 0f;
		float averageY = 0f;
		float averageZ = 0f;
		for (int i = 0; i < Points.Length; i++)
		{
			averageX += runner.points[i].x;
			averageY += runner.points[i].y;
			averageZ += runner.points[i].z;
		}

		float3 averagePos = new float3(averageX / Points.Length, averageY / Points.Length,
			averageZ / Points.Length);

		for (int i = 0; i < Points.Length; i++)
		{
			float3 startPos = runner.points[i];
			runner.prevPoints[i].y += .005f;

			runner.prevPoints[i].x -= (runner.points[i].x - averagePos.x) * runner.spreadForce;
			runner.prevPoints[i].y -= (runner.points[i].y - averagePos.y) * runner.spreadForce;
			runner.prevPoints[i].z -= (runner.points[i].z - averagePos.z) * runner.spreadForce;

			runner.points[i].x += (runner.points[i].x - runner.prevPoints[i].x) * (1f - runner.xzDamping);
			runner.points[i].y += runner.points[i].y - runner.prevPoints[i].y;
			runner.points[i].z += (runner.points[i].z - runner.prevPoints[i].z) * (1f - runner.xzDamping);
			runner.prevPoints[i] = startPos;
			if (runner.points[i].y < -150f)
			{
				runner.dead = 1;
			}
		}

		for (int i = 0; i < Bones.Length; i++)
		{
			float3 point1 = runner.points[globals.bones[i].point0];
			float3 point2 = runner.points[globals.bones[i].point1];
			float dx = point1.x - point2.x;
			float dy = point1.y - point2.y;
			float dz = point1.z - point2.z;
			float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
			float pushDist = (dist - runner.boneLengths[i]) * .5f / dist;
			point1.x -= dx * pushDist;
			point1.y -= dy * pushDist;
			point1.z -= dz * pushDist;
			point2.x += dx * pushDist;
			point2.y += dy * pushDist;
			point2.z += dz * pushDist;

			runner.points[globals.bones[i].point0] = point1;
			runner.points[globals.bones[i].point1] = point2;
		}
	}

	// multiple physics ticks may have happened during this render frame.
	// here we apply all of them in turn to a single Runner.
	public static void UpdateOneTick(ref Runner runner, Globals globals, int frame)
	{
		runner.timeSinceSpawn += globals.fixedUpdate[frame].deltaTime;
		runner.timeSinceSpawn = math.saturate(runner.timeSinceSpawn);		
		if(runner.ragdollMode != 0)
			UpdateOneTickRagDoll(ref runner, globals, frame);
		else
			UpdateOneTickNonRagDoll(ref runner, globals, frame);
	}
	public static void UpdateMultipleTicks(ref Runner runner, Globals globals) 
	{
		for (var frame = 0; frame < globals.fixedUpdates; ++frame)
		{
			UpdateOneTick(ref runner, globals, frame);
		}
	}
		
}
