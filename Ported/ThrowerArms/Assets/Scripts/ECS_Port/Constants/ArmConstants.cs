public static class ArmConstants
{
    public const int ChainCount = 3;
    
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

    public const int BoneCount = (ArmConstants.ChainCount - 1 + FingerConstants.TotalChainCount - FingerConstants.CountPerArm + ThumbConstants.ChainCount - 1);
}