using Unity.Entities;

[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
class MetroReferencesDeclaration : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Metro metro) =>
        {
            DeclareReferencedPrefab(metro.prefab_commuter);
            DeclareReferencedPrefab(metro.prefab_platform);
            DeclareReferencedPrefab(metro.prefab_trainCarriage);
        });
    }
}