
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(IKSolverGroupSystem))]
public class UpdateTargetSystem : JobComponentSystem
{
	private const float reachDuration = 1.0f;
	private const float maxReachLength = 1.8f;
	
	private const float windupDuration = 0.7f;
	private const float throwDuration = 1.2f;

	private Unity.Mathematics.Random randomGenerator;

	protected override void OnCreate()
	{
		randomGenerator = new Unity.Mathematics.Random((uint)Environment.TickCount);
	}

	struct UpdateTargetJob : IJobForEach_BCC<BoneJoint, Timers, ArmTarget>
    {
        public float Time;
        public float DeltaTime;

        public Unity.Mathematics.Random RandomGenerator;
        [ReadOnly] public ComponentDataFromEntity<Translation> Positions;
        
//        public void Execute([ReadOnly] DynamicBuffer<BoneJoint> joints, ref Timers timer, ref ArmTarget target)
//        {
//            float time = Time + timer.TimeOffset;
//
//			// resting position
//			var idleHandTarget = joints[0].JointPos + new float3(math.sin(time)*.35f,1f+math.cos(time*1.618f)*.5f,1.5f);
//			var grabHandTarget = float3.zero;
//			
//			if (!target.IsHolding && timer.Windup<=0f) 
//			{
//				if (target.TargetRock == Entity.Null)
//				{
//					if (timer.Reach != 0.0f)
//					{
//						// stop reaching if we've lost our target
//						timer.Reach -= DeltaTime / reachDuration;
//					}
//				}
//				else
//				{
//					// we're reaching for a rock (but we haven't grabbed it yet)
//					var delta = Positions[target.TargetRock].Value - joints[0].JointPos;
//					if (math.lengthsq(delta)< maxReachLength * maxReachLength) 
//					{
//						target.LastRockPosSize.xyz = Positions[target.TargetRock].Value;
//						
//						// figure out where we want to put our wrist
//						// in order to grab the rock
//						var flatDelta = delta;
//						flatDelta.y = 0f;
//						flatDelta = math.normalize(flatDelta);
//						grabHandTarget = target.LastRockPosSize.xyz + math.up() * target.LastRockPosSize.w * .5f - flatDelta * target.LastRockPosSize.w * .5f;
//						
//						
//						timer.Reach += DeltaTime / reachDuration;
//						if (timer.Reach >= 1f) {
//							// we've arrived at the rock - pick it up
//							target.IsHolding = true;
//							// remember the rock's position in "hand space"
//							// (so we can position the rock while holding it)
//							//heldRockOffset = handMatrix.inverse.MultiplyPoint3x4(heldRock.position);
//
//							// random minimum delay before starting the windup
//							timer.Windup = RandomGenerator.NextFloat(-1, 0);
//							timer.Throw = 0f;
//						}
//					} 
//					else 
//					{
//						// we didn't grab the rock in time - forget it
//						target.TargetRock = Entity.Null;
//						target.IsHolding = false;
//					}
//				}
//			}
//			else 
//			{
//				// stop reaching after we've successfully grabbed a rock
//				timer.Reach -= DeltaTime / reachDuration;
//				
//				if (target.TargetCan != Entity.Null) 
//				{
//					// found a target - prepare to throw
//					timer.Windup += DeltaTime / windupDuration;
//				}
//			}
//			timer.Reach = math.clamp(timer.Reach, 0.0f, 1.0f);
//
//			// smoothed reach timer
//			timer.GrabT = timer.Reach;
//			timer.GrabT = 3f * timer.GrabT * timer.GrabT - 2f * timer.GrabT * timer.GrabT * timer.GrabT;
//
//			// reaching overrides our idle hand position
//			target.Value = math.lerp(idleHandTarget,grabHandTarget,timer.GrabT);
//
//			if (target.TargetCan != Entity.Null) 
//			{
//				// we've got a target, which means we're currently throwing
//				if (timer.Windup < 1f) 
//				{
//					// still winding up...
//					float windupT = math.clamp(timer.Windup, 0.0f, 1.0f) - math.clamp(timer.Throw * 2f, 0.0f, 1.0f);
//					windupT = 3f * windupT * windupT - 2f * windupT * windupT * windupT;
//					target.Value = math.lerp(target.Value,target.WindupHandTarget,windupT);
//					var flatTargetDelta = Positions[target.TargetCan].Value - joints[0].JointPos;
//					flatTargetDelta.y = 0f;
//					flatTargetDelta = math.normalize(flatTargetDelta);
//
//					// windup position is "behind us," relative to the target position
//					target.WindupHandTarget = joints[0].JointPos - flatTargetDelta * 2f + math.up() * (3f - windupT * 2.5f);
//				} 
//				else 
//				{
//					// done winding up - actual throw, plus resetting to idle
//					timer.Throw += DeltaTime / throwDuration;
//
//					// update our aim until we release the rock
//					if (target.IsHolding) 
//					{
//						//target.AimVector = AimAtCan(targetCan,lastIntendedRockPos);
//						target.AimVector =
//							math.normalize(target.LastRockPosSize.xyz - Positions[target.TargetCan].Value);
//					}
//
//					// we start this animation in our windup position,
//					// and end it by returning to our default idle pose
//					var restingPos = math.lerp(target.WindupHandTarget,target.Value,timer.Throw);
//
//					// find the hand's target position to perform the throw
//					// (somewhere forward and upward from the windup position)
//					var throwHandTarget = target.WindupHandTarget + target.AimVector * 2.5f;
//
//					target.Value = math.lerp(restingPos,throwHandTarget, timer.Throw / throwDuration /*throwCurve.Evaluate(throwTimer)*/);
//
//					if (timer.Throw > .15f && target.IsHolding)
//					{
//						target.TargetRock = Entity.Null;
//						target.IsHolding = false;
//					}
//
//					if (timer.Throw >= 1f) 
//					{
//						// we've completed the animation - return to idle
//						timer.Windup = 0f;
//						timer.Reach = 0f;
//						target.TargetCan = Entity.Null;
//					}
//				}
//			}
//        }

	    public void Execute([ReadOnly] DynamicBuffer<BoneJoint> joints, ref Timers timer, ref ArmTarget target)
	    {
		    if (target.TargetRock != Entity.Null)
		    {
			    target.Value = Positions[target.TargetRock].Value;
		    }
//		    else
//		    {
//			    float time = Time + timer.TimeOffset;
//			    target.Value = joints[0].JointPos +
//			                         new float3(math.sin(time) * .35f, 1f + math.cos(time * 1.618f) * .5f, 1.5f);
//			    
//		    }
	    }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new UpdateTargetJob
        {
            Time = Time.time,
            DeltaTime = Time.deltaTime,
            RandomGenerator = randomGenerator,
            Positions = GetComponentDataFromEntity<Translation>(true)
        };
        return job.Schedule(this, inputDeps);
    }
}