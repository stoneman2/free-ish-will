using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FreeWill
{
    public abstract class Dialog_FocusBase : Window
    {
        private const int TicksPerHour = 2500;
        private const int TicksPerDay = 60000;
        private const string CustomPresetKey = "Custom";

        private static readonly FocusPreset[] presets =
        {
            new FocusPreset("Focus", "FreeWillFocusPresetFocus", "FreeWillFocusPresetFocusTip", 1.5f, 0.75f, 0),
            new FocusPreset("Important", "FreeWillFocusPresetImportant", "FreeWillFocusPresetImportantTip", 2.5f, 0.50f, TicksPerDay),
            new FocusPreset("Urgent", "FreeWillFocusPresetUrgent", "FreeWillFocusPresetUrgentTip", 3.5f, 0.25f, TicksPerHour * 12),
            new FocusPreset("Critical", "FreeWillFocusPresetCritical", "FreeWillFocusPresetCriticalTip", 5.0f, 0.0f, TicksPerHour * 6)
        };

        private static readonly DurationOption[] durationOptions =
        {
            new DurationOption(0, "FreeWillFocusDurationUntilCleared"),
            new DurationOption(TicksPerHour * 3, "FreeWillFocusDuration3Hours"),
            new DurationOption(TicksPerHour * 6, "FreeWillFocusDuration6Hours"),
            new DurationOption(TicksPerHour * 12, "FreeWillFocusDuration12Hours"),
            new DurationOption(TicksPerDay, "FreeWillFocusDuration1Day"),
            new DurationOption(TicksPerDay * 2, "FreeWillFocusDuration2Days"),
            new DurationOption(TicksPerDay * 5, "FreeWillFocusDuration5Days")
        };

        private Vector2 scrollPosition;
        protected WorkTypeDef selectedWorkType;
        protected float intensity = 2.5f;
        protected float defocusMultiplier = 0.5f;
        protected int durationIndex = 4;
        protected string presetKey = "Important";

        protected Dialog_FocusBase()
        {
            doCloseButton = false;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            draggable = true;
        }

        public override Vector2 InitialSize => new Vector2(560f, 680f);

        protected abstract string Title { get; }
        protected abstract string SetMessage(WorkTypeDef workType);
        protected abstract string ClearedMessage { get; }
        protected abstract void SaveFocus(WorkTypeDef workType, float focusIntensity, float otherWorkMultiplier, int durationTicks, string selectedPresetKey);
        protected abstract void ClearFocus();

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 32f), Title);
            Text.Font = GameFont.Small;

            DrawSummary(new Rect(0f, 38f, inRect.width, 46f));

            float panelTop = 98f;
            float panelBottom = inRect.height - 48f;
            Rect workPanel = new Rect(0f, panelTop, 220f, panelBottom - panelTop);
            Rect controlPanel = new Rect(232f, panelTop, inRect.width - 232f, panelBottom - panelTop);
            DrawWorkTypePanel(workPanel);
            DrawControlPanel(controlPanel);
            DrawBottomButtons(inRect);
        }

        protected void LoadFocus(PawnFocusData focus)
        {
            if (focus == null) return;
            selectedWorkType = focus.WorkType;
            intensity = RoundIntensity(focus.Intensity);
            defocusMultiplier = RoundDefocus(focus.DefocusMultiplier);
            durationIndex = GetDurationIndex(focus.DurationTicks);
            presetKey = string.IsNullOrEmpty(focus.PresetKey) ? CustomPresetKey : focus.PresetKey;
            if (!PresetMatchesCurrent(presetKey))
            {
                presetKey = CustomPresetKey;
            }
        }

        private void DrawSummary(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect inner = rect.ContractedBy(10f);
            string workLabel = selectedWorkType == null
                ? "FreeWillFocusNoTarget".TranslateSimple()
                : selectedWorkType.labelShort.CapitalizeFirst().ToString();
            string summary = "FreeWillFocusSummary".Translate(workLabel, GetPresetLabel(presetKey), GetDurationLabel(SelectedDurationTicks));
            Widgets.Label(inner, summary);
        }

        private void DrawWorkTypePanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect inner = rect.ContractedBy(10f);
            Widgets.Label(new Rect(inner.x, inner.y, inner.width, 24f), "FreeWillFocusTargetWork".TranslateSimple());

            Rect scrollRect = new Rect(inner.x, inner.y + 30f, inner.width, inner.height - 30f);
            List<WorkTypeDef> workTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
            Rect viewRect = new Rect(0f, 0f, scrollRect.width - 16f, workTypes.Count * 28f);

            Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect);
            float y = 0f;
            foreach (WorkTypeDef workType in workTypes)
            {
                Rect rowRect = new Rect(0f, y, viewRect.width, 26f);
                if (selectedWorkType == workType)
                {
                    Widgets.DrawHighlightSelected(rowRect);
                }
                else if (Mouse.IsOver(rowRect))
                {
                    Widgets.DrawHighlight(rowRect);
                }

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(new Rect(rowRect.x + 6f, rowRect.y, rowRect.width - 12f, rowRect.height), workType.labelShort.CapitalizeFirst());
                Text.Anchor = TextAnchor.UpperLeft;

                if (Widgets.ButtonInvisible(rowRect))
                {
                    selectedWorkType = workType;
                }
                y += 28f;
            }
            Widgets.EndScrollView();
        }

        private void DrawControlPanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect inner = rect.ContractedBy(12f);
            float y = inner.y;

            Widgets.Label(new Rect(inner.x, y, inner.width, 24f), "FreeWillFocusPreset".TranslateSimple());
            y += 28f;
            DrawPresetRow(new Rect(inner.x, y, inner.width, 34f));
            y += 40f;
            DrawCustomPresetButton(new Rect(inner.x, y, inner.width, 28f));
            y += 42f;

            DrawSliderLabel(new Rect(inner.x, y, inner.width, 24f), "FreeWillFocusBoost".TranslateSimple(), intensity.ToString("F1") + "x");
            y += 24f;
            float nextIntensity = Widgets.HorizontalSlider(new Rect(inner.x, y, inner.width, 24f), intensity, 1.0f, 5.0f, true);
            nextIntensity = RoundIntensity(nextIntensity);
            if (!Mathf.Approximately(nextIntensity, intensity))
            {
                intensity = nextIntensity;
                presetKey = CustomPresetKey;
            }
            y += 44f;

            string otherWorkValue = defocusMultiplier < 0.01f ? "FreeWillFocusOtherWorkDisabled".TranslateSimple() : defocusMultiplier.ToStringPercent();
            DrawSliderLabel(new Rect(inner.x, y, inner.width, 24f), "FreeWillFocusOtherWork".TranslateSimple(), otherWorkValue);
            y += 24f;
            float nextDefocus = Widgets.HorizontalSlider(new Rect(inner.x, y, inner.width, 24f), defocusMultiplier, 0.0f, 1.0f, true);
            nextDefocus = RoundDefocus(nextDefocus);
            if (!Mathf.Approximately(nextDefocus, defocusMultiplier))
            {
                defocusMultiplier = nextDefocus;
                presetKey = CustomPresetKey;
            }
            y += 44f;

            DrawSliderLabel(new Rect(inner.x, y, inner.width, 24f), "FreeWillFocusDuration".TranslateSimple(), GetDurationLabel(SelectedDurationTicks));
            y += 24f;
            int nextDurationIndex = Mathf.RoundToInt(Widgets.HorizontalSlider(new Rect(inner.x, y, inner.width, 24f), durationIndex, 0f, durationOptions.Length - 1, true));
            nextDurationIndex = Mathf.Clamp(nextDurationIndex, 0, durationOptions.Length - 1);
            if (nextDurationIndex != durationIndex)
            {
                durationIndex = nextDurationIndex;
                presetKey = CustomPresetKey;
            }
            y += 44f;

            Rect noteRect = new Rect(inner.x, y, inner.width, inner.yMax - y);
            GUI.color = Color.grey;
            Widgets.Label(noteRect, "FreeWillFocusHelpText".TranslateSimple());
            GUI.color = Color.white;
        }

        private void DrawPresetRow(Rect rect)
        {
            float gap = 6f;
            float buttonWidth = (rect.width - (gap * (presets.Length - 1))) / presets.Length;
            float x = rect.x;
            foreach (FocusPreset preset in presets)
            {
                Rect buttonRect = new Rect(x, rect.y, buttonWidth, rect.height);
                bool selected = presetKey == preset.Key;
                if (selected)
                {
                    Widgets.DrawHighlightSelected(buttonRect);
                }
                if (Widgets.ButtonText(buttonRect, preset.LabelKey.TranslateSimple()))
                {
                    ApplyPreset(preset);
                }
                TooltipHandler.TipRegion(buttonRect, preset.TipKey.TranslateSimple());
                x += buttonWidth + gap;
            }
        }

        private void DrawCustomPresetButton(Rect rect)
        {
            bool selected = presetKey == CustomPresetKey;
            if (selected)
            {
                Widgets.DrawHighlightSelected(rect);
            }
            if (Widgets.ButtonText(rect, "FreeWillFocusPresetCustom".TranslateSimple()))
            {
                presetKey = CustomPresetKey;
            }
            TooltipHandler.TipRegion(rect, "FreeWillFocusPresetCustomTip".TranslateSimple());
        }

        private void DrawSliderLabel(Rect rect, string label, string value)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(new Rect(rect.x, rect.y, rect.width * 0.62f, rect.height), label);
            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(new Rect(rect.x + rect.width * 0.38f, rect.y, rect.width * 0.62f, rect.height), value);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawBottomButtons(Rect inRect)
        {
            float buttonY = inRect.height - 35f;
            if (Widgets.ButtonText(new Rect(0f, buttonY, 150f, 30f), "FreeWillApplyFocus".TranslateSimple()))
            {
                ApplyFocus();
            }
            if (Widgets.ButtonText(new Rect(160f, buttonY, 150f, 30f), "FreeWillClearFocus".TranslateSimple()))
            {
                ClearFocus();
                Messages.Message(ClearedMessage, MessageTypeDefOf.SilentInput);
                Close();
            }
            if (Widgets.ButtonText(new Rect(inRect.width - 100f, buttonY, 100f, 30f), "FreeWillCancel".TranslateSimple()))
            {
                Close();
            }
        }

        private void ApplyFocus()
        {
            if (selectedWorkType == null)
            {
                Messages.Message("FreeWillNoWorkTypeSelected".TranslateSimple(), MessageTypeDefOf.RejectInput);
                return;
            }

            SaveFocus(selectedWorkType, intensity, defocusMultiplier, SelectedDurationTicks, presetKey);
            Messages.Message(SetMessage(selectedWorkType), MessageTypeDefOf.PositiveEvent);
            Close();
        }

        private void ApplyPreset(FocusPreset preset)
        {
            intensity = preset.Intensity;
            defocusMultiplier = preset.DefocusMultiplier;
            durationIndex = GetDurationIndex(preset.DurationTicks);
            presetKey = preset.Key;
        }

        private bool PresetMatchesCurrent(string key)
        {
            foreach (FocusPreset preset in presets)
            {
                if (preset.Key == key && Mathf.Approximately(intensity, preset.Intensity) && Mathf.Approximately(defocusMultiplier, preset.DefocusMultiplier) && SelectedDurationTicks == preset.DurationTicks)
                {
                    return true;
                }
            }
            return key == CustomPresetKey;
        }

        private string GetPresetLabel(string key)
        {
            foreach (FocusPreset preset in presets)
            {
                if (preset.Key == key)
                {
                    return preset.LabelKey.TranslateSimple();
                }
            }
            return "FreeWillFocusPresetCustom".TranslateSimple();
        }

        private int SelectedDurationTicks => durationOptions[durationIndex].Ticks;

        private static int GetDurationIndex(int durationTicks)
        {
            int closestIndex = 0;
            int closestDistance = int.MaxValue;
            for (int i = 0; i < durationOptions.Length; i++)
            {
                int distance = Mathf.Abs(durationOptions[i].Ticks - durationTicks);
                if (distance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }
            return closestIndex;
        }

        private static string GetDurationLabel(int durationTicks)
        {
            foreach (DurationOption option in durationOptions)
            {
                if (option.Ticks == durationTicks)
                {
                    return option.LabelKey.TranslateSimple();
                }
            }
            return "FreeWillFocusDurationUntilCleared".TranslateSimple();
        }

        private static float RoundIntensity(float value)
        {
            return Mathf.Round(Mathf.Clamp(value, 1.0f, 5.0f) * 10f) / 10f;
        }

        private static float RoundDefocus(float value)
        {
            return Mathf.Round(Mathf.Clamp01(value) * 20f) / 20f;
        }

        private class FocusPreset
        {
            public readonly string Key;
            public readonly string LabelKey;
            public readonly string TipKey;
            public readonly float Intensity;
            public readonly float DefocusMultiplier;
            public readonly int DurationTicks;

            public FocusPreset(string key, string labelKey, string tipKey, float intensity, float defocusMultiplier, int durationTicks)
            {
                Key = key;
                LabelKey = labelKey;
                TipKey = tipKey;
                Intensity = intensity;
                DefocusMultiplier = defocusMultiplier;
                DurationTicks = durationTicks;
            }
        }

        private class DurationOption
        {
            public readonly int Ticks;
            public readonly string LabelKey;

            public DurationOption(int ticks, string labelKey)
            {
                Ticks = ticks;
                LabelKey = labelKey;
            }
        }
    }

    public class Dialog_GlobalFocus : Dialog_FocusBase
    {
        private int maxPawns;

        public Dialog_GlobalFocus()
        {
            FreeWill_WorldComponent worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            maxPawns = worldComp?.MaxPawnsForFocusedWork ?? 0;
            LoadFocus(worldComp?.GlobalFocus);
        }

        protected override string Title => "FreeWillGlobalFocusTitle".TranslateSimple();
        protected override string ClearedMessage => "FreeWillGlobalFocusCleared".TranslateSimple();

        protected override string SetMessage(WorkTypeDef workType)
        {
            return "FreeWillGlobalFocusSet".Translate(workType.labelShort).ToString();
        }

        protected override void SaveFocus(WorkTypeDef workType, float focusIntensity, float otherWorkMultiplier, int durationTicks, string selectedPresetKey)
        {
            FreeWill_WorldComponent worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            worldComp?.SetGlobalFocus(workType, focusIntensity, otherWorkMultiplier, maxPawns, durationTicks, selectedPresetKey);
        }

        protected override void ClearFocus()
        {
            FreeWill_WorldComponent worldComp = Find.World?.GetComponent<FreeWill_WorldComponent>();
            worldComp?.ClearGlobalFocus();
        }
    }
}
