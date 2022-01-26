using Unity.Entities;

[System.Serializable]
public struct FloatRange
{
    public float Min;
    public float Max;
}

//TODO: Maybe break this based on what is accessed in each job? (check before implementing)
[GenerateAuthoringComponent]
public struct TornadoSimulationParameters : IComponentData
{
    public float TornadoForce;
    public FloatRange ForceMultiplyRange;
    public float TornadoUpForce;
    public float TornadoInwardForce;
    public float Friction;

    public float Gravity;
    public float Damping;

    public float BreakDistance;
}