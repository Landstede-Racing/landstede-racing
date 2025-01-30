using Unity.Netcode;

// namespace LandstedeRacing.Types
// {
    [System.Serializable]
    public struct PlayerInfo : INetworkSerializable
    {
        public int position;
        public string shortName;
        public float time;
        public string tire;
        
        public PlayerInfo(int pos, string sName, float tm, string tr) {
            position = pos;
            shortName = sName;
            time = tm;
            tire = tr;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref shortName);
            serializer.SerializeValue(ref time);
            serializer.SerializeValue(ref tire);
        }
    }
// }