using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct PlayerSnapshotData : ISnapshotData<PlayerSnapshotData>
{
    public uint tick;
    private int PlayerComponentPlayerId;
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

    public void PredictDelta(uint tick, ref PlayerSnapshotData baseline1, ref PlayerSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        PlayerComponentPlayerId = predictor.PredictInt(PlayerComponentPlayerId, baseline1.PlayerComponentPlayerId, baseline2.PlayerComponentPlayerId);
    }

    public void Serialize(int networkId, ref PlayerSnapshotData baseline, DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (PlayerComponentPlayerId != baseline.PlayerComponentPlayerId) ? 1u : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(PlayerComponentPlayerId, baseline.PlayerComponentPlayerId, compressionModel);
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
    }
    public void Interpolate(ref PlayerSnapshotData target, float factor)
    {
    }
}
