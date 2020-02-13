using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public struct BitArray
{
    public NativeArray<uint> m_Data;
    public int m_CountBits;
    public bool IsCreated => m_Data.IsCreated;

    public void Allocate(int capacityBits, Allocator allocator)
    {
        m_CountBits = capacityBits;
        m_Data = new NativeArray<uint>((capacityBits + 31) / 32, allocator);
    }

    public void Dispose()
    {
        if (m_Data.IsCreated)
        {
            m_Data.Dispose();
        }
    }

    public void ClearAllBits()
    {
        for (int i = 0, iLen = m_Data.Length; i < iLen; ++i)
        {
            m_Data[i] = 0;
        }
    }

    public void SetBit(int indexBit)
    {
        int indexElement = ComputeIndexElement(indexBit);

        m_Data[indexElement] |= ComputeMaskFromIndexBit(indexBit);
    }

    public void ClearBit(int indexBit)
    {
        int indexElement = ComputeIndexElement(indexBit);
        
        m_Data[indexElement] &= ~ComputeMaskFromIndexBit(indexBit);
    }

    public bool IsBitSet(int indexBit)
    {
        int indexElement = ComputeIndexElement(indexBit);
        
        return (m_Data[indexElement] & ComputeMaskFromIndexBit(indexBit)) > 0;
    }

    public static int ComputeIndexElement(int indexBit)
    {
        return indexBit / 32;
    }

    public static uint ComputeMaskFromIndexBit(int indexBit)
    {
        return (uint)(1 << (indexBit % 32));
    }

}
