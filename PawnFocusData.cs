using RimWorld;
using Verse;

namespace FreeWill
{
    /// <summary>
    /// Stores focus data for a pawn or global focus.
    /// </summary>
    public class PawnFocusData : IExposable
    {
        /// <summary>
        /// The work type being focused.
        /// </summary>
        public WorkTypeDef WorkType;
        
        /// <summary>
        /// Intensity multiplier for FOCUSED work (1.0 to 5.0).
        /// </summary>
        public float Intensity = 3.0f;
        
        /// <summary>
        /// Multiplier for NON-FOCUSED work (0.0 to 1.0).
        /// 1.0 = no reduction, 0.5 = half priority, 0.0 = disabled.
        /// </summary>
        public float DefocusMultiplier = 0.33f;

        /// <summary>
        /// Duration selected by the player. Zero means until cleared.
        /// </summary>
        public int DurationTicks = 0;

        /// <summary>
        /// Game tick when this focus expires. Zero means until cleared.
        /// </summary>
        public int ExpiresAtTick = 0;

        /// <summary>
        /// Preset selected in the focus menu.
        /// </summary>
        public string PresetKey = "Custom";

        public void ExposeData()
        {
            Scribe_Defs.Look(ref WorkType, "workType");
            Scribe_Values.Look(ref Intensity, "intensity", 3.0f);
            Scribe_Values.Look(ref DefocusMultiplier, "defocusMultiplier", 0.33f);
            Scribe_Values.Look(ref DurationTicks, "durationTicks", 0);
            Scribe_Values.Look(ref ExpiresAtTick, "expiresAtTick", 0);
            Scribe_Values.Look(ref PresetKey, "presetKey", "Custom");
        }

        public bool IsExpired(int currentTick)
        {
            return ExpiresAtTick > 0 && currentTick >= ExpiresAtTick;
        }
    }
}
