using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
        var buffer = AddBuffer<BezierPoint>();

        var t2 = authoring.transform.GetComponentsInChildren<Transform>(false);
        List<Transform> transforms = new List<Transform>(t2);
        transforms.RemoveRange(0, 1);

        // TODO Why is the parent node also in transforms ?

        foreach (var t in transforms)
        {
            buffer.Add(new BezierPoint { location = t.position });
        }

        var array = buffer.AsNativeArray();
        BezierPath.MeasurePath(array);
        var totalSidePoints = array.Length;

        var otherSide = new NativeArray<BezierPoint>(totalSidePoints, Allocator.Temp);

        for (int i = 0; i < totalSidePoints; ++i)
        {
            Vector3 _targetLocation = BezierPath.GetPoint_PerpendicularOffset(array, array[totalSidePoints -1  - i], 10.0f);
            otherSide[i] = new BezierPoint { location = _targetLocation };
        }

        buffer.AddRange(otherSide);

        BezierPath.MeasurePath(array);
    }

}
