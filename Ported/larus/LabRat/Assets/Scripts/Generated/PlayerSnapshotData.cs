using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct PlayerSnapshotData : ISnapshotData<PlayerSnapshotData>
{
    public uint tick;
    private int PlayerComponentPlayerId;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
    public int GetPlayerComponentPlayerId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerComponentPlayerId;
    }
    public int GetPlayerComponentPlayerId()
    {
        return (int)PlayerComponentPlayerId;
    }
    public void SetPlayerComponentPlayerId(int val, GhostSerializerState serializerState)
    {
        PlayerComponentPlayerId = (int)val;
    }
    public void SetPlayerComponentPlayerId(int val)
    {
        PlayerComponentPlayerId = (int)val;
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

    public void PredictDelta(uint tick, ref PlayerSnapshotData baseline1, ref PlayerSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        PlayerComponentPlayerId = predictor.PredictInt(PlayerComponentPlayerId, baseline1.PlayerComponentPlayerId, baseline2.PlayerComponentPlayerId);
        TranslationValueX = predictor.PredictInt(TranslationValueX, baseline1.TranslationValueX, baseline2.TranslationValueX);
        TranslationValueY = predictor.PredictInt(TranslationValueY, baseline1.TranslationValueY, baseline2.TranslationValueY);
        TranslationValueZ = predictor.PredictInt(TranslationValueZ, baseline1.TranslationValueZ, baseline2.TranslationValueZ);
    }

    public void Serialize(int networkId, ref PlayerSnapshotData baseline, DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (PlayerComponentPlayerId != baseline.PlayerComponentPlayerId) ? 1u : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<1) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(PlayerComponentPlayerId, baseline.PlayerComponentPlayerId, compressionModel);
        if ((changeMask0 & (1 << 1)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref PlayerSnapshotData baseline, DataStreamReader reader, ref DataStreamReader.Context ctx,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(ref ctx, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            PlayerComponentPlayerId = reader.ReadPackedIntDelta(ref ctx, baseline.PlayerComponentPlayerId, compressionModel);
        else
            PlayerComponentPlayerId = baseline.PlayerComponentPlayerId;
        if ((changeMask0 & (1 << 1)) != 0)
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
    public void Interpolate(ref PlayerSnapshotData target, float factor)
    {
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
    }
}
