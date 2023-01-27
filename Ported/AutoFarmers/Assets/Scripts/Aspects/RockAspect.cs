using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

readonly partial struct RockAspect : IAspect
{
    public readonly Entity Self;

    public readonly TransformAspect Transform;

    public readonly RefRW<Rock> Rock;

    public int Health
    {
        get => Rock.ValueRW.RockHealth;
        set => Rock.ValueRW.RockHealth = value;
    }

    public bool Damage(int damage,int2 gridPoint, ref WorldGrid grid)
    {
        Health -= damage;

        UpdateSizeBasedOnHealth();

        

        if (Health <= 0)
        {
            grid.SetTypeAt(gridPoint, 0);
            return true;
        }
        return false;
    }

    public void UpdateSizeBasedOnHealth()
    {
        Transform.LocalScale = (Health / (float)RockAuthoring.MAX_ROCK_HEALTH);
    }
}
