using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum GridDirection
{
    NORTH = (1 << 0),
    EAST = (1 << 1),
    SOUTH = (1 << 2),
    WEST = (1 << 3)
}

// Contents of CellInfo
// - bottom 4 bits GridDirection
// - is hole -> 1 bit
// - is base -> 1 bit
// - above that is owning player id for bases -> which player owns a base
struct CellInfo
{
    const byte k_IsHoleFlag = (1 << 4);
    const byte k_IsBaseFlag = (1 << 5);

    const byte k_BasePlayerIdMask = 0b11000000;
    const byte k_BasePlayerIdShift = 6;

    // - bottom 4 bits GridDirection
    // - is hole -> 1 bit
    // - is base -> 1 bit
    // - player base owner -> 2 bit
    byte m_Value;


    bool CanTravel(GridDirection dir)
    {
        return (m_Value & (byte)dir) > 0;
    }

    bool IsHole()
    {
        return (m_Value & k_IsHoleFlag) > 0;
    }

    bool IsBase()
    {
        return (m_Value & k_IsBaseFlag) > 0;
    }

    int GetBasePlayerId()
    {
        return (m_Value & k_BasePlayerIdMask) >> k_BasePlayerIdShift;
    }
}