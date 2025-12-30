using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace FreeWill
{
    public static class WorkTypeHeaderPatch
    {
        [HarmonyPatch(typeof(PawnColumnWorker_WorkPriority), nameof(PawnColumnWorker_WorkPriority.DoHeader))]
        public static class DoHeader_Patch
        {
            public static void Postfix(PawnColumnWorker_WorkPriority __instance, Rect rect)
            {
                if (__instance?.def?.workType == null) return;
                
                var worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
                if (worldComp == null) return;
                
                WorkTypeDef workType = __instance.def.workType;
                bool isDisabled = worldComp.IsWorkTypeDisabled(workType);
                
                const float iconSize = 16f;
                Rect buttonRect = new Rect(
                    rect.center.x - iconSize / 2f,
                    rect.yMax - iconSize - 4f,
                    iconSize,
                    iconSize
                );
                
                Texture2D icon = isDisabled ? FreeWillResources.WorkTypeOff : FreeWillResources.WorkTypeOn;
                string tooltip = isDisabled 
                    ? "FreeWillWorkTypeDisabled".TranslateSimple() 
                    : "FreeWillWorkTypeEnabled".TranslateSimple();
                
                TooltipHandler.TipRegion(buttonRect, tooltip);
                if (Widgets.ButtonImage(buttonRect, icon))
                {
                    worldComp.ToggleWorkTypeEnabled(workType);
                }
            }
        }

        [HarmonyPatch(typeof(PawnColumnWorker_WorkPriority), nameof(PawnColumnWorker_WorkPriority.GetMinHeaderHeight))]
        public static class GetMinHeaderHeight_Patch
        {
            public static void Postfix(ref int __result)
            {
                __result += 24;
            }
        }
    }
}

