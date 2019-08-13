using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class CarMovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        
        //Entities.ForEach((ref TrackSplineComponent trackSpline) =>
        //{
        //    startPoint = trackSpline.startPoint;
        //});

        //Entities.ForEach((ref CarComponent carSpeed, ref Unity.Transforms.Translation position,ref Unity.Transforms.Rotation rotation) =>
        //{
        //    var deltaTime = Time.deltaTime;
        //    position.Value = position.Value + (math.forward(rotation.Value) * carSpeed.speed * deltaTime);
        //});
    }
}