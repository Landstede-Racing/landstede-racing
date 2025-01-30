using System.Collections.Generic;

namespace LandstedeRacing.Types
{
    public class PlayerTimingList : List<PlayerTiming>
    {
        public PlayerTimingList(int listSector)
        {
            ListSector = listSector;
        }

        public int ListSector { get; private set; }
    }
}