using RimWorld;
using Verse;
using FreeWill;

public class ThoughtWorker_Precept_EnforcedWorkSchedule : ThoughtWorker_Precept
{
    protected override ThoughtState ShouldHaveThought(Pawn pawn)
    {
        FreeWill_WorldComponent worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
        if (worldComp == null || !worldComp.Settings.ConsiderIdeology || !worldComp.CanManagePawn(pawn))
        {
            return ThoughtState.Inactive;
        }
        return worldComp.HasFreeWill(pawn, pawn.GetUniqueLoadID()) ? ThoughtState.Inactive : ThoughtState.ActiveDefault;
    }
}
