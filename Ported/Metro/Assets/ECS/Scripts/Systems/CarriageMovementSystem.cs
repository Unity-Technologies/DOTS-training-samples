using Unity.Entities;
using Unity.Transforms;

public partial class CarriageMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        
        Entities
            .ForEach((Carriage carriage, TransformAspect transformAspect) =>
            {
                var train = GetComponent<Train>(carriage.Train);
                var bezierPath = GetComponent<BezierPath>(train.Line);
                ref var points = ref bezierPath.Data.Value.Points;

                var carriagePositionOnSpline = train.SplinePosition +
                                               carriage.Index *
                                               Bezier.GetRelativeSizeOnBezier(ref bezierPath.Data.Value, config.CarriageSizePlusPadding);
                
                transformAspect.Position = Bezier.GetPosition(ref points, carriagePositionOnSpline);
                transformAspect.LookAt(Bezier.GetLookAtTarget(ref points, carriagePositionOnSpline));
            }).Run();
    }
}