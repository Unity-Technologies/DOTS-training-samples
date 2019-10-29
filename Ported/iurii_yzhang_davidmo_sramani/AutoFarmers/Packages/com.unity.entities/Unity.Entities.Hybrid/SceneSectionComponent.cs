using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

[RequiresEntityConversion]
public class SceneSectionComponent : MonoBehaviour
{
    [FormerlySerializedAs("SectionId")] 
    public int         SectionIndex;
}
