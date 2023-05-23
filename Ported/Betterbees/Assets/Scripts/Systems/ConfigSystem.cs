using Unity.Entities;
using UnityEngine;

public partial struct ConfigSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        bool created = false;
        foreach (var config in SystemAPI.Query<Config>())
        {
            if (!created)
            {
                SystemAPI.SetSingleton(config);
                created = true;
            }
            else
                Debug.LogError("Multiple configs");
        }
    }
}
