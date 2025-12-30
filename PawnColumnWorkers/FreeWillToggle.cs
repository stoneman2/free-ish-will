using RimWorld;
using UnityEngine;
using Verse;

namespace FreeWill.PawnColumnWorkers
{
    public class FreeWillToggle : PawnColumnWorker
    {
        private const float IconSize = 16f;
        
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.Dead || pawn.workSettings == null || !pawn.workSettings.EverWork)
            {
                return;
            }
            
            var worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            if (worldComp == null)
            {
                return;
            }
            
            string pawnKey = pawn.GetUniqueLoadID();
            bool hasFreeWill = worldComp.HasFreeWill(pawn, pawnKey);
            bool canChange = worldComp.FreeWillCanChange(pawn, pawnKey);
            
            Rect iconRect = new Rect(
                rect.center.x - IconSize / 2f,
                rect.center.y - IconSize / 2f,
                IconSize,
                IconSize
            );
            
            GUI.color = canChange ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            
            Texture2D icon = hasFreeWill ? FreeWillResources.FreeWillOn : FreeWillResources.FreeWillOff;
            
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && rect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                Find.WindowStack.Add(new Dialog_PawnFocus(pawn));
                return;
            }
            
            if (Widgets.ButtonImage(iconRect, icon) && canChange)
            {
                if (hasFreeWill)
                {
                    worldComp.TryRemoveFreeWill(pawn);
                }
                else
                {
                    worldComp.TryGiveFreeWill(pawn);
                }
            }
            
            GUI.color = Color.white;
            
            string tip;
            if (!canChange)
            {
                tip = "FreeWillToggleCannotChange".TranslateSimple();
            }
            else if (hasFreeWill)
            {
                tip = "FreeWillToggleDisable".Translate(pawn.Name.ToStringShort) + "\n" + "FreeWillToggleRightClickFocus".TranslateSimple();
            }
            else
            {
                tip = "FreeWillToggleEnable".Translate(pawn.Name.ToStringShort);
            }
            TooltipHandler.TipRegion(iconRect, tip);
        }
        
        public override int GetMinWidth(PawnTable table)
        {
            return 26;
        }
        
        public override int GetMaxWidth(PawnTable table)
        {
            return 26;
        }
        
        public override int GetOptimalWidth(PawnTable table)
        {
            return 26;
        }
    }
}
