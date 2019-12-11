public static class FingerConstants
{    
    public const int CountPerArm = 4;
    public const int PerFingerChainCount = 4;
    public const int TotalChainCount = CountPerArm * PerFingerChainCount;
    public const float BoneThickness = 0.05f;
    public const float XOffset = -0.12f;
    public const float Spacing = 0.08f;
    public const float BendStrength = 0.2f;

    public static readonly float[] BoneLengths = {0.2f, 0.22f, 0.2f, 0.16f};
}