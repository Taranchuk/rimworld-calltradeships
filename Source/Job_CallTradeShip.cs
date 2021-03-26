using RimWorld;
using Verse;
using Verse.AI;

namespace CallTradeShips
{
    class Job_CallTradeShip : Job
    {
        public enum TraderKindEnum
        {
            Orbital,
            Lander
        }

        public readonly TraderKindDef TraderKindDef;
        public readonly TraderKindEnum TraderKind;
        public Job_CallTradeShip(JobDef jobDef, LocalTargetInfo targetA, TraderKindDef traderKindDef, TraderKindEnum traderKind) : base(jobDef, targetA)
        {
            this.TraderKindDef = traderKindDef;
            this.TraderKind = traderKind;
        }
    }
}
