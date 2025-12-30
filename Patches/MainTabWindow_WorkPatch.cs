using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace FreeWill.Patches
{
    /// <summary>
    /// Adds new buttons to the Work tab header
    /// </summary>
    [HarmonyPatch(typeof(MainTabWindow_Work), nameof(MainTabWindow_Work.DoWindowContents))]
    public static class MainTabWindow_WorkPatch
    {
        private const float ButtonSize = 24f;
        private const float ButtonGap = 4f;
    
        private static void Postfix(Rect rect)
        {
            float xPos = rect.xMax - ButtonSize - ButtonGap;
            float yPos = rect.yMin + ButtonGap;
            
            // Settings button
            Rect settingsRect = new Rect(xPos, yPos, ButtonSize, ButtonSize);
            TooltipHandler.TipRegion(settingsRect, "FreeWillOpenSettings".TranslateSimple());
            if (Widgets.ButtonImage(settingsRect, FreeWillResources.SettingsIcon))
            {
                OpenFreeWillSettings();
            }
            
            xPos -= ButtonSize + ButtonGap;
            
            // Refresh button
            Rect refreshRect = new Rect(xPos, yPos, ButtonSize, ButtonSize);
            TooltipHandler.TipRegion(refreshRect, "FreeWillRefreshPriorities".TranslateSimple());
            if (Widgets.ButtonImage(refreshRect, FreeWillResources.RefreshIcon))
            {
                ForceRefreshPriorities();
            }
            
            xPos -= ButtonSize + ButtonGap;
            
            // Global focus
            var worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            bool hasGlobalFocus = worldComp?.GlobalFocus != null;
            Rect focusRect = new Rect(xPos, yPos, ButtonSize, ButtonSize);
            
            if (hasGlobalFocus)
            {
                GUI.color = new Color(1f, 0.8f, 0.2f);
            }
            TooltipHandler.TipRegion(focusRect, hasGlobalFocus 
                ? "FreeWillClearGlobalFocus".TranslateSimple() 
                : "FreeWillSetGlobalFocus".TranslateSimple());
            if (Widgets.ButtonImage(focusRect, FreeWillResources.FocusIcon))
            {
                if (hasGlobalFocus)
                {
                    worldComp.ClearGlobalFocus();
                }
                else
                {
                    Find.WindowStack.Add(new Dialog_GlobalFocus());
                }
            }
            GUI.color = Color.white;
        }
        
        private static void OpenFreeWillSettings()
        {
            var mod = LoadedModManager.GetMod<FreeWill_Mod>();
            if (mod != null)
            {
                Find.WindowStack.Add(new Dialog_ModSettings(mod));
            }
        }
        
        private static void ForceRefreshPriorities()
        {
            foreach (var map in Find.Maps)
            {
                var mapComp = map.GetComponent<FreeWill_MapComponent>();
                mapComp?.ForceRefresh();
            }
            Messages.Message("FreeWillPrioritiesRefreshed".TranslateSimple(), MessageTypeDefOf.SilentInput);
        }
    }
}
