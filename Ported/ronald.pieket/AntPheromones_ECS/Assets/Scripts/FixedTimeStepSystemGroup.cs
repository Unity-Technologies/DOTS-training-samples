using Unity.Entities;
using UnityEngine.Scripting;

public class FixedTimeStepSystemGroup : ComponentSystemGroup
{
  [Preserve] public FixedTimeStepSystemGroup() {}

  public static int UpdateCount = 1;
  
  protected override void OnUpdate()
  {
    for (int i = 0; i < UpdateCount; ++i)
    {
      base.OnUpdate();
    }
  }
}
