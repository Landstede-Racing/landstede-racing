using System.Collections.Generic;
using LandstedeRacing.Types;

namespace LandstedeRacing.Types
{
    public class PlayerTimingList : List<PlayerTiming>
    {
        public int ListSector { get; private set; }

        public PlayerTimingList(int listSector)
        {
            ListSector = listSector;
        }
    }
}