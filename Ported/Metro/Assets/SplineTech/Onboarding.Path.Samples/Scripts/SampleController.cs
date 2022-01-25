using Onboarding.BezierPath;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

public class SampleController : MonoBehaviour
{
    public enum UpdateStrategy
    { 
        MONO_SINGLETHREAD,
        BURSTED_JOBS,
        OFF
    }

    public PathData pathData;
    public int agentsCount;
    public Material materialToUse;

    private Transform[] agentsTrs;

    private UpdateStrategy currentUpdateStrategy = UpdateStrategy.MONO_SINGLETHREAD;

    private NativeArray<float> entitiesDistancesFromSplineOrigin;
    private NativeArray<int> entitiesLastUsedCurveIndex;
    private NativeArray<float> entitiesSpeed;
    private NativeArray<Vector3> controlPoints;
    private NativeArray<ApproximatedCurveSegment> pathDataCurves;
    private TransformAccessArray transformArray;

    private EntityNavigationJob navigationJob;
    private JobHandle lastJNavigationJob;

    void Start()
    {
        entitiesDistancesFromSplineOrigin = new NativeArray<float>(agentsCount, Allocator.Persistent);
        entitiesLastUsedCurveIndex = new NativeArray<int>(agentsCount, Allocator.Persistent);
        entitiesSpeed = new NativeArray<float>(agentsCount, Allocator.Persistent);
        pathDataCurves = new NativeArray<ApproximatedCurveSegment>(pathData.m_DistanceToParametric.Length, Allocator.Persistent);
        pathDataCurves.CopyFrom(pathData.m_DistanceToParametric);
        controlPoints = new NativeArray<Vector3>(pathData.m_BezierControlPoints.Length, Allocator.Persistent);
        controlPoints.CopyFrom(pathData.m_BezierControlPoints);

        agentsTrs = new Transform[agentsCount];
        for (int i = 0; i < agentsCount; ++i)
        {
            GameObject go = GameObject.CreatePrimitive( (PrimitiveType)Random.Range(0, 4));
            var collider = go.GetComponent<Collider>();
            if (collider!=null)
                Destroy(collider);

            var renderer = go.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.sharedMaterial = materialToUse;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.allowOcclusionWhenDynamic = false;

            agentsTrs[i] = go.transform;
            float randomScale = Random.Range(0.5f, 1.6f);
            go.transform.localScale = randomScale * Vector3.one;
            go.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            entitiesDistancesFromSplineOrigin[i] = Random.Range(0, pathData.PathLength);
            entitiesLastUsedCurveIndex[i] = 0;
            entitiesSpeed[i] = (Random.value < 0.5f ? -1 : 1) * Random.Range(0.8f, 2.5f);
        }

        navigationJob = new EntityNavigationJob
        { 
            entitiesDistancesFromSplineOrigin = entitiesDistancesFromSplineOrigin, 
            entitiesLastUsedCurveIndex = entitiesLastUsedCurveIndex, 
            entitiesSpeed = entitiesSpeed, 
            pathDataCurves = pathDataCurves,
            pathLength = pathData.PathLength,
            controlPoints = controlPoints
        };

        transformArray = new TransformAccessArray(agentsTrs);
    }

    private void OnDestroy()
    {
        entitiesDistancesFromSplineOrigin.Dispose();
        entitiesLastUsedCurveIndex.Dispose();
        entitiesSpeed.Dispose();
        pathDataCurves.Dispose();
        controlPoints.Dispose();
        transformArray.Dispose();
    }

    private void Update()
    {
        switch (currentUpdateStrategy)
        {
            case UpdateStrategy.MONO_SINGLETHREAD:
                UpdateBasic();
                break;
            case UpdateStrategy.BURSTED_JOBS:
                UpdateWithJobs();
                break;
            case UpdateStrategy.OFF:
            default:
                break;
        }
    }

    public void OnDropdownChanged(int newValue)
    {
        currentUpdateStrategy = (UpdateStrategy)newValue;
        foreach (var trs in agentsTrs)
            trs.gameObject.SetActive(currentUpdateStrategy != UpdateStrategy.OFF);
    }

    private void UpdateBasic()
    {
        for (int i = 0; i < agentsCount; ++i)
        {
            int lastUsedCurveIndex = entitiesLastUsedCurveIndex[i];
            float distance = entitiesDistancesFromSplineOrigin[i] + Time.deltaTime * entitiesSpeed[i];
            if (distance > pathData.PathLength)
            {
                distance = distance % pathData.PathLength;
                lastUsedCurveIndex = 0;
            }
            else if (distance < 0)
            {
                distance = pathData.PathLength + (distance % pathData.PathLength);
                lastUsedCurveIndex = 0;
            }
            
            entitiesDistancesFromSplineOrigin[i] = distance;

            PathController.InterpolatePosition(pathData, ref lastUsedCurveIndex, distance, out var position);
            entitiesLastUsedCurveIndex[i] = lastUsedCurveIndex;
            agentsTrs[i].position = position;
        }
    }

    public void UpdateWithJobs()
    {
        Profiler.BeginSample("UpdateWithJobs");
        {
            // Synchronous call so we can actually compare with mono implementation
            navigationJob.deltaTime = Time.deltaTime;

            lastJNavigationJob = navigationJob.Schedule(transformArray);
            lastJNavigationJob.Complete();
        }
        Profiler.EndSample();
    }
}
