using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TornadoMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        
        var tornado = GetSingletonEntity<Tornado>();
        var tornadoMovement = GetComponent<TornadoMovement>(tornado);
        var translation = GetComponent<Translation>(tornado);

        translation.Value.x = (float) math.cos(time * tornadoMovement.XFrequency) * tornadoMovement.Amplitude;
        translation.Value.z = (float) math.sin(time * tornadoMovement.ZFrequency) * tornadoMovement.Amplitude;
        
        SetComponent(tornado, translation);
    }
}