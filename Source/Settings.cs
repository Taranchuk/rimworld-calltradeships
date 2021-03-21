using UnityEngine;
using Verse;

namespace CallTradeShips
{
    public class Settings : ModSettings
    {
        public static int Cost = 500;

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
        }
    }
}