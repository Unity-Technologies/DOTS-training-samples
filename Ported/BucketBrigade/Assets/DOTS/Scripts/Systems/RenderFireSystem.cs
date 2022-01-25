using Unity.Entities;
using Unity.Rendering;

public partial class RenderFireSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref FireField fireField) =>
        {
            var field = GetBuffer<FireHeat>(e);

        }).Schedule();
    }
    
   /* void Update()
    {
            Vector3 _POS = t.localPosition;
            _POS.y = (-flameHeight*0.5f + (temperature * flameHeight)) - flickerRange;
            //_POS.y += (flickerRange*0.5f) + Mathf.PerlinNoise((Time.time -index)* flickerRate - temperature,index) * flickerRange;
            _POS.y += (flickerRange * 0.5f) + Mathf.PerlinNoise((Time.time - index) * flickerRate - temperature, temperature) * flickerRange;
            t.localPosition = _POS;
            SetColour(Color.Lerp(fireSim.colour_fireCell_cool, fireSim.colour_fireCell_hot, temperature));
    }*/
}
