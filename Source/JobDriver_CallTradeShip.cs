using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using static CallTradeShips.Job_CallTradeShip;

namespace CallTradeShips
{
    internal class JobDriver_CallTradeShip : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            yield return new Toil
            {
                initAction = delegate
                {
                    var job = this.job as Job_CallTradeShip;
                    if (job == null)
                    {
                        Log.Error("CallTradeShips: Job is not of type Job_CallTradeShip.");
                        return;
                    }

                    ThingWithComps twc = job.targetA.Thing as ThingWithComps;
                    if (twc?.def?.IsCommsConsole == false)
                    {
                        Log.Error("CallTradeShips: Target is not a CommsConsole");
                        return;
                    }

                    if (!Util.HasEnoughSilver(twc.Map, out int found))
                    {
                        Log.Warning("Not enough silver");
                        return;
                    }
                    TradeUtility.LaunchSilver(twc.Map, Settings.Cost);

                    if (job.TraderKind == TraderKindEnum.Orbital)
                    {
                        if (twc.GetComp<CompPowerTrader>()?.PowerOn == true)
                        {
                            TradeShip tradeShip = new TradeShip(job.TraderKindDef);
                            twc.Map.passingShipManager.AddShip(tradeShip);
                            tradeShip.GenerateThings();
                            Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(tradeShip.name, tradeShip.def.label, "TraderArrivalNoFaction".Translate()), LetterDefOf.PositiveEvent, null);
                        }
                    }
                    else if (job.TraderKind == TraderKindEnum.Lander)
                    {
                        foreach (var id in DefDatabase<IncidentDef>.AllDefsListForReading)
                        {
                            if (id.Worker is IncidentWorker_OrbitalTraderArrival)
                            {
                                if (id.Worker.TryExecute(new IncidentParms() { target = twc.Map }))
                                {
                                    return;
                                }
                                break;
                            }
                        }
                        Log.Error("CallTradShips failed to create trade ship from mod TraderShips");
                        return;
                    }
                    else
                    {
                        Log.Error("CallTradeShips: Unknown TraderKindEnum: " + job.TraderKind);
                        return;
                    }
                }
            };
            yield break;
        }
    }
}
