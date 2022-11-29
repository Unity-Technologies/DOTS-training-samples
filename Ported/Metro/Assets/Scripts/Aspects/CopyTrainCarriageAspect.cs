using Unity.Entities;
using Unity.Mathematics;

readonly partial struct CopyTrainCarriageAspect : IAspect
{
    readonly RefRW<Carriage> CarriageComponent;
    
    public float3 TrainPosition
    {
        get => CarriageComponent.ValueRO.TrainPosition;
        set => CarriageComponent.ValueRW.TrainPosition = value;
    }
    
    public float3 TrainDirection
    {
        get => CarriageComponent.ValueRO.TrainDirection;
        set => CarriageComponent.ValueRW.TrainDirection = value;
    }
    
    public Entity Train => CarriageComponent.ValueRO.Train;
}