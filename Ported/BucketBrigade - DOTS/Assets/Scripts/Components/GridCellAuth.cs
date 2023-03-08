using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GridCellAuth : MonoBehaviour
{
    public class GridCellBaker : Baker<GridCellAuth>
    {
       
        public override void Bake(GridCellAuth authoring)
        {
            AddComponent<GridCell>();
        }
    }
}

//This is a tag
public struct GridCell : IComponentData{
   
}