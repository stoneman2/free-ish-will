using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FreeWill.Patches
{
    /// <summary>
    /// Injects FreeWill columns into the Work pawn table at game startup.
    /// </summary>
    [HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
    public static class DefGeneratorPatch
    {
        /// <summary>
        /// Postfix that inserts the FreeWillToggle column after the "Label" column.
        /// </summary>
        public static void Postfix()
        {
            try
            {
                // Find the Label column in the Work table
                int labelIndex = PawnTableDefOf.Work.columns.FindIndex(
                    x => x.defName.Equals("Label", StringComparison.Ordinal)
                );
                
                if (labelIndex < 0)
                {
                    labelIndex = 0;
                }
                var freeWillToggleColumn = DefDatabase<PawnColumnDef>.GetNamed("FreeWillToggle", false);
                if (freeWillToggleColumn == null)
                {
                    return;
                }
                
                PawnTableDefOf.Work.columns.Insert(labelIndex + 1, freeWillToggleColumn);
            }
            catch (Exception ex)
            {
                Log.Error($"Free Will: Error in DefGeneratorPatch: {ex.Message}");
            }
        }
    }
}
