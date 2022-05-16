using System.Collections;
using System.Collections.Generic;
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

        var transforms = authoring.transform.GetComponentsInChildren<Transform>(false);
        // TODO Why is the parent node also in transforms ?
        int i = 0;
        foreach (var t in transforms)
        {
            if (i != 0) // Workaround don't take first node (the parent)
            {
                buffer.Add(new BezierPoint { location = t.position });
            }
            i++;
        }
    }

}
