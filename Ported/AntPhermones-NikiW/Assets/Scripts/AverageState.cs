using System;

/// <summary>
///     Can stores the average of millions of values.
/// </summary>
public struct AverageState
{
    public double Sum { get; private set; }
    public double Average { get; private set; }
    public uint Count { get; private set; }

    public void AddSample(double value)
    {
        Sum += value;
        Count++;
        Average = Sum / Count;
    }
}
