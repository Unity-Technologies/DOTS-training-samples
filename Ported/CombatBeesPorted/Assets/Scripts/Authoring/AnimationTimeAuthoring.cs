using Unity.Entities;
using UnityEngine;

class AnimationTimeAuthoring : UnityEngine.MonoBehaviour
{
}

class AnimationTimeBaker : Baker<AnimationTimeAuthoring>
{
    public override void Bake(AnimationTimeAuthoring authoring)
    {
        AddComponent(new AnimationTime());
    }
}
