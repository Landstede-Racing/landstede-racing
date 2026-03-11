using System;
using Unity.Netcode;

namespace LandstedeRacing.Types
{
    [Serializable]
    public class PlayerTiming : INetworkSerializeByMemcpy
    {
        public PlayerTiming(ulong networkId, long timing, int sectorId, int lap)
        {
            NetworkId = networkId;
            Timing = timing;
            SectorId = sectorId;
            Lap = lap;
        }

        public ulong NetworkId { get; private set; }
        public long Timing { get; private set; }
        public int SectorId { get; private set; }
        public int Lap { get; private set; }
    }
}