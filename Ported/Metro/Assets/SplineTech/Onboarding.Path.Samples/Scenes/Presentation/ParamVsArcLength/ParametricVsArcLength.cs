using Onboarding.BezierPath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametricVsArcLength : MonoBehaviour
{
    public GameObject parametricGO;
    public GameObject arcLengthGO;
    public PathData pathData;
    public float cycleTime = 4;
    public TMPro.TextMeshPro parametricVelocityText;
    public TMPro.TextMeshPro arcLengthVelocityText;

    private int navigationCache;
    private Vector3 parametricLastPosition;
    private Vector3 arcLengthLastPosition;
    private double globalTime = 0;

    private double parametericGOAccDistance;
    private double arcLengthGOAccDistance;
    private float parametericGOSpeed = 0;
    private float arcLengthGOSpeed = 0;
    private double accTime = 0;


    // Update is called once per frame
    void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        //Debug.Log($"Delta Time {Time.deltaTime} - Fixed {Time.fixedDeltaTime}");

        globalTime += deltaTime;
        accTime += deltaTime;

        double timeBase = (globalTime / cycleTime) % 1; // fract part [0,1]
        double percentage = timeBase < 0.5 ? 2 * timeBase : 2 * (1 - timeBase);
        float parametricMethod_t = (float)percentage;
        float arcLengthMethod_Distance = (float)(percentage * pathData.PathLength);

        // Param based
        PathController.InterpolatePosition(pathData, 0, parametricMethod_t, out var positionParametric);
        PathController.InterpolatePosition(pathData, ref navigationCache, arcLengthMethod_Distance, out var positionArcLength);

        parametricGO.transform.position = positionParametric;
        arcLengthGO.transform.position = positionArcLength;

        parametericGOAccDistance += (positionParametric - parametricLastPosition).magnitude;
        arcLengthGOAccDistance += (positionArcLength - arcLengthLastPosition).magnitude;

        if (accTime > 0.15f)
        {
            parametericGOSpeed = Mathf.Lerp(parametericGOSpeed, (float)(parametericGOAccDistance / accTime), 0.5f);
            arcLengthGOSpeed = Mathf.Lerp(arcLengthGOSpeed, (float)(arcLengthGOAccDistance / accTime), 0.5f);
            parametericGOAccDistance = 0;
            arcLengthGOAccDistance = 0;
            accTime = 0;
        }

        parametricVelocityText.text = $"{parametericGOSpeed:0.0} m/s";
        parametricLastPosition = positionParametric;
        arcLengthVelocityText.text = $"{arcLengthGOSpeed:0.0} m/s";
        arcLengthLastPosition = positionArcLength;
    }
}
