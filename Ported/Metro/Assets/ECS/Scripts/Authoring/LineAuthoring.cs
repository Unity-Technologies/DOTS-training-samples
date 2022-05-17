using Unity.Entities;
using UnityEngine;

public class LineAuthoring : MonoBehaviour
{
    public Color Colour;
    public int MaxTrains;
    public int CarriagesPerTrain;
    public float MaxTrainSpeed = 0.002f;
}

public class LineBaker : Baker<LineAuthoring>
{
    public override void Bake(LineAuthoring authoring)
    {
        AddComponent(new Line
        {
            Name = authoring.name,
            Colour = authoring.Colour,
            MaxTrains = authoring.MaxTrains,
            CarriagesPerTrain = authoring.CarriagesPerTrain,
            MaxTrainSpeed = authoring.MaxTrainSpeed
        });
    }
}