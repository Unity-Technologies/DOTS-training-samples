using Unity.Entities;
using UnityEngine;

class PathAuthoring : MonoBehaviour
{
    public int PathID;
    public Transform Default;
    public Transform EntryLeft, ExitLeft;
    public Transform EntryRight, ExitRight;
}
 
class PathBaker : Baker<PathAuthoring>
{
    public override void Bake(PathAuthoring authoring)
    {
        AddComponent<PathID>();
        AddComponent(new Path
        {
            Default = GetEntity(authoring.Default),
            EntryLeft = GetEntity(authoring.EntryLeft),
            ExitLeft = GetEntity(authoring.ExitLeft),
            EntryRight = GetEntity(authoring.EntryRight),
            ExitRight = GetEntity(authoring.ExitRight)
        });
    }
}