using System;
using Unity.Mathematics;
using Unity.Rendering;

static class PlayerUtility
{
    public static URPMaterialPropertyBaseColor ColorFromPlayerIndex(int index)
    {
        URPMaterialPropertyBaseColor color;
        switch (index)
        {
            case 1: // Red
                color = new URPMaterialPropertyBaseColor {Value = new float4(1.0f, 0.0f, 0.0f, 1.0f)};
                break;
            case 2: // Green
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 1.0f, 0.0f, 1.0f)};
                break;
            case 3: // Blue
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 0.0f, 1.0f, 1.0f)};
                break;
            default: // Black
                color = new URPMaterialPropertyBaseColor {Value = new float4(0.0f, 0.0f, 0.0f, 1.0f)};
                break;
        }

        return color;
    }
}
