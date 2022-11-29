using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ColonyAuthor : MonoBehaviour
{

}

class ColonyBaker : Baker<ColonyAuthor>
{
    public override void Bake(ColonyAuthor authoring)
    {
        AddComponent(new Colony());
    }
}
