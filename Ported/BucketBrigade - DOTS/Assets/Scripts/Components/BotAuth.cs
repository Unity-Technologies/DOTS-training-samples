using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class BotAuth : MonoBehaviour
{
   public float cooldown;
   public class BotBaker : Baker<BotAuth>
   {
      public override void Bake(BotAuth authoring)
      {
         AddComponent(new BotTag
         {
            cooldown = authoring.cooldown
         });
         AddComponent<FrontBotTag>();
         AddComponent<ForwardPassingBotTag>();
         AddComponent<BackwardPassingBotTag>();
         AddComponent<BackBotTag>();
         AddComponent<CarryingBotTag>();
         AddComponent<ReachedTarget>();
         AddComponent<URPMaterialPropertyBaseColor>();

      }
   }
}

public struct BotTag : IComponentData
{
   public float cooldown;
   public int noInChain;
   public int indexInChain;

}

public struct FrontBotTag : IComponentData, IEnableableComponent{}

public struct ForwardPassingBotTag : IComponentData, IEnableableComponent{}

public struct BackwardPassingBotTag : IComponentData, IEnableableComponent{}

public struct BackBotTag : IComponentData, IEnableableComponent{}

public struct CarryingBotTag : IComponentData, IEnableableComponent {}

public struct BucketFetcherBotTag : IComponentData {}