using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TileAuth : MonoBehaviour
{
    public float Temperature;
    public class TileBaker : Baker<TileAuth>
    {
       
        public override void Bake(TileAuth authoring)
        {
            AddComponent(new Tile
            {
                Temperature = authoring.Temperature
            });
        }
    }
}

//This is a tag
public struct Tile : IComponentData
{   
    public float Temperature;
}