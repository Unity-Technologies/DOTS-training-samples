// Contents of CellInfo
// - bottom 4 bits GridDirection
// - is hole -> 1 bit
// - is base -> 1 bit
// - above that is owning player id for bases -> which player owns a base
using System.Runtime.CompilerServices;

public struct CellInfo
{
    const byte k_IsHoleFlag = (1 << 4);
    const byte k_IsBaseFlag = (1 << 5);

    const byte k_TravelMask = 0xf;
    const byte k_BasePlayerIdMask = 0b11000000;
    const byte k_BasePlayerIdShift = 6;

    // - bottom 4 bits GridDirection
    // - is hole -> 1 bit
    // - is base -> 1 bit
    // - player base owner -> 2 bit
    byte m_Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CellInfo SetTravelDirections(GridDirection dir)
    {
        m_Value = (byte)((m_Value & ~k_TravelMask) | (~(byte)dir & k_TravelMask));
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CellInfo SetIsHole()
    {
        m_Value = (byte)(m_Value | k_IsHoleFlag);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CellInfo ClearIsHole(bool isHole)
    {
        m_Value = (byte)(m_Value & ~k_IsHoleFlag);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanTravel(GridDirection dir)
    {
        return (m_Value & (byte)dir) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsHole()
    {
        return (m_Value & k_IsHoleFlag) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsBase()
    {
        return (m_Value & k_IsBaseFlag) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetBasePlayerId()
    {
        return (m_Value & k_BasePlayerIdMask) >> k_BasePlayerIdShift;
    }
}