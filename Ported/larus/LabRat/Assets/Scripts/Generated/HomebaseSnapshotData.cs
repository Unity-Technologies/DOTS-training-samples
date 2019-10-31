using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct HomebaseSnapshotData : ISnapshotData<HomebaseSnapshotData>
{
    public uint tick;
    private int HomebaseComponentColorX;
    private int HomebaseComponentColorY;
    private int HomebaseComponentColorZ;
    private int HomebaseComponentColorW;
    private int HomebaseComponentPlayerId;
    private int RotationValueX;
    private int RotationValueY;
    private int RotationValueZ;
    private int RotationValueW;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
    public float4 GetHomebaseComponentColor(GhostDeserializerState deserializerState)
    {
        return GetHomebaseComponentColor();
    }
    public float4 GetHomebaseComponentColor()
    {
        return new float4(HomebaseComponentColorX * 0.01f, HomebaseComponentColorY * 0.01f, HomebaseComponentColorZ * 0.01f, HomebaseComponentColorW * 0.01f);
    }
    public void SetHomebaseComponentColor(float4 val, GhostSerializerState serializerState)
    {
        SetHomebaseComponentColor(val);
    }
    public void SetHomebaseComponentColor(float4 val)
    {
        HomebaseComponentColorX = (int)(val.x * 100);
        HomebaseComponentColorY = (int)(val.y * 100);
        HomebaseComponentColorZ = (int)(val.z * 100);
        HomebaseComponentColorW = (int)(val.w * 100);
    }
    public int GetHomebaseComponentPlayerId(GhostDeserializerState deserializerState)
    {
        return (int)HomebaseComponentPlayerId;
    }
    public int GetHomebaseComponentPlayerId()
    {
        return (int)HomebaseComponentPlayerId;
    }
    public void SetHomebaseComponentPlayerId(int val, GhostSerializerState serializerState)
    {
        HomebaseComponentPlayerId = (int)val;
    }
    public void SetHomebaseComponentPlayerId(int val)
    {
        HomebaseComponentPlayerId = (int)val;
    }
    public quaternion GetRotationValue(GhostDeserializerState deserializerState)
    {
        return GetRotationValue();
    }
    public quaternion GetRotationValue()
    {
        return new quaternion(RotationValueX * 0.001f, RotationValueY * 0.001f, RotationValueZ * 0.001f, RotationValueW * 0.001f);
    }
    public void SetRotationValue(quaternion q, GhostSerializerState serializerState)
    {
        SetRotationValue(q);
    }
    public void SetRotationValue(quaternion q)
    {
        RotationValueX = (int)(q.value.x * 1000);
        RotationValueY = (int)(q.value.y * 1000);
        RotationValueZ = (int)(q.value.z * 1000);
        RotationValueW = (int)(q.value.w * 1000);
    }
    public float3 GetTranslationValue(GhostDeserializerState deserializerState)
    {
        return GetTranslationValue();
    }
    public float3 GetTranslationValue()
    {
        return new float3(TranslationValueX * 0.01f, TranslationValueY * 0.01f, TranslationValueZ * 0.01f);
    }
    public void SetTranslationValue(float3 val, GhostSerializerState serializerState)
    {
        SetTranslationValue(val);
    }
    public void SetTranslationValue(float3 val)
    {
        TranslationValueX = (int)(val.x * 100);
        TranslationValueY = (int)(val.y * 100);
        TranslationValueZ = (int)(val.z * 100);
    }

    public void PredictDelta(uint tick, ref HomebaseSnapshotData baseline1, ref HomebaseSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        HomebaseComponentColorX = predictor.PredictInt(HomebaseComponentColorX, baseline1.HomebaseComponentColorX, baseline2.HomebaseComponentColorX);
        HomebaseComponentColorY = predictor.PredictInt(HomebaseComponentColorY, baseline1.HomebaseComponentColorY, baseline2.HomebaseComponentColorY);
        HomebaseComponentColorZ = predictor.PredictInt(HomebaseComponentColorZ, baseline1.HomebaseComponentColorZ, baseline2.HomebaseComponentColorZ);
        HomebaseComponentColorW = predictor.PredictInt(HomebaseComponentColorW, baseline1.HomebaseComponentColorW, baseline2.HomebaseComponentColorW);
        HomebaseComponentPlayerId = predictor.PredictInt(HomebaseComponentPlayerId, baseline1.HomebaseComponentPlayerId, baseline2.HomebaseComponentPlayerId);
        RotationValueX = predictor.PredictInt(RotationValueX, baseline1.RotationValueX, baseline2.RotationValueX);
        RotationValueY = predictor.PredictInt(RotationValueY, baseline1.RotationValueY, baseline2.RotationValueY);
        RotationValueZ = predictor.PredictInt(RotationValueZ, baseline1.RotationValueZ, baseline2.RotationValueZ);
        RotationValueW = predictor.PredictInt(RotationValueW, baseline1.RotationValueW, baseline2.RotationValueW);
        TranslationValueX = predictor.PredictInt(TranslationValueX, baseline1.TranslationValueX, baseline2.TranslationValueX);
        TranslationValueY = predictor.PredictInt(TranslationValueY, baseline1.TranslationValueY, baseline2.TranslationValueY);
        TranslationValueZ = predictor.PredictInt(TranslationValueZ, baseline1.TranslationValueZ, baseline2.TranslationValueZ);
    }

    public void Serialize(int networkId, ref HomebaseSnapshotData baseline, DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (HomebaseComponentColorX != baseline.HomebaseComponentColorX ||
                                          HomebaseComponentColorY != baseline.HomebaseComponentColorY ||
                                          HomebaseComponentColorZ != baseline.HomebaseComponentColorZ ||
                                          HomebaseComponentColorW != baseline.HomebaseComponentColorW) ? 1u : 0;
        changeMask0 |= (HomebaseComponentPlayerId != baseline.HomebaseComponentPlayerId) ? (1u<<1) : 0;
        changeMask0 |= (RotationValueX != baseline.RotationValueX ||
                                           RotationValueY != baseline.RotationValueY ||
                                           RotationValueZ != baseline.RotationValueZ ||
                                           RotationValueW != baseline.RotationValueW) ? (1u<<2) : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<3) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
        {
            writer.WritePackedIntDelta(HomebaseComponentColorX, baseline.HomebaseComponentColorX, compressionModel);
            writer.WritePackedIntDelta(HomebaseComponentColorY, baseline.HomebaseComponentColorY, compressionModel);
            writer.WritePackedIntDelta(HomebaseComponentColorZ, baseline.HomebaseComponentColorZ, compressionModel);
            writer.WritePackedIntDelta(HomebaseComponentColorW, baseline.HomebaseComponentColorW, compressionModel);
        }
        if ((changeMask0 & (1 << 1)) != 0)
            writer.WritePackedIntDelta(HomebaseComponentPlayerId, baseline.HomebaseComponentPlayerId, compressionModel);
        if ((changeMask0 & (1 << 2)) != 0)
        {
            writer.WritePackedIntDelta(RotationValueX, baseline.RotationValueX, compressionModel);
            writer.WritePackedIntDelta(RotationValueY, baseline.RotationValueY, compressionModel);
            writer.WritePackedIntDelta(RotationValueZ, baseline.RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(RotationValueW, baseline.RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 3)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref HomebaseSnapshotData baseline, DataStreamReader reader, ref DataStreamReader.Context ctx,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(ref ctx, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
        {
            HomebaseComponentColorX = reader.ReadPackedIntDelta(ref ctx, baseline.HomebaseComponentColorX, compressionModel);
            HomebaseComponentColorY = reader.ReadPackedIntDelta(ref ctx, baseline.HomebaseComponentColorY, compressionModel);
            HomebaseComponentColorZ = reader.ReadPackedIntDelta(ref ctx, baseline.HomebaseComponentColorZ, compressionModel);
            HomebaseComponentColorW = reader.ReadPackedIntDelta(ref ctx, baseline.HomebaseComponentColorW, compressionModel);
        }
        else
        {
            HomebaseComponentColorX = baseline.HomebaseComponentColorX;
            HomebaseComponentColorY = baseline.HomebaseComponentColorY;
            HomebaseComponentColorZ = baseline.HomebaseComponentColorZ;
            HomebaseComponentColorW = baseline.HomebaseComponentColorW;
        }
        if ((changeMask0 & (1 << 1)) != 0)
            HomebaseComponentPlayerId = reader.ReadPackedIntDelta(ref ctx, baseline.HomebaseComponentPlayerId, compressionModel);
        else
            HomebaseComponentPlayerId = baseline.HomebaseComponentPlayerId;
        if ((changeMask0 & (1 << 2)) != 0)
        {
            RotationValueX = reader.ReadPackedIntDelta(ref ctx, baseline.RotationValueX, compressionModel);
            RotationValueY = reader.ReadPackedIntDelta(ref ctx, baseline.RotationValueY, compressionModel);
            RotationValueZ = reader.ReadPackedIntDelta(ref ctx, baseline.RotationValueZ, compressionModel);
            RotationValueW = reader.ReadPackedIntDelta(ref ctx, baseline.RotationValueW, compressionModel);
        }
        else
        {
            RotationValueX = baseline.RotationValueX;
            RotationValueY = baseline.RotationValueY;
            RotationValueZ = baseline.RotationValueZ;
            RotationValueW = baseline.RotationValueW;
        }
        if ((changeMask0 & (1 << 3)) != 0)
        {
            TranslationValueX = reader.ReadPackedIntDelta(ref ctx, baseline.TranslationValueX, compressionModel);
            TranslationValueY = reader.ReadPackedIntDelta(ref ctx, baseline.TranslationValueY, compressionModel);
            TranslationValueZ = reader.ReadPackedIntDelta(ref ctx, baseline.TranslationValueZ, compressionModel);
        }
        else
        {
            TranslationValueX = baseline.TranslationValueX;
            TranslationValueY = baseline.TranslationValueY;
            TranslationValueZ = baseline.TranslationValueZ;
        }
    }
    public void Interpolate(ref HomebaseSnapshotData target, float factor)
    {
        SetRotationValue(math.slerp(GetRotationValue(), target.GetRotationValue(), factor));
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
    }
}
