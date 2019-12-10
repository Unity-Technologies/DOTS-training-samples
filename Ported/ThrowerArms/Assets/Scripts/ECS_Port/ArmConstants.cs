public static class ArmConstants
{
    public const int ArmChainCount = 3;
    public const int FingerCount = 4;
    public const int FingerChainCountPerFinger = 4;
    public const int ThumbChainCount = 4;
    
    public const float BoneLength = 1;
    public const float BoneThickness = 0.15f;
    public const float BendStrength = 0.1f;
    public const float MaxReach = 1.8f;
    public const float ReachDuration = 1;
    public const float MaxHandSpeed = 1;
    public const float GrabTimerSmooth = 0; 
    public const float FingerThickness = 0.05f;

    public const float FingerXOffset = -0.12f;
    public const float FingerSpacing = 0.08f;
    public const float FingerBendStrength = 0.2f;

    public const float ThumbLength = 0.13f;
    public const float ThumbThickness = 0.06f;
    public const float ThumbBendStrength = 0.1f;
    public const float ThumbXOffset = -0.05f;

    public const float WindUpDuration = 0.7f;
    public const float ThrowDuraton = 1.2f;
    public const float BaseThrowSpeed = 24f;
    public const float TargetXRange = 15f;

    public static readonly float[] FingerBoneLengths = {0.2f, 0.22f, 0.2f, 0.16f};
}