using Unity.Mathematics;

public class Constants {
    public const float SPACING = 1;
    public const float Y_OFFSET = 0;
    public const float HEIGHT_MIN = .5f;
    public static readonly float4 MIN_HEIGHT_COLOR = new float4(0.0f, 1.0f, 0.0f, 1.0f);
    public static readonly float4 MAX_HEIGHT_COLOR = new float4(99 /255f, 47 /255f, 0 /255f, 1.0f);
    public const float TANK_Y_OFFSET = .4f;
    
    public const float FOLLOW_HEIGHT_OFFSET_MIN = 4;
    public const float SMOOTH_DAMP_DURATION = .5f;
    public const float CANNONBALL_SPEED = 2.5f;
}