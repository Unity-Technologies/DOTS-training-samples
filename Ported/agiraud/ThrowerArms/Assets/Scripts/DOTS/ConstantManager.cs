using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ConstantManager : ScriptableObject
{
    public static Vector3 DestroyBoxMin = new Vector3(-100, -5, -100);
    public static Vector3 DestroyBoxMax = new Vector3(10, 50, 100);

    // TODO Move to the proper system
    public static float baseThrowSpeed = 24f; // TODO get from ArmManagerAuthoring
    /// <summary>
    /// Port of AimAtCan with math lib and float3
    /// </summary>
    /// <param name="canPosition"> targetCan position</param>
    /// <param name="canVelocity"> targetCan velocity</param>
    /// <param name="startPos"> last intended rock position</param>
    /// <returns>aim Vector</returns>
    public static float3 AimAtCan(float3 canPosition, float3 canVelocity, float3 startPos) {

        // predictive aiming based on this article by Kain Shin:
        // https://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php

        float targetSpeed = math.length(canVelocity);
        float cosTheta = math.dot(math.normalize(startPos - canPosition),math.normalize(canVelocity));

        float D =  math.length(canPosition - startPos);

        // quadratic equation terms
        float A = baseThrowSpeed * baseThrowSpeed - targetSpeed * targetSpeed;
        float B = (2f * D * targetSpeed * cosTheta);
        float C = -D * D;

        if (B * B < 4f * A * C) {
            // it's impossible to hit the target
            return math.forward(quaternion.identity) * 10f + math.up() * 8f;
        }

        // quadratic equation has two possible outputs
        float t1 = (-B + math.sqrt(B*B - 4f * A * C))/(2f*A);
        float t2 = (-B - math.sqrt(B*B - 4f * A * C))/(2f*A);

        // our two t values represent two possible trajectory durations.
        // pick the best one - whichever is lower, as long as it's positive
        float t;
        if (t1 < 0f && t2 < 0f) {
            // both potential collisions take place in the past!
            return  math.forward(quaternion.identity) * 10f + math.up() * 8f;
        } else if (t1<0f && t2>0f) {
            t = t2;
        } else if (t1>0f && t2<0f) {
            t = t1;
        } else {
            t = math.min(t1,t2);
        }

        float3 output = canVelocity - .5f*new float3(0f,-RockManagerAuthoring.RockGravityStrength,0f)*t + (canPosition - startPos) / t;

        if (math.length(output) > baseThrowSpeed*2f) {
            // the required throw is too serious for us to handle
            return math.forward(quaternion.identity) * 10f + math.up() * 8f;
        }

        return output;
    }

    public static void IKSolve(NativeArray<float3> chain, float boneLength, float3 anchor, float3 target, float3 bendHint) {
        chain[chain.Length - 1] = target;
        for (int i=chain.Length-2;i>=0;i--) {
            chain[i] += bendHint;
            float3 delta = chain[i] - chain[i + 1];
            chain[i] = chain[i + 1] + math.normalize(delta) * boneLength;
        }

        chain[0] = anchor;
        for (int i = 1; i<chain.Length; i++) {
            float3 delta = chain[i] - chain[i - 1];
            chain[i] = chain[i - 1] + math.normalize(delta) * boneLength;
        }
    }
}
