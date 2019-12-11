using static UnityEngine.Random;


public static class FingerConstants
{    
    public const int CountPerArm = 4;
    public const int PerFingerChainCount = 4;
    public const int TotalChainCount = CountPerArm * PerFingerChainCount;
    public const float Thickness = 0.05f;
    public const float XOffset = -0.12f;
    public const float Spacing = 0.08f;
    public const float BendStrength = 0.2f;

    public static readonly float[] BoneLengths = {0.2f, 0.22f, 0.2f, 0.16f};
}

public static class ThumbConstants
{
    public const float Length = 0.13f;
    public const float Thickness = 0.06f;
    public const float BendStrength = 0.1f;
    public const float XOffset = -0.05f;
    public const int ChainCount = 4;
}

public static class ArmConstants
{
    public const int ArmChainCount = 3;
    
    public const float BoneLength = 1;
    public const float BoneThickness = 0.15f;
    public const float BendStrength = 0.1f;
    public const float MaxReach = 1.8f;
    public const float ReachDuration = 1;
    public const float MaxHandSpeed = 1;
    public const float GrabTimerSmooth = 0; 


    public const float WindUpDuration = 0.7f;
    public const float ThrowDuration = 1.2f;
    public const float BaseThrowSpeed = 24f;
    public const float TargetXRange = 15f;
}

public static class TimeConstants
{
    public static readonly float Offset = value * 100f; 
}