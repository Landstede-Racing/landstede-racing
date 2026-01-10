using System.Linq;
using LandstedeRacing.Types;

public class PlayerInfoUtils
{
    public static PlayerInfo StatsToInfo(PlayerStats player)
    {
        PlayerTiming lastTiming = player.playerTimings[^1];
        float currentLapTime = player.playerTimings.FindAll((t) => t.Lap == lastTiming.Lap).Sum((t) => t.Timing);
        
        return new PlayerInfo(player.position, player.name, lastTiming.Timing, player.tire, lastTiming.Lap, currentLapTime);
    }
}