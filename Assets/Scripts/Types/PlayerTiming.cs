using System;
using Unity.Netcode;

namespace LandstedeRacing.Types
{
    [Serializable]
    public class PlayerTiming : INetworkSerializeByMemcpy
    {
        public PlayerTiming(ulong networkId, long timing, int sectorId, long sectorTimestamp, int lap)
        {
            NetworkId = networkId;
            Timing = timing;
            SectorId = sectorId;
            SectorTimestamp = sectorTimestamp;
            Lap = lap;
        }

        public ulong NetworkId { get; private set; }
        public long Timing { get; private set; }
        public int SectorId { get; private set; }
        public long SectorTimestamp { get; private set; }
        public int Lap { get; private set; }
    }
}