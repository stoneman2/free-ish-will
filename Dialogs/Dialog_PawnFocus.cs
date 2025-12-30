using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FreeWill
{
    // why cant i just use the global focus dialog again? because im lazy
    public class Dialog_PawnFocus : Window
    {
        private readonly Pawn pawn;
        private Vector2 scrollPosition;
        private float intensity = 3.0f;
        private float defocusMultiplier = 0.33f;
        private WorkTypeDef selectedWorkType = null;
        
        public Dialog_PawnFocus(Pawn pawn)
        {
            this.pawn = pawn;
            doCloseButton = false;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            draggable = true;
            var worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            var existingFocus = worldComp?.GetFocusForPawn(pawn);
            if (existingFocus != null && worldComp?.GlobalFocus == null)
            {
                // Only load pawn-specific focus, not global
                string pawnKey = pawn.GetUniqueLoadID();
                if (worldComp.pawnFocuses?.ContainsKey(pawnKey) == true)
                {
                    selectedWorkType = existingFocus.WorkType;
                    intensity = existingFocus.Intensity;
                    defocusMultiplier = existingFocus.DefocusMultiplier;
                }
            }
        }
        
        public override Vector2 InitialSize => new Vector2(400f, 540f);
        
        // the wacky menu
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, inRect.width, 30), "FreeWillPawnFocusTitle".Translate(pawn.LabelShort));
            Text.Font = GameFont.Small;
            float y = 40f;
            Widgets.Label(new Rect(0, y, 150, 24), "FreeWillFocusIntensity".TranslateSimple());
            Widgets.Label(new Rect(160, y, 100, 24), $"{intensity:F1}x boost");
            y += 24;
            intensity = Widgets.HorizontalSlider(new Rect(0, y, inRect.width - 20, 20), intensity, 1.0f, 5.0f, true);
            y += 30;
            Widgets.Label(new Rect(0, y, 150, 24), "FreeWillDefocusIntensity".TranslateSimple());
            string defocusLabel = defocusMultiplier < 0.01f ? "Disabled" : $"{defocusMultiplier:P0}";
            Widgets.Label(new Rect(160, y, 100, 24), defocusLabel);
            y += 24;
            defocusMultiplier = Widgets.HorizontalSlider(new Rect(0, y, inRect.width - 20, 20), defocusMultiplier, 0.0f, 1.0f, true);
            y += 40;
            Widgets.Label(new Rect(0, y, inRect.width, 24), "FreeWillSelectWorkType".TranslateSimple());
            y += 28;
            Rect scrollRect = new Rect(0, y, inRect.width, inRect.height - y - 60);
            List<WorkTypeDef> workTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
            Rect viewRect = new Rect(0, 0, scrollRect.width - 16, workTypes.Count * 28);
            Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect);
            float listY = 0;
            foreach (var workType in workTypes)
            {
                Rect rowRect = new Rect(0, listY, viewRect.width, 26); 
                if (selectedWorkType == workType)
                {
                    Widgets.DrawHighlightSelected(rowRect);
                }
                else if (Mouse.IsOver(rowRect))
                {
                    Widgets.DrawHighlight(rowRect);
                }
                bool isSelected = selectedWorkType == workType;
                if (Widgets.RadioButtonLabeled(rowRect, workType.labelShort.CapitalizeFirst(), isSelected))
                {
                    selectedWorkType = workType;
                }
                
                listY += 28;
            }
            Widgets.EndScrollView();
            float buttonY = inRect.height - 35;
            if (Widgets.ButtonText(new Rect(0, buttonY, 120, 30), "FreeWillApplyFocus".TranslateSimple()))
            {
                ApplyPawnFocus();
            }
            
            if (Widgets.ButtonText(new Rect(130, buttonY, 120, 30), "FreeWillClearFocus".TranslateSimple()))
            {
                ClearPawnFocus();
            }
            
            if (Widgets.ButtonText(new Rect(260, buttonY, 100, 30), "FreeWillCancel".TranslateSimple()))
            {
                Close();
            }
        }
        
        private void ApplyPawnFocus()
        {
            if (selectedWorkType == null)
            {
                Messages.Message("FreeWillNoWorkTypeSelected".TranslateSimple(), MessageTypeDefOf.RejectInput);
                return;
            }
            
            var worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            if (worldComp != null)
            {
                worldComp.SetPawnFocus(pawn, selectedWorkType, intensity, defocusMultiplier);
                Messages.Message("FreeWillPawnFocusSet".Translate(pawn.LabelShort, selectedWorkType.labelShort), MessageTypeDefOf.PositiveEvent);
            }
            Close();
        }
        
        private void ClearPawnFocus()
        {
            var worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            worldComp?.ClearPawnFocus(pawn);
            Messages.Message("FreeWillPawnFocusCleared".Translate(pawn.LabelShort), MessageTypeDefOf.SilentInput);
            Close();
        }
    }
}
