using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FreeWill
{
    /// <summary>
    /// Holds user configurable settings for the FreeWill mod and draws the mod settings UI.
    /// </summary>
    public class FreeWill_ModSettings : ModSettings
    {
        // mod default settings
        private const bool ConsiderIdeologyDefault = true;
        private const bool ConsiderBrawlersNotHuntingDefault = true;
        private const bool ConsiderHasHuntingWeaponDefault = true;
        private const float ConsiderMovementSpeedDefault = 1.0f;
        private const float ConsiderPassionsDefault = 1.0f;
        private const float ConsiderBeautyDefault = 1.0f;
        private const float ConsiderBestAtDoingDefault = 1.0f;
        private const float ConsiderFoodPoisoningDefault = 1.0f;
        private const float ConsiderLowFoodDefault = 1.0f;
        private const float ConsiderWeaponRangeDefault = 1.0f;
        private const float ConsiderOwnRoomDefault = 1.0f;
        private const float ConsiderPlantsBlightedDefault = 1.0f;
        private const float ConsiderGauranlenPruningDefault = 1.0f;
        private const int TickIntervalDefault = 4;
        
        public bool ConsiderIdeology = ConsiderIdeologyDefault;
        public bool ConsiderBrawlersNotHunting = ConsiderBrawlersNotHuntingDefault;
        public bool ConsiderHasHuntingWeapon = ConsiderHasHuntingWeaponDefault;
        public float ConsiderMovementSpeed = ConsiderMovementSpeedDefault;
        public float ConsiderPassions = ConsiderPassionsDefault;
        public float ConsiderBeauty = ConsiderBeautyDefault;
        public float ConsiderBestAtDoing = ConsiderBestAtDoingDefault;
        public float ConsiderFoodPoisoning = ConsiderFoodPoisoningDefault;
        public float ConsiderLowFood = ConsiderLowFoodDefault;
        public float ConsiderWeaponRange = ConsiderWeaponRangeDefault;
        public float ConsiderOwnRoom = ConsiderOwnRoomDefault;
        public float ConsiderPlantsBlighted = ConsiderPlantsBlightedDefault;
        public float ConsiderGauranlenPruning = ConsiderGauranlenPruningDefault;
        public int TickInterval = TickIntervalDefault;

        public Dictionary<string, float> globalWorkAdjustments;
        public Dictionary<string, int> globalWorkCaps;

        private const int TAB_GENERAL = 0;
        private const int TAB_WORK_PRIORITIES = 1;
        private static int currentTab = TAB_GENERAL;


        private Vector2 pos;
        private float height;

        /// <summary>
        /// Initializes the settings with default values.
        /// </summary>
        public FreeWill_ModSettings()
        {
            globalWorkAdjustments = new Dictionary<string, float>();
            pos = Vector2.zero;
            height = 500.0f;
        }

        /// <summary>
        /// Draws the settings UI for the mod.
        /// </summary>
        /// <param name="inRect">Area to draw within.</param>
        public void DoSettingsWindowContents(Rect inRect)
        {
            if (globalWorkAdjustments == null) globalWorkAdjustments = new Dictionary<string, float>();
            if (globalWorkCaps == null) globalWorkCaps = new Dictionary<string, int>();

            float tabPadding = 15f;
            Rect tabRect = new Rect(inRect.x, inRect.y + tabPadding, inRect.width, 35f);
            List<TabRecord> tabs = new List<TabRecord>();
            tabs.Add(new TabRecord("FreeWillSettingsGeneral".TranslateSimple(), () => { currentTab = TAB_GENERAL; pos = Vector2.zero; }, currentTab == TAB_GENERAL));
            tabs.Add(new TabRecord("FreeWillSettingsPriorities".TranslateSimple(), () => { currentTab = TAB_WORK_PRIORITIES; pos = Vector2.zero; }, currentTab == TAB_WORK_PRIORITIES));
            TabDrawer.DrawTabs(tabRect, tabs);

            Rect contentRect = new Rect(inRect.x, inRect.y + tabPadding + 45f, inRect.width, inRect.height - tabPadding - 45f);
            
            Rect viewRect = new Rect(0f, 0f, contentRect.width - 20f, Mathf.Max(height, 1000f));
            
            Widgets.BeginScrollView(contentRect, ref pos, viewRect);
            
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(new Rect(0f, 0f, viewRect.width - 16f, 99999f));

            if (currentTab == TAB_GENERAL)
            {
                DrawGeneralSettings(ls);
            }
            else if (currentTab == TAB_WORK_PRIORITIES)
            {
                DrawPrioritySettings(ls);
            }

            height = ls.CurHeight + 50f;

            ls.End();
            Widgets.EndScrollView();
        }

        private void DrawGeneralSettings(Listing_Standard ls)
        {
            ls.Gap(10.0f);
            ls.Label("FreeWillPerformanceSettings".TranslateSimple());
            ls.GapLine(10.0f);
            ls.CheckboxLabeled("FreeWillConsiderIdeology".TranslateSimple(), ref ConsiderIdeology, "FreeWillConsiderIdeologyLong".TranslateSimple());
            ls.Gap(10.0f);

            string tickLabel = "FreeWillTickInterval".TranslateSimple();
            string tickValue = string.Format("{0}", TickInterval);
            string tickTip = "FreeWillTickIntervalLong".TranslateSimple();
            ls.LabelDouble(tickLabel, tickValue, tip: tickTip);
            TickInterval = Mathf.RoundToInt(ls.Slider(TickInterval, 1, 60));
            if (ls.ButtonText("FreeWillDefaultSliderButtonLabel".TranslateSimple()))
            {
                TickInterval = TickIntervalDefault;
            }
            ls.GapLine(30.0f);

            DrawFactorSlider(ls, ref ConsiderMovementSpeed, "FreeWillConsiderMovementSpeed", ConsiderMovementSpeedDefault);
            DrawFactorSlider(ls, ref ConsiderPassions, "FreeWillConsiderPassions", ConsiderPassionsDefault);
            DrawFactorSlider(ls, ref ConsiderBeauty, "FreeWillConsiderBeauty", ConsiderBeautyDefault);
            DrawFactorSlider(ls, ref ConsiderBestAtDoing, "FreeWillConsiderBestAtDoing", ConsiderBestAtDoingDefault);
            DrawFactorSlider(ls, ref ConsiderLowFood, "FreeWillConsiderLowFood", ConsiderLowFoodDefault);
            DrawFactorSlider(ls, ref ConsiderWeaponRange, "FreeWillConsiderWeaponRange", ConsiderWeaponRangeDefault);
            DrawFactorSlider(ls, ref ConsiderFoodPoisoning, "FreeWillConsiderFoodPoisoning", ConsiderFoodPoisoningDefault);
            DrawFactorSlider(ls, ref ConsiderOwnRoom, "FreeWillConsiderOwnRoom", ConsiderOwnRoomDefault);
            DrawFactorSlider(ls, ref ConsiderPlantsBlighted, "FreeWillConsiderPlantsBlighted", ConsiderPlantsBlightedDefault);
            DrawFactorSlider(ls, ref ConsiderGauranlenPruning, "FreeWillConsiderGauranlenPruning", ConsiderGauranlenPruningDefault);
            
            ls.CheckboxLabeled("FreeWillConsiderHasHuntingWeapon".TranslateSimple(), ref ConsiderHasHuntingWeapon, "FreeWillConsiderHasHuntingWeaponLong".TranslateSimple());
            ls.Gap(10.0f);
            ls.CheckboxLabeled("FreeWillConsiderBrawlersNotHunting".TranslateSimple(), ref ConsiderBrawlersNotHunting, "FreeWillConsiderBrawlersNotHuntingLong".TranslateSimple());
        }

        private void DrawPrioritySettings(Listing_Standard ls)
        {
            List<WorkTypeDef> workTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
            
            ls.Label("FreeWillSettingsPrioritiesDescription".TranslateSimple());
            ls.GapLine();

            // Reset Button
            if (ls.ButtonTextLabeled("FreeWillResetGlobalSlidersLabel".TranslateSimple(), "FreeWillResetGlobalSlidersButtonLabel".TranslateSimple()))
            {
                foreach (WorkTypeDef workTypeDef in workTypes)
                {
                    globalWorkAdjustments.SetOrAdd(workTypeDef.defName, 0f);
                    globalWorkCaps.SetOrAdd(workTypeDef.defName, 0);
                }
            }
            ls.Gap(20f);

            foreach (WorkTypeDef workTypeDef in workTypes)
            {
                // Adjustment Slider
                _ = globalWorkAdjustments.TryGetValue(workTypeDef.defName, out float adj);
                string s1 = string.Format("{0} {1}", "FreeWillWorkTypeAdjustment".TranslateSimple(), workTypeDef.labelShort);
                string s2 = string.Format("{0}%", Mathf.RoundToInt(adj * 100.0f));
                ls.LabelDouble(s1, s2, tip: workTypeDef.description);
                globalWorkAdjustments.SetOrAdd(
                    workTypeDef.defName,
                    Mathf.RoundToInt(ls.Slider(adj, -1.0f, 1.0f) * 100.0f) / 100.0f
                );

                // Priority Cap - use simple clickable number boxes
                _ = globalWorkCaps.TryGetValue(workTypeDef.defName, out int cap);

                Rect capRow = ls.GetRect(24f);
                float labelWidth = 90f;
                float boxSize = 28f;
                float spacing = 4f;
                
                // Label
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(new Rect(capRow.x, capRow.y, labelWidth, capRow.height), "FreeWillPriorityCap".TranslateSimple() + ":");
                
                float boxX = capRow.x + labelWidth + 10f;
                
                // Helper to draw a selectable box
                void DrawCapBox(int value, string label)
                {
                    Rect box = new Rect(boxX, capRow.y, boxSize, capRow.height);
                    bool isSelected = (cap == value);
                    
                    // Draw background
                    if (isSelected)
                        Widgets.DrawBoxSolid(box, new Color(0.2f, 0.6f, 0.2f, 0.8f)); // Green for selected
                    else
                        Widgets.DrawBoxSolid(box, new Color(0.15f, 0.15f, 0.15f, 0.8f)); // Dark gray
                    
                    // Draw border
                    Widgets.DrawBox(box);
                    
                    // Draw label centered
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(box, label);
                    
                    // Handle click
                    if (Widgets.ButtonInvisible(box))
                        cap = value;
                    
                    boxX += boxSize + spacing;
                }
                
                DrawCapBox(0, "-");  // No cap (dash means "no limit")
                DrawCapBox(1, "1");
                DrawCapBox(2, "2");
                DrawCapBox(3, "3");
                DrawCapBox(4, "4");
                
                Text.Anchor = TextAnchor.UpperLeft; // Reset

                globalWorkCaps.SetOrAdd(workTypeDef.defName, cap);
                ls.Gap(8f);
            }
        }

        private void DrawFactorSlider(Listing_Standard ls, ref float setting, string key, float defaultValue)
        {
            string s1 = key.TranslateSimple();
            string s2 = string.Format("{0}x", setting);
            string s3 = (key + "Long").TranslateSimple();
            ls.LabelDouble(s1, s2, tip: s3);
            setting = Mathf.RoundToInt(ls.Slider(setting, 0.0f, 10.0f) * 10.0f) / 10.0f;
            if (ls.ButtonText("FreeWillDefaultSliderButtonLabel".TranslateSimple()))
            {
                setting = defaultValue;
            }
            ls.Gap(10f);
        }

        /// <summary>
        /// Saves and loads the mod settings.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref ConsiderIdeology, "freeWillConsiderIdeology", ConsiderIdeologyDefault, true);
            Scribe_Values.Look(ref ConsiderMovementSpeed, "freeWillConsiderMovementSpeed", ConsiderMovementSpeedDefault, true);
            Scribe_Values.Look(ref ConsiderPassions, "freeWillConsiderPassions", ConsiderPassionsDefault, true);
            Scribe_Values.Look(ref ConsiderBeauty, "freeWillConsiderBeauty", ConsiderBeautyDefault, true);
            Scribe_Values.Look(ref ConsiderBestAtDoing, "freeWillConsiderBestAtDoing", ConsiderBestAtDoingDefault, true);
            Scribe_Values.Look(ref ConsiderFoodPoisoning, "freeWillConsiderFoodPoisoning", ConsiderFoodPoisoningDefault, true);
            Scribe_Values.Look(ref ConsiderLowFood, "freeWillConsiderLowFood", ConsiderLowFoodDefault, true);
            Scribe_Values.Look(ref ConsiderWeaponRange, "freeWillConsiderWeaponRange", ConsiderWeaponRangeDefault, true);
            Scribe_Values.Look(ref ConsiderOwnRoom, "freeWillConsiderOwnRoom", ConsiderOwnRoomDefault, true);
            Scribe_Values.Look(ref ConsiderBrawlersNotHunting, "freeWillBrawlersNotHunting", ConsiderBrawlersNotHuntingDefault, true);
            Scribe_Values.Look(ref ConsiderHasHuntingWeapon, "freeWillHuntingWeapon", ConsiderHasHuntingWeaponDefault, true);
            Scribe_Values.Look(ref ConsiderPlantsBlighted, "freeWillPlantsBlighted", ConsiderPlantsBlightedDefault, true);
            Scribe_Values.Look(ref ConsiderGauranlenPruning, "freeWillGauranlenPruning", ConsiderGauranlenPruningDefault, true);
            Scribe_Values.Look(ref TickInterval, "freeWillTickInterval", TickIntervalDefault, true);
            if (globalWorkAdjustments == null)
            {
                globalWorkAdjustments = new Dictionary<string, float>();
            }
            Scribe_Collections.Look(ref globalWorkAdjustments, "freeWillWorkTypeAdjustments", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref globalWorkCaps, "freeWillWorkTypeCaps", LookMode.Value, LookMode.Value);
            base.ExposeData();
        }
    }
}
