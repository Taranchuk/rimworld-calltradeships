using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CallTradeShips
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.calltradeships.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    static class Patch_FloatMenuMakerMap_AddHumanlikeOrders
    {
        private static List<TraderKindDef> orbitalTraders = new List<TraderKindDef>();

        [HarmonyPriority(Priority.Last)]
        static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            foreach (Thing t in IntVec3.FromVector3(clickPos).GetThingList(pawn.Map))
            {
                if (t is Building_CommsConsole)
                    addOptions(pawn.Map, opts);
            }
        }

        static void addOptions(Map map, List<FloatMenuOption> opts)
        {
            if (orbitalTraders.Count == 0)
            {
                foreach (var d in DefDatabase<TraderKindDef>.AllDefsListForReading)
                {
                    if (d.orbital)
                        orbitalTraders.Add(d);
                }
                orbitalTraders.Sort(delegate (TraderKindDef d1, TraderKindDef d2)
                {
                    return d1.label.CompareTo(d2.label);
                });
            }

            if (Settings.Cost > 0)
            {
                int found = 0;
                foreach (Thing t in TradeUtility.AllLaunchableThingsForTrade(map))
                {
                    if (t.def == ThingDefOf.Silver)
                    {
                        found += t.stackCount;
                        if (found > Settings.Cost)
                            break;
                    }
                }
                if (found < Settings.Cost)
                    return;
            }

            foreach (var d in orbitalTraders)
            {
                opts.Add(new FloatMenuOption(getMenuLabel(d), delegate ()
                {
                    TradeShip tradeShip = new TradeShip(d);
                    if (map.listerBuildings.allBuildingsColonist.Any((Building b) => b.def.IsCommsConsole && b.GetComp<CompPowerTrader>().PowerOn))
                    {
                        TradeUtility.LaunchSilver(map, Settings.Cost);
                        Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(
                            tradeShip.name,
                            tradeShip.def.label,
                            "TraderArrivalNoFaction".Translate()
                        ), LetterDefOf.PositiveEvent, null);
                    }
                    map.passingShipManager.AddShip(tradeShip);
                    tradeShip.GenerateThings();
                }, MenuOptionPriority.Low));
            }
        }

        static string getMenuLabel(TraderKindDef d)
        {
            if (Settings.Cost > 0)
                return "CallTradeShips.CallWithCost".Translate(d.LabelCap, Settings.Cost);
            return "CallTradeShips.Call".Translate(d.LabelCap);
        }
    }
}
