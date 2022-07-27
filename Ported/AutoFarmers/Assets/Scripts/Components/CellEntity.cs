using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

   struct CellEntity  : IBufferElementData
    {
        public Entity Value;

        public CellEntity (Entity value)
        {
            Value = value;
        }
    }