using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Hybrid
{
    public class CursorController : MonoBehaviour
    {
        public Image[] Cursors;
        private Canvas parentCanvas;

        public void Awake()
        {
            this.parentCanvas = this.Cursors[0].GetComponentInParent<Canvas>();
        }

        public void SetPosition(int i, float2 position, float4 color)
        {
            if (this.parentCanvas == null)
                return;

            var positionVec2 = new Vector2(position.x, position.y);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.parentCanvas.transform as RectTransform,
                positionVec2,
                this.parentCanvas.worldCamera,
                out Vector2 pos);

            this.Cursors[i].transform.position = this.parentCanvas.transform.TransformPoint(pos);

            this.Cursors[i].color = new UnityEngine.Color(color.x, color.y, color.z);
        }

        public void Update()
        {
            if (this.parentCanvas == null)
            {
                this.parentCanvas = this.Cursors[0].GetComponentInParent<Canvas>();
            }

            //RectTransformUtility.ScreenPointToLocalPointInRectangle();
            //Cursors[0].re;
            //Cursors[0];
            //Cursors[0];
            //Cursors[0];
        }
    }
}