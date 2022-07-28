using Unity.Entities;

// i think this is right? unsure as of now :/ https://docs.unity3d.com/Packages/com.unity.entities@0.51/manual/dynamic_buffers.html
[InternalBufferCapacity(10)]
public struct HeightElement : IBufferElementData
{
    public float heightValue;

    public static implicit operator float(HeightElement height)
    {
        return height.heightValue;
    }

    public static implicit operator HeightElement(float value)
    {
        return new HeightElement { heightValue = value };
    }
}