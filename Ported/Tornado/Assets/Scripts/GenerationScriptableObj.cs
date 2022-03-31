using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "GenerationParameters" , menuName =  "Data/GenParams")]
    public class GenerationScriptableObj : ScriptableObject
    {
        public Texture2D spawnMap;
    }
}
