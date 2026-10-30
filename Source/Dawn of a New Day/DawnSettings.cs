using DawnNewDay.Compatibility;
using DawnNewDay.Dialogs;
using DawnNewDay.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnSettings : ModSettings
    {
        #region Header

        public bool Enabled = DawnDefault.Enabled;
        public bool ScreenshotMode = DawnDefault.ScreenshotMode;

        private bool m_ShowExample = false;
        public bool ConsumeShowExample
        {
            get
            {
                if (m_ShowExample)
                {
                    m_ShowExample = false;
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Appearance Section

        private bool m_AppearanceSection = false;

        public bool ShowHighlight = DawnDefault.ShowHighlight;

        public float Scale = DawnDefault.Scale;
        public Vector2 Offset = DawnDefault.Offset;

        #endregion

        #region Duration Section

        private bool m_DurationSection = false;

        public float DisplayDurationSeconds = DawnDefault.DisplayDurationSeconds;
        public float FadeInDurationSeconds = DawnDefault.FadeInDurationSeconds;
        public float FadeOutDurationSeconds = DawnDefault.FadeOutDurationSeconds;

        #endregion

        #region Overlay Widget Section

        private bool m_TextSection = false;

        private int m_TextReorderableGroupID = -1;
        public List<IDawnWidget> OverlayWidgets = DawnDefault.OverlayWidgets;

        private readonly List<string> m_RemovedFormatTexts = [];

        private FormatContext m_FormatContext;

        #region Legacy

        public string Legacy_UpperTextFormat = null;
        public string Legacy_BottomTextFormat = null;
        public string Legacy_SubtitleTextFormat = null;

        #endregion

        #endregion

        #region Sound Section

        private bool m_SoundSection = false;

        public bool SoundEnabled = DawnDefault.SoundEnabled;

        private string m_SoundDefName = DawnData.DefaultSoundDefName;

        public SoundDef Sound => DefDatabase<SoundDef>.GetNamedSilentFail(m_SoundDefName) ?? DefDatabase<SoundDef>.GetNamedSilentFail(DawnData.DefaultSoundDefName);

        public float SoundVolume = DawnDefault.SoundVolume;
        public float SoundPitch = DawnDefault.SoundPitch;

        #endregion

        #region Extra Section

        private bool m_ExtraSection = false;

        public bool StartsAtZero = DawnDefault.StartsAtZero;
        public int ShowEveryXDays = DawnDefault.ShowEveryXDays;
        public int TriggerHour = DawnDefault.TriggerHour;

        #endregion

        #region Compatibility Section

        private bool m_CompatibilitySection = false;

        #region Modern Notifications Section

        private bool m_MN_CompatibilitySection = false;

        private bool m_MN_ExcludeOccasionSection = false;

        public Dictionary<string, bool> MN_ExcludeOccasionCategory = DawnDefault.MN_ExcludeOccasionCategory;

        private bool m_MN_ReminderTextSection = false;
        public DawnTextConfig MN_Reminder = DawnDefault.MN_Reminder;

        private bool m_MN_OccasionTextSection = false;
        public DawnTextConfig MN_Occasion = DawnDefault.MN_Occasion;

        #endregion

        #endregion

        #region Buffers

        private string m_DisplayDurationBuffer;
        private string m_FadeInDurationBuffer;
        private string m_FadeOutDurationBuffer;

        private string m_ShowEveryXDaysBuffer;

        #endregion

        private Vector2 m_ScrollPosition = Vector2.zero;
        private float m_CachedScrollViewHeight = 0f;

        public void OnceOpen()
        {
            UpdateText();
        }

        public void EveryOpen()
        {
            m_FormatContext = new DawnFormatter(Current.Game).CreateFormatContext(StartsAtZero);
        }

        public void DoWindowContents(Rect canva)
        {
            canva = canva.MiddlePart(1f, 0.95f);

            Listing_Standard listing = new() { maxOneColumn = true };
            try
            {
                listing.Begin(canva);

                ShowHeader(listing);

                listing.GapLine();

                listing.End();

                float headerHeight = listing.CurHeight;

                Rect outRect = new(canva.x, canva.y + headerHeight, canva.width, canva.height - headerHeight);
                Rect viewRect = new(0f, 0f, outRect.width - 32f, Mathf.Max(outRect.height, m_CachedScrollViewHeight));
                Widgets.BeginScrollView(outRect, ref m_ScrollPosition, viewRect);

                listing.Begin(viewRect);

                if (listing.SectionButton(DawnTranslation.Section_Appearance, ref m_AppearanceSection, false))
                    listing.Indented(() => ShowAppearanceSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Duration, ref m_DurationSection))
                    listing.Indented(() => ShowDurationSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_OverlayWidget, ref m_TextSection))
                    listing.Indented(() => ShowOverlayWidgetSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Sound, ref m_SoundSection))
                    listing.Indented(() => ShowSoundSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Extra, ref m_ExtraSection))
                    listing.Indented(() => ShowExtraSection(listing));

                if (listing.SectionButton(DawnTranslation.Section_Compatibility, ref m_CompatibilitySection))
                    listing.Indented(() => ShowCompatibilitySection(listing));

                listing.GapLine();
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
            finally
            {
                m_CachedScrollViewHeight = listing.CurHeight + (canva.height * 0.2f);

                listing.End();

                Widgets.EndScrollView();
            }
        }

        private void ShowHeader(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_Enabled, ref Enabled);
            listing.CheckboxLabeled(DawnTranslation.Label_ScreenshotMode, ref ScreenshotMode);

            listing.Gap();

            if (listing.ButtonText(DawnTranslation.Label_ShowExample))
            {
                Messages.Message(DawnTranslation.Message_ShowExample, MessageTypeDefOf.PositiveEvent, false);
                m_ShowExample = true;
            }
        }

        private void ShowAppearanceSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_ShowHighlight, ref ShowHighlight);

            listing.Gap();

            if (listing.ButtonText($"{DawnTranslation.Label_Scale}: {Scale}x"))
            {
                var scaleModes = SettingsHelper.CreateFloatMenu(DawnData.ScalePresets, scale => new($"{scale}x", () => Scale = scale));
                Find.WindowStack.Add(scaleModes);
            }

            if (listing.ButtonText(DawnTranslation.Label_OffsetPresets))
            {
                var offsetPresets = SettingsHelper.CreateFloatMenu(DawnTranslation.OffsetPresets, item => new(item.Name, () =>
                    Offset = new Vector2(UI.screenWidth, UI.screenHeight) * item.Preset));
                Find.WindowStack.Add(offsetPresets);
            }

            Offset.x = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Offset} X ({Offset.x} px)", Offset.x, 0f, UI.screenWidth, 0.25f));
            Offset.y = Mathf.Ceil(listing.SliderLabeled($"{DawnTranslation.Label_Offset} Y ({Offset.y} px)", Offset.y, 0f, UI.screenHeight, 0.25f));
        }

        private void ShowDurationSection(Listing_Standard listing)
        {
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_DisplayDuration} (seconds)", ref DisplayDurationSeconds, ref m_DisplayDurationBuffer, 0f, 120f);
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_FadeInDuration} (seconds)", ref FadeInDurationSeconds, ref m_FadeInDurationBuffer, 0f, DisplayDurationSeconds);
            listing.LabeledTextFieldNumeric($"{DawnTranslation.Label_FadeOutDuration} (seconds)", ref FadeOutDurationSeconds, ref m_FadeOutDurationBuffer, 0f, DisplayDurationSeconds);
        }

        private void ShowOverlayWidgetSection(Listing_Standard listing)
        {
            const float cRowHeight = 30f;

            Rect listRect = listing.GetRect(cRowHeight * OverlayWidgets.Count);

            if (Event.current.type == EventType.Repaint)
            {
                m_TextReorderableGroupID = ReorderableWidget.NewGroup((fromIndex, toIndex) =>
                    {
                        var item = OverlayWidgets[fromIndex];
                        OverlayWidgets.Insert(toIndex, item);
                        OverlayWidgets.RemoveAt((fromIndex < toIndex) ? fromIndex : (fromIndex + 1));
                    }, ReorderableDirection.Vertical, listRect);
            }

            for (int i = 0; i < OverlayWidgets.Count; i++)
                ShowOverlayWidgetEntry(listRect, OverlayWidgets[i], i, cRowHeight);

            listing.Gap();

            Rect rowRect = listing.GetRect(cRowHeight);

            Rect addRect = new(rowRect.position, new(cRowHeight, cRowHeight));
            if (Widgets.ButtonImage(addRect.ExpandedBy(4f), TexButton.Add))
            {
                var addMenu = new FloatMenu([
                    new(DawnTranslation.Label_Text, () => OverlayWidgets.Add(new DawnText())),
                    new(DawnTranslation.Label_Separator, () => OverlayWidgets.Add(new DawnSeparator()))
                    ]);
                Find.WindowStack.Add(addMenu);
            }

            Rect resetRect = new(rowRect.xMax - cRowHeight, rowRect.y, cRowHeight, cRowHeight);
            if (Widgets.ButtonImage(resetRect.ContractedBy(4f), TexButton.HotReloadDefs))
            {
                Find.WindowStack.Add(new Dialog_Confirm(DawnTranslation.Message_ResetWarning, delegate
                    {
                        static IEnumerable<string> SelectTextFormat(List<IDawnWidget> list) =>
                            list.Where(widget => widget is DawnText).Select(widget => ((DawnText)widget).Format)
                            .Where(format => !format.NullOrEmpty());

                        m_RemovedFormatTexts.AddRange(SelectTextFormat(OverlayWidgets)
                            .Where(text => !SelectTextFormat(DawnDefault.OverlayWidgets).Contains(text)));

                        OverlayWidgets = DawnDefault.OverlayWidgets;
                    }));
            }

            Rect backupRect = new(resetRect.x - cRowHeight, rowRect.y, cRowHeight, cRowHeight);
            if (Widgets.ButtonImage(backupRect.ContractedBy(4f), TexButton.Save))
            {
                Dictionary<string, string> backupFormats = [];

                void TryAddBackup(string format, string label, bool legacy)
                {
                    string legacyStr = legacy ? $"({DawnTranslation.Label_Legacy}) " : string.Empty;
                    if (!format.NullOrEmpty())
                        backupFormats.TryAdd(format, $"{legacyStr} {label}");
                }
                TryAddBackup(Legacy_UpperTextFormat, DawnTranslation.Label_UpperTextFormat, true);
                TryAddBackup(Legacy_BottomTextFormat, DawnTranslation.Label_BottomTextFormat, true);
                TryAddBackup(Legacy_SubtitleTextFormat, DawnTranslation.Label_SubtitleTextFormat, true);

                for (int i = 0; i < m_RemovedFormatTexts.Count; i++)
                    TryAddBackup(m_RemovedFormatTexts[i], DawnTranslation.Description_BackupFormat($"{i + 1:D2}"), false);

                Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_BackupFormats, backupFormats));
            }
        }

        private void ShowOverlayWidgetEntry(Rect listRect, IDawnWidget widget, int index, float rowHeight)
        {
            if (widget == null)
            {
                OverlayWidgets.Remove(widget);
                return;
            }

            Rect rowRect = new(listRect.x, listRect.y + (index * rowHeight), listRect.width, rowHeight);

            Rect dragRect = new(rowRect.position, new(rowHeight, rowHeight));
            if (Event.current.type != EventType.MouseDown || Mouse.IsOver(dragRect))
                ReorderableWidget.Reorderable(m_TextReorderableGroupID, rowRect);
            Widgets.ButtonImage(dragRect, TexButton.DragHash);

            Rect deleteRect = new(rowRect.xMax - rowHeight, rowRect.y, rowHeight, rowHeight);
            void ShowDeleteButton(Action onPressed)
            {
                if (Widgets.ButtonImage(deleteRect, TexButton.Delete))
                    onPressed();
            }

            Rect editRect = new(deleteRect.x - rowHeight - 10f, rowRect.y, rowHeight, rowHeight);
            void ShowEditButton(Action onPressed)
            {
                if (Widgets.ButtonImage(editRect, TexButton.Rename))
                    onPressed();
            }

            Rect previewRect = new(dragRect.xMax, rowRect.y, editRect.x - dragRect.xMax, rowHeight);

            switch (widget)
            {
                case DawnText text:
                    float textScale = text.FontSize <= rowHeight ? 1f : rowHeight / text.FontSize;
                    text.ApplyToGUIStyle(textScale);

                    string label = m_FormatContext.FormatText(text.Format, index, textScale);
                    text.Draw(previewRect, label, textScale);

                    text.ApplyToGUIStyle(Scale);

                    ShowEditButton(() => Find.WindowStack.Add(new Dialog_EditText(text, index, Scale, m_FormatContext)));
                    ShowDeleteButton(delegate
                    {
                        string message = $"{DawnTranslation.Message_DeleteWarning}: {DawnTranslation.Label_Text}\n\n(N° {index + 1}) ~ {label.StripTags()}";
                        Find.WindowStack.Add(new Dialog_Confirm(message, delegate
                            {
                                if (!text.Format.NullOrEmpty() && !DawnDefault.OverlayWidgets.Contains(text))
                                    m_RemovedFormatTexts.Add(text.Format);

                                OverlayWidgets.Remove(text);
                            }));
                    });
                    break;
                case DawnSeparator separator:
                    Rect drawRect = separator.CalcDrawRect(separator.CalcFullRect(previewRect.y, previewRect.width, 0f), 0f);
                    separator.Draw(new(previewRect.x + drawRect.x, drawRect.y + previewRect.height / 3f, drawRect.width, drawRect.height));

                    ShowEditButton(() => Find.WindowStack.Add(new Dialog_EditSeparator(separator)));
                    ShowDeleteButton(delegate
                    {
                        string message = $"{DawnTranslation.Message_DeleteWarning}: {DawnTranslation.Label_Separator}\n\n(N° {index + 1})";
                        Find.WindowStack.Add(new Dialog_Confirm(message, () => OverlayWidgets.Remove(separator)));
                    });
                    break;

                default:
                    break;
            }
        }

        private void ShowSoundSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_Enabled, ref SoundEnabled);

            listing.Gap();

            if (listing.ButtonText($"{DawnTranslation.Label_Sound} ({m_SoundDefName})"))
                Find.WindowStack.Add(new Dialog_ChooseSound(soundDefName => m_SoundDefName = soundDefName, m_SoundDefName));

            SoundVolume = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnTranslation.Label_SoundVolume} ({SoundVolume.ToStringPercent()})", SoundVolume, 0.01f, 2f), 0.01f);
            SoundPitch = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnTranslation.Label_SoundPitch} ({SoundPitch.ToStringPercent()})", SoundPitch, 0.01f, 2f), 0.01f);
        }

        private void ShowExtraSection(Listing_Standard listing)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_StartsAtZero, ref StartsAtZero);

            listing.Gap();

            listing.LabeledTextFieldNumeric(DawnTranslation.Label_ShowEveryXDays, ref ShowEveryXDays, ref m_ShowEveryXDaysBuffer, 1f, 1200f);
            TriggerHour = Mathf.CeilToInt(listing.SliderLabeled($"{DawnTranslation.Label_TriggerHour} ({TriggerHour:00}h)", TriggerHour, 0f, 23f, 0.35f));

            listing.Gap();
        }

        private void ShowCompatibilitySection(Listing_Standard listing)
        {
            if (listing.SectionButton(DawnTranslation.MN_ModName, ref m_MN_CompatibilitySection, false))
                listing.Indented(() => ShowMNCompatibilitySection(listing));
        }

        private void ShowMNCompatibilitySection(Listing_Standard listing)
        {
            if (!ModernNotifications.Present)
            {
                SettingsHelper.WithGUIColor(Color.grey, () => listing.Label(DawnTranslation.Label_ModIncompatibility));
                return;
            }

            if (listing.SectionButton(DawnTranslation.Label_MN_ExcludeOccasions, ref m_MN_ExcludeOccasionSection, false))
                listing.Indented(() => listing.LabeledCheckboxTable(ModernNotificationUtility.OccasionCategories, ref MN_ExcludeOccasionCategory));

            listing.Gap();

            if (listing.SectionButton(DawnTranslation.Section_MN_Reminder, ref m_MN_ReminderTextSection))
                listing.Indented(() => MN_Reminder.DoContents(listing, Scale));

            if (listing.SectionButton(DawnTranslation.Section_MN_Occasion, ref m_MN_OccasionTextSection))
                listing.Indented(() => MN_Occasion.DoContents(listing, Scale));

            listing.Gap();

            Rect rowRect = listing.GetRect(30f);

            if (Widgets.ButtonText(rowRect.LeftHalf(), DawnTranslation.Label_MN_LabelFormatPresets))
                Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_MN_LabelFormatPresets, DawnTranslation.Hints_MN_LabelFormatPresets));

            if (Widgets.ButtonText(rowRect.RightHalf(), DawnTranslation.Label_MN_LabelFormatHints))
                Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_LabelFormatHints, DawnTranslation.Hints_MN_LabelFormat));

        }

        public void UpdateText()
        {
            foreach (DawnText text in OverlayWidgets.Where(widget => widget is DawnText).Cast<DawnText>())
            {
                text.UpdateFontFamily();
                text.ApplyToGUIStyle(Scale);
            }

            if (ModernNotifications.Present)
            {
                MN_Reminder.TextStyle.UpdateFontFamily();
                MN_Reminder.TextStyle.ApplyToGUIStyle(Scale);

                MN_Occasion.TextStyle.UpdateFontFamily();
                MN_Occasion.TextStyle.ApplyToGUIStyle(Scale);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            try
            {
                #region Header

                Scribe_Values.Look(ref Enabled, "Enabled", DawnDefault.Enabled);
                Scribe_Values.Look(ref ScreenshotMode, "ScreenshotMode", DawnDefault.ScreenshotMode);

                #endregion

                #region Appearance Section

                Scribe_Values.Look(ref ShowHighlight, "ShowHighlight", DawnDefault.ShowHighlight);

                Scribe_Values.Look(ref Scale, "TextScale", DawnDefault.Scale);
                Scribe_Values.Look(ref Offset, "Offset", DawnDefault.Offset);

                #endregion

                #region Duration Section

                Scribe_Values.Look(ref DisplayDurationSeconds, "DisplayDurationSeconds", DawnDefault.DisplayDurationSeconds);
                Scribe_Values.Look(ref FadeInDurationSeconds, "FadeInDurationSeconds", DawnDefault.FadeInDurationSeconds);
                Scribe_Values.Look(ref FadeOutDurationSeconds, "FadeOutDurationSeconds", DawnDefault.FadeOutDurationSeconds);

                #endregion

                #region Overlay Widget Section

                Scribe_Values.Look(ref Legacy_UpperTextFormat, "UpperTextFormat1", null);
                Scribe_Values.Look(ref Legacy_BottomTextFormat, "BottomTextFormat1", null);
                Scribe_Values.Look(ref Legacy_SubtitleTextFormat, "SubtitleTextFormat", null);

                Scribe_Collections.Look(ref OverlayWidgets, "OverlayWidgets", LookMode.Deep);

                if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.LoadingVars)
                    OverlayWidgets ??= DawnDefault.OverlayWidgets;

                #endregion

                #region Sound Section

                Scribe_Values.Look(ref SoundEnabled, "SoundEnabled", DawnDefault.SoundEnabled);
                Scribe_Values.Look(ref m_SoundDefName, "SoundDefName", DawnData.DefaultSoundDefName);

                Scribe_Values.Look(ref SoundVolume, "SoundVolume", DawnDefault.SoundVolume);
                Scribe_Values.Look(ref SoundPitch, "SoundPitch", DawnDefault.SoundPitch);

                #endregion

                #region Extra Section

                Scribe_Values.Look(ref StartsAtZero, "StartsAtZero", DawnDefault.StartsAtZero);
                Scribe_Values.Look(ref ShowEveryXDays, "ShowEveryXDays", DawnDefault.ShowEveryXDays);
                Scribe_Values.Look(ref TriggerHour, "TriggerHour", DawnDefault.TriggerHour);

                #endregion

                #region Compatibility Section

                #region Modern Notifications Section

                Scribe_Collections.Look(ref MN_ExcludeOccasionCategory, "MN_ExcludeOccasionCategory");

                Scribe_Deep.Look(ref MN_Reminder, "MN_Reminder");
                Scribe_Deep.Look(ref MN_Occasion, "MN_Occasion");

                if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    MN_ExcludeOccasionCategory ??= DawnDefault.MN_ExcludeOccasionCategory;

                    MN_Reminder ??= DawnDefault.MN_Reminder;
                    MN_Reminder.TextStyle ??= DawnDefault.MN_ReminderTextStyle;

                    MN_Occasion ??= DawnDefault.MN_Occasion;
                    MN_Occasion.TextStyle ??= DawnDefault.MN_OccasionTextStyle;
                }

                #endregion

                #endregion
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
        }
    }
}
