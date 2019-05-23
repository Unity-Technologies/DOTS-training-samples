using UnityEngine;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// A ranged attribute for handling Parade parameters that are variable and random (such as distraction timing for Person objects)
/// </summary>
public class MinMaxRangeAttribute : PropertyAttribute
{

    public float minLimit = 0.0f;
    public float maxLimit = 0.0f;

    public MinMaxRangeAttribute(float minLimit, float maxLimit)
    {
        this.minLimit = minLimit;
        this.maxLimit = maxLimit;
    }

}

[Serializable]
public class MinMaxRange
{

    public float minimumValue = 0.0f;
    public float maximumValue = 0.0f;

    public MinMaxRange(float min, float max)
    {
        minimumValue = min;
        maximumValue = max;
    }

    public float GetRandomValue()
    {
        return Random.Range(minimumValue, maximumValue);
    }

    public int GetRandomValueAsInt()
    {
        return Mathf.RoundToInt(Random.Range(minimumValue, maximumValue));
    }

}

