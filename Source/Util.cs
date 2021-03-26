using RimWorld;
using Verse;

namespace CallTradeShips
{
 static class Util
    {
        public static bool HasEnoughSilver(Map map, out int found)
        {
            found = 0;

            int need = Settings.Cost;
            if (need == 0)
                return true;

            foreach (Thing t in TradeUtility.AllLaunchableThingsForTrade(map))
            {
                if (t.def == ThingDefOf.Silver)
                {
                    found += t.stackCount;
                    if (found >= Settings.Cost)
                        return true;
                }
            }
            return false;
        }
    }
}
