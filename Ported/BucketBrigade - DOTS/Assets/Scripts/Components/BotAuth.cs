using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BotAuth : MonoBehaviour
{
   public class BotAuthBaker : Baker<BotAuth>
   {
      public override void Bake(BotAuth authoring)
      {
         AddComponent<Bot>();
      }
   }
}
//This is a tag
public struct Bot : IComponentData{
   
}