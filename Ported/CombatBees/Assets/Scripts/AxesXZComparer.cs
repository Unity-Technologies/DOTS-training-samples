using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;

// public struct AxesXZComparer : IComparer<float3>
// {
//     /// <summary>
//     /// Round to ints and first compare by x value, then z value
//     /// </summary>
//     /// <param name="a"></param>
//     /// <param name="b"></param>
//     /// <returns></returns>
//     public int Compare(float3 a, float3 b)
//     {
//         var result = math.round(a.x).CompareTo(math.round(b.x));
//         if (result == 0)
//         {
//             result = math.round(a.z).CompareTo(math.round(b.z));
//         }
//         return result;
//     }
// }

public struct AxesXZComparer : IComparer<Entity>
// public struct AxesXZComparer : IComparer<Entity>
{
    [ReadOnly(true)] public ComponentLookup<Physical> PhysicaLookup;
    // AxesXZComparer(ComponentLookup<Physical> physicaLookup)
    // {
    //     _physicaLookup = physicaLookup;
    // }

    public void SetLookupReadOnly(in ComponentLookup<Physical> physicalLookup)
    {
        PhysicaLookup = physicalLookup;
    }
    /// <summary>
    /// Round to ints and first compare by x value, then z value
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    // public int Compare(float3 a, float3 b)
    // public int Compare(Entity a, Entity b)
    // {
    //     return a.CompareTo(b);
    //     
    //     // var result = math.round(a.x).CompareTo(math.round(b.x));
    //     // if (result == 0)
    //     // {
    //     //     result = math.round(a.z).CompareTo(math.round(b.z));
    //     // }
    //     // return result;
    // }
    
    public int Compare(Entity a, Entity b)
    {
        var aPosn = PhysicaLookup[a].Position;
        var bPosn = PhysicaLookup[b].Position;
        
        var result = math.round(aPosn.x).CompareTo(math.round(bPosn.x));
        if (result == 0)
        {
            result = math.round(aPosn.z).CompareTo(math.round(bPosn.z));
        }
        return result;
    }
}