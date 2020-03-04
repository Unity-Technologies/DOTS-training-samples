﻿using Unity.Entities;

public struct ChildComponentData: IBufferElementData
{
    public Entity child;

    public static implicit operator ChildComponentData(Entity entity)
    {
        var c  = new ChildComponentData();
        c.child = entity;
        return c;
    }
}