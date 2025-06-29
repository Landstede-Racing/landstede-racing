using Unity.Netcode;
using Unity.Collections;

using System;

public struct PlayerInfo : INetworkSerializable, IEquatable<PlayerInfo>
{
    public int position;
    public FixedString64Bytes shortName;
    public float time;
    public FixedString64Bytes tire;

    public PlayerInfo(int position, string shortName, float time, string tire)
    {
        this.position = position;
        this.shortName = shortName;
        this.time = time;
        this.tire = tire;
    }

    public bool Equals(PlayerInfo other)
    {
        return position == other.position &&
               shortName == other.shortName &&
               time.Equals(other.time) &&
               tire == other.tire;
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(position, shortName, time, tire);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref shortName);
        serializer.SerializeValue(ref time);
        serializer.SerializeValue(ref tire);
    }
}