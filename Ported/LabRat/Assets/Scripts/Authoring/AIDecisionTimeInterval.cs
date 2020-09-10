using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Entities;

/// <summary>
/// Min time / max time between CPU players decisions
/// </summary>
[GenerateAuthoringComponent]
public struct AIDecisionTimeInterval : IComponentData
{
    public int MinMiliseconds;
    public int MaxMiliseconds;
}