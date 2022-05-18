using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Rendering;

public class LineAuthoring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

class LineBaker : Baker<LineAuthoring>
{
    public override void Bake(LineAuthoring authoring)
    {
        var bezierBuffer = AddBuffer<BezierPoint>();
        var transformArray = authoring.transform.GetComponentsInChildren<Transform>(false);

        List<Transform> transforms = new List<Transform>(transformArray);
        transforms.RemoveRange(0, 1);

        //Create bezier
        foreach (var t in transforms)
        {
            bezierBuffer.Add(new BezierPoint { location = t.position });
        }

        var array = bezierBuffer.AsNativeArray();
        BezierPath.MeasurePath(array);
        var totalSidePoints = array.Length;

        var otherSide = new NativeArray<BezierPoint>(totalSidePoints, Allocator.Persistent);

        for (int i = 0; i < totalSidePoints; ++i)
        {
            Vector3 _targetLocation = BezierPath.GetPoint_PerpendicularOffset(array, array[totalSidePoints -1  - i], 10.0f);
            otherSide[i] = new BezierPoint { location = _targetLocation };
        }

        bezierBuffer.AddRange(otherSide);
        otherSide.Dispose();
        BezierPath.MeasurePath(bezierBuffer.AsNativeArray());

        //Create platforms
        var platformBuffer = AddBuffer<Platform>();
        var railMarkers = authoring.transform.GetComponentsInChildren<RailMarker>(false);

        for (int i = 0; i < railMarkers.Length - 1; i++)
        {
            if (railMarkers[i].railMarkerType == RailMarkerType.PLATFORM_START)
            {
                float totalDistance = BezierPath.Get_PathLength(bezierBuffer.AsNativeArray());

                platformBuffer.Add(new Platform { startPoint = bezierBuffer[i].distanceAlongPath,
                    endPoint = bezierBuffer[i + 1].distanceAlongPath,
                    startWorldPosition = BezierPath.Get_Position(bezierBuffer.AsNativeArray(), bezierBuffer[i].distanceAlongPath / totalDistance),
                    endWorldPosition = BezierPath.Get_Position(bezierBuffer.AsNativeArray(), bezierBuffer[i + 1].distanceAlongPath / totalDistance)
                });

                platformBuffer.Add(new Platform
                {
                    startPoint = bezierBuffer[bezierBuffer.Length - 1 - i - 1].distanceAlongPath,
                    endPoint = bezierBuffer[bezierBuffer.Length - 1 - i + 1 - 1].distanceAlongPath,
                    startWorldPosition = BezierPath.Get_Position(bezierBuffer.AsNativeArray(), bezierBuffer[bezierBuffer.Length - 1 -i - 1].distanceAlongPath / totalDistance),
                    endWorldPosition = BezierPath.Get_Position(bezierBuffer.AsNativeArray(), bezierBuffer[bezierBuffer.Length - 1 - i + 1 - 1].distanceAlongPath / totalDistance)
                });
            }
        }

        // Set Color
        AddComponent<URPMaterialPropertyBaseColor>(new URPMaterialPropertyBaseColor { Value = new Vector4(0, 0, 1, 0) });
    }
}
