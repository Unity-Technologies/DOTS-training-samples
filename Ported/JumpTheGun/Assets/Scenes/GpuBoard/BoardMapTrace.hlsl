
void BoardTraceMarch_float(
    UnityTexture2D Heightmap,
    UnitySamplerState PointSampler,
    float3 CamPos,
    float3 WorldPos,
    out float4 OutputColor,
    out float3 OutputNormal)
{
    float2 uvD = (1.0f / 256.0f).xx;

    float3 dirVector = CamPos - WorldPos;
    float3 dirVectorNorm = normalize(dirVector);
    float incY = dirVectorNorm.y;
    float3 p = WorldPos;
    
    float3 bestP = p;
    float bestH = -1.0;
    float bestHeight = -1.0;
    float dInc = 0.01;
    for (int i = 0; i < 50; ++i)
    {
        float h = Heightmap.SampleLevel(PointSampler, p.xz * uvD, 0).x ;
        float height = h * 2.0;
        if (p.y <= height)
        {
            bestHeight = height;
            bestH = h;
            bestP = p;
        }

        p = p + dirVector * dInc;
    }

    float normUp = all(frac(bestP.xz) >= float2(0.01.xx)) || all(frac(bestP.xz) <= float2(0.99.xx)) ? 1.0 : 0.0;

    float inZEdge = frac(bestP.x) < 0.01 || frac(bestP.x) > 0.99 ? 1 : 0;
    float inXEdge = frac(bestP.z) < 0.01 || frac(bestP.z) > 0.99 ? 1 : 0;
    float xN = dirVector.x < 0.0 ? -1.0 : 1.0;
    float zN = dirVector.z < 0.0 ? -1.0 : 1.0;

    OutputColor = lerp(float4(1.0,1.0,0.0,1.0), float4(0.2, 0.2, 0.0, 1.0), bestH);
    OutputNormal = normalize(float3(normUp > 0.0 ? 0.0 : xN, normUp, normUp > 0.0 ? 0.0 : zN));
}