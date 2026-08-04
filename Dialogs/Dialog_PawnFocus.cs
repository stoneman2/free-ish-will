using RimWorld;
using Verse;

namespace FreeWill
{
    public class Dialog_PawnFocus : Dialog_FocusBase
    {
        private readonly Pawn pawn;

        public Dialog_PawnFocus(Pawn pawn)
        {
            this.pawn = pawn;
            FreeWill_WorldComponent worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            LoadFocus(worldComp?.GetPawnFocus(pawn));
        }

        protected override string Title => "FreeWillPawnFocusTitle".Translate(pawn?.LabelShort ?? "pawn").ToString();
        protected override string ClearedMessage => "FreeWillPawnFocusCleared".Translate(pawn?.LabelShort ?? "pawn").ToString();

        protected override string SetMessage(WorkTypeDef workType)
        {
            return "FreeWillPawnFocusSet".Translate(pawn?.LabelShort ?? "pawn", workType.labelShort).ToString();
        }

        protected override void SaveFocus(WorkTypeDef workType, float focusIntensity, float otherWorkMultiplier, int durationTicks, string selectedPresetKey)
        {
            FreeWill_WorldComponent worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            worldComp?.SetPawnFocus(pawn, workType, focusIntensity, otherWorkMultiplier, durationTicks, selectedPresetKey);
        }

        protected override void ClearFocus()
        {
            FreeWill_WorldComponent worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            worldComp?.ClearPawnFocus(pawn);
        }
    }
}
