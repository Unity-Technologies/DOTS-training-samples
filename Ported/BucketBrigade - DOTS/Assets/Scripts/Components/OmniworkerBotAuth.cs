using Unity.Entities;
using UnityEngine;

public class OmniworkerBotAuth : MonoBehaviour
{
    public class OmniworkerBaker : Baker<OmniworkerBotAuth>
    {
        public override void Bake(OmniworkerBotAuth authoring)
        {
            AddComponent<BotTag>();
            AddComponent<OmniworkerBotTag>();
            AddComponent<OmniworkerGoForBucketTag>();
            AddComponent<OmniworkerGoForWaterTag>();
            AddComponent<OmniworkerGoForFireTag>();
        }
    }
}

public struct OmniworkerBotTag : IComponentData, IEnableableComponent{
}
public struct OmniworkerGoForBucketTag : IComponentData, IEnableableComponent{
}
public struct OmniworkerGoForWaterTag : IComponentData, IEnableableComponent{
}
public struct OmniworkerGoForFireTag : IComponentData, IEnableableComponent{
}