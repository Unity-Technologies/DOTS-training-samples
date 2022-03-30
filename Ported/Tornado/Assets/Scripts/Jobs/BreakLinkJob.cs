using Components;
using Unity.Collections;
using Unity.Jobs;

public struct BreakLinkJob : IJob
{
    public NativeArray<VerletPoints> points;

    [ReadOnly] public NativeArray<Link> links;
    [ReadOnly] public NativeArray<float> extraDist;

    [ReadOnly] public PhysicsSettings physicSettings;

    public void Execute()
    {
        for (int r = 0; r < links.Length; r++)
        {

        }
    }
        
}
