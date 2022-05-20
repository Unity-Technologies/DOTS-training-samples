using Unity.Entities;

public partial class TrainMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var deltaTime = Time.DeltaTime;
        
        Entities
            .WithStructuralChanges()
            .ForEach((ref Train train) =>
            {
                var line = GetComponent<Line>(train.Line);
                
                train.SplinePosition += line.MaxTrainSpeed * deltaTime;
                
                if (train.SplinePosition >= 1f)
                    train.SplinePosition = 0f;
            }).Run();
    }
}