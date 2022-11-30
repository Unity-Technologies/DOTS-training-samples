using Unity.Entities;
using UnityEngine;

class PathAuthoring : MonoBehaviour
{
    public int PathID;
    public Transform Default;
    public Transform Entry;
    public Transform Exit;
}
 
class PathBaker : Baker<PathAuthoring>
{
    public override void Bake(PathAuthoring authoring)
    {
        AddComponent<PathID>();
        AddComponent(new Path
        {
            Default = GetEntity(authoring.Default),
            Entry = GetEntity(authoring.Entry),
            Exit = GetEntity(authoring.Exit)
        });
    }
}