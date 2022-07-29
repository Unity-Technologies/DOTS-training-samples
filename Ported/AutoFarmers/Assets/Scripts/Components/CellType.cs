using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

    enum CellState : byte
    {
        Raw,
        Tilled,
        Rock,
        Plant,
        HarvestablePlant,
        Silo
    }
    
    struct CellType : IBufferElementData
    {
        public CellState Value;

        public CellType(CellState value)
        {
            Value = value;
        }
    }

