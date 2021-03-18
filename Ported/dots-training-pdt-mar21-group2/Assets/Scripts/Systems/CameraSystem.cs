using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

public class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var parameters = GetSingleton<SimulationParameters>();

        var armRowWidth = Utils.GetArmRowWidth(this);
        var deltaTime = Time.DeltaTime;

        var tiltJob = Entities
            .ForEach((ref CameraSwayTilt timer) => { timer.Value += deltaTime; })
            .ScheduleParallel(Dependency);

        var spinJob = Entities
            .ForEach((ref CameraSpin timer) => { timer.Value += deltaTime; })
            .ScheduleParallel(Dependency);

        var zoomJob = Entities
            .ForEach((ref CameraZoom timer) => { timer.Value += deltaTime; })
            .ScheduleParallel(Dependency);

        var pitchJob = Entities
            .ForEach((ref CameraSwayX timer) => { timer.Value += deltaTime; })
            .ScheduleParallel(Dependency);

        var angleJob = Entities
            .WithAll<CameraRef>()
            .ForEach((ref Rotation rotation, in CameraSwayTilt swayTilt, in CameraSpin spin) =>
            {
                var t = new float3(
                    swayTilt.Value / parameters.CameraSwayTiltDuration * 2.0f * math.PI,
                    spin.Value / parameters.CameraSpinDuration * 2.0f * math.PI,
                    0.0f);
                var eulerAngles =
                    math.sin(t) * new float3(parameters.CameraTiltAmount, parameters.CameraSpinAmount, 1.0f);
                eulerAngles += +parameters.CameraTiltOffset;
                rotation = new Rotation
                {
                    Value = quaternion.Euler(
                        math.radians(eulerAngles), math.RotationOrder.XYZ
                    )
                };
            })
            .ScheduleParallel(Unity.Jobs.JobHandle.CombineDependencies(tiltJob, spinJob));

        var positionJob = Entities
            .ForEach((ref Translation translation, in CameraSwayX swayX) =>
            {
                var x = math.cos(swayX.Value / parameters.CameraSwayXDuration * 2.0f * math.PI + math.PI) * 0.5f +
                        0.5f;
                translation.Value = new float3(x * armRowWidth, 0, 0) + parameters.CameraPosOffset;
            })
            .ScheduleParallel(pitchJob);

        Dependency = Unity.Jobs.JobHandle.CombineDependencies(angleJob, zoomJob, positionJob);

        Entities
            .WithoutBurst()
            .ForEach((in CameraRef cameraRef, in Translation translation) =>
            {
                cameraRef.m_CameraContainer.transform.position = (Vector3) translation.Value;
            })
            .Run();

        Entities
            .WithoutBurst()
            .ForEach((in CameraRef cameraRef, in Rotation rotation) =>
            {
                cameraRef.m_CameraContainer.transform.rotation = (Quaternion) rotation.Value;
            })
            .Run();

        Entities
            .WithoutBurst()
            .ForEach((in CameraRef cameraRef, in CameraZoom zoom) =>
            {
                var cam = cameraRef.m_CameraContainer.GetComponentInChildren<Camera>().transform;
                var t = math.cos(zoom.Value / parameters.CameraZoomDuration * 2.0f * math.PI + math.PI) * 0.5f + 0.5f;
                cam.localPosition = new Vector3(0.0f, 0.0f,
                    -math.lerp(parameters.CameraMinCamDist, parameters.CameraMaxCamDist, t));
            })
            .Run();
    }
}
