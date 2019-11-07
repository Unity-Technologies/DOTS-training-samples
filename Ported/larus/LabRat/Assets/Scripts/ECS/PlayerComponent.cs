using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    public int  PlayerId;
}

// References to the overlays the player owns (his placed arrows)
public struct PlayerOverlayComponent : IComponentData
{
    public Entity overlay0;
    public Entity overlay1;
    public Entity overlay2;
    public Entity overlayColor0;
    public Entity overlayColor1;
    public Entity overlayColor2;
}

public struct LocalPlayerComponent : IComponentData
{
    public int  PlayerId;
}
