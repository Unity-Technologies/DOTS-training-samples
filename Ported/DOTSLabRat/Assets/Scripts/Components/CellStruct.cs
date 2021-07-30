using System;
using Unity.Entities;

namespace DOTSRATS
{
    public struct CellStruct : IBufferElementData
    {
        byte m_Byte;
        
        public Direction wallLayout
        {
            get => (Direction)(m_Byte & (byte)0xF);
            set => m_Byte = (byte)((byte)(m_Byte & (byte)0xF0) | (byte)value);
        }

        public Direction arrow
        {
            get => (m_Byte & (byte)0xF0) == 3 ? Direction.None : (Direction)((m_Byte & (byte)0xF0) >> 4);
            set => m_Byte = (byte)((byte)(m_Byte & (byte)0xF) | (byte)(((byte)value) << 4));
        }

        public bool hole
        {
            get => (m_Byte & (byte)0xF0) == 7 << 4;
            set => m_Byte = (byte)((byte)(m_Byte & (byte)0xF) | (value ? 7 : 3) << 4);
        }

        public bool goal
        {
            get => (m_Byte & (byte) 0xF0) == 11 << 4;
            set => m_Byte = (byte)((byte)(m_Byte & (byte)0xF) | (value ? 11 : 3) << 4);
        }

        public byte goalPlayerNumber
        {
            get => (byte)(m_Byte & (byte)0xF);
            set => m_Byte = (byte)((byte)(m_Byte & (byte)0xF0) | value);
        }
    }
}
