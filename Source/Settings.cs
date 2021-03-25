using System.Reflection;
using UnityEngine;
using Verse;

namespace CallTradeShips
{
    public class Settings : ModSettings
    {
        public static int Cost = 500;
        public static bool AllowOrbitalTraders_ForTraderShipsMod = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Cost, "CallTradeShip.Cost", 500);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (Cost < 0)
                    Cost = 0;
                else if (Cost > 100000)
                    Cost = 100000;
                SettingsController.CostBuffer = Cost.ToString();
            }
        }

        private static bool isTradeShipInitialized = false;
        private static bool isUsingTraderShipsMod = false;
        public static bool IsUsingTraderShipsMod()
        {
            if (!isTradeShipInitialized)
            {
                foreach (ModContentPack pack in LoadedModManager.RunningMods)
                {
                    foreach (Assembly assembly in pack.assemblies.loadedAssemblies)
                    {
                        if (assembly.GetName().Name.Equals("TraderShips"))
                        {
                            isUsingTraderShipsMod = true;
                            break;
                        }
                    }
                    if (isUsingTraderShipsMod)
                    {
                        break;
                    }
                }
            }
            return isUsingTraderShipsMod;
        }
    }

    public class SettingsController : Mod
    {
        public static string CostBuffer = "500";
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }
        public override string SettingsCategory()
        {
            return "CallTradeShips.ModName".Translate();
        }
        public override void DoSettingsWindowContents(Rect r)
        {
            Widgets.Label(new Rect(r.xMin, r.yMin, 100, 32), "CallTradeShips.Cost".Translate());
            CostBuffer = Widgets.TextField(new Rect(r.xMin + 110, r.yMin, 100, 32), CostBuffer);
            if (int.TryParse(CostBuffer, out int i))
            {
                if (i > 0 && i < 100000)
                {
                    Settings.Cost = i;
                }
            }
            else if (CostBuffer.Trim() == "")
                Settings.Cost = 0;

            if (Settings.IsUsingTraderShipsMod())
            {
                Widgets.Label(new Rect(r.xMin, r.yMin + 50, 200, 32), "CallTradeShips.AllowOrbitalTraders".Translate());
                Widgets.Checkbox(r.xMin + 210, r.yMin + 50, ref Settings.AllowOrbitalTraders_ForTraderShipsMod);
            }
        }
    }
}