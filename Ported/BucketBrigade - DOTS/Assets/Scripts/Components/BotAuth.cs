using Unity.Entities;
using UnityEngine;

public class BotAuth : MonoBehaviour
{
   public class BotBaker : Baker<BotAuth>
   {
      public override void Bake(BotAuth authoring)
      {
         AddComponent<BotTag>();
         AddComponent<FrontBotTag>();
         AddComponent<ForwardPassingBotTag>();
         AddComponent<BackwardPassingBotTag>();
         AddComponent<BackBotTag>();
      }
   }
}

public struct BotTag : IComponentData{
   
}

public struct FrontBotTag : IComponentData, IEnableableComponent{
   
}

public struct ForwardPassingBotTag : IComponentData, IEnableableComponent{
   
}

public struct BackwardPassingBotTag : IComponentData, IEnableableComponent{
   
}

public struct BackBotTag : IComponentData, IEnableableComponent{
   
}