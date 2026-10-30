using DawnNewDay.Compatibility;
using DawnNewDay.Dialogs;
using DawnNewDay.Utils;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using System.Linq;
using System.Reflection.Metadata;

namespace DawnNewDay
{
    public class DawnText : IDawnWidget, IExposable
    {
        public GUIStyle TextGUIStyle = new(Text.CurFontStyle);

        public string Format = "";

        public string FontFamilyName = "";
        public TextAnchor Anchor = TextAnchor.MiddleCenter;
        public int FontSize = 20;
        public Color TextColor = Color.white;

        public bool Bold = false;
        public bool Italic = false;

        public float OutlineThickness = 1f;
        public Color OutlineColor = Color.black;

        private static readonly Regex m_WholeColorTagRegex = new(@"</?color[^<>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DawnText()
        {
            TextGUIStyle.font = null;
            TextGUIStyle.richText = true;
        }

        public DawnText(string format, int fontSize, bool bold, bool italic, float outlineThickness = 1f)
        {
            TextGUIStyle.font = null;
            TextGUIStyle.richText = true;

            Format = format;
            FontSize = fontSize;
            Bold = bold;
            Italic = italic;

            OutlineThickness = outlineThickness;
        }

        public void DoContents(Listing_Standard listing, float scale)
        {
            try
            {
                listing.Label(DawnTranslation.Label_TextFormat);
                Format = listing.TextEntry(Format, 3);

                listing.Gap();

                Vector2 iconSize = new(30f, 30f);
                Rect hintsRect = listing.GetRect(iconSize.y);

                Rect formatHintsRect = new(new(hintsRect.x, hintsRect.y), iconSize);
                if (Widgets.ButtonImage(formatHintsRect.ContractedBy(4f), DawnData.CurlyBrackets))
                {
                    var hints = DawnTranslation.Hints_LabelFormat;
                    if (ModernNotifications.Present)
                        hints.AddRange(DawnTranslation.Hints_MN_LabelFormat);

                    Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_LabelFormatHints, hints));
                }

                Rect richHintsRect = new(new(formatHintsRect.xMax * 1.5f, hintsRect.y), iconSize);
                if (Widgets.ButtonImage(richHintsRect.ContractedBy(4f), DawnData.AngleBrackets))
                    Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_RichTextHints, DawnTranslation.Hints_RichText));

                Rect presetsRect = new(richHintsRect.xMax * 2f, hintsRect.y, hintsRect.width / 3f, hintsRect.height);
                if (Widgets.ButtonText(presetsRect, DawnTranslation.Label_LabelFormatPresets))
                {
                    var presets = DawnTranslation.Hints_LabelFormatPresets;
                    if (ModernNotifications.Present)
                        presets.AddRange(DawnTranslation.Hints_MN_LabelFormatPresets);

                    Find.WindowStack.Add(new Dialog_Hint(DawnTranslation.Label_LabelFormatPresets, presets));
                }

                listing.Gap(24f);

                if (listing.ButtonText($"{DawnTranslation.Label_FontFamily} ({FontFamilyName.Fallback(DawnTranslation.Label_DefaultFont)})"))
                {
                    Find.WindowStack.Add(new Dialog_ChooseFontFamily(fontName =>
                    {
                        if (FontFamilyName == fontName)
                            return;

                        FontFamilyName = fontName != DawnTranslation.Label_DefaultFont ? fontName : "";
                        UpdateFontFamily();
                    }, FontFamilyName));
                }

                if (listing.ButtonText(Anchor.ToString()))
                {
                    var anchorModes = SettingsHelper.CreateFloatMenu(Enum.GetValues(typeof(TextAnchor)).Cast<TextAnchor>(),
                        anchor => new(anchor.ToString(), () => Anchor = anchor));
                    Find.WindowStack.Add(anchorModes);
                }

                FontSize = Mathf.CeilToInt(listing.SliderLabeled($"{DawnTranslation.Label_FontSize} ({FontSize})", FontSize, 0f, 256f, 0.35f));
                OutlineThickness = SettingsHelper.SnapToStep(listing.SliderLabeled($"{DawnTranslation.Label_OutlineThickness} ({OutlineThickness})", OutlineThickness, 0f, 3f, 0.35f), 0.25f);

                listing.CheckboxLabeled(DawnTranslation.Label_Bold, ref Bold);
                listing.CheckboxLabeled(DawnTranslation.Label_Italic, ref Italic);

                listing.LabeledRadioColorPresets(ref TextColor, DawnTranslation.Label_TextColor);
                listing.LabeledRadioColorPresets(ref OutlineColor, DawnTranslation.Label_OutlineColor);

                ApplyToGUIStyle(scale);
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
        }

        public void Draw(Rect rect, string text, float scale)
        {
            if (OutlineThickness > 0f)
                DrawOutline(rect, text, scale);

            Color defaultColor = GUI.contentColor;
            GUI.contentColor = TextColor;

            GUI.Label(rect, text, TextGUIStyle);

            GUI.contentColor = defaultColor;
        }

        private void DrawOutline(Rect rect, string text, float scale)
        {
            float scaledOutlineThickness = OutlineThickness * scale;

            Color defaultColor = GUI.contentColor;
            GUI.contentColor = OutlineColor;

            string outlineText = m_WholeColorTagRegex.Replace(text, "");

            float step = scaledOutlineThickness / 5f;
            for (float x = -scaledOutlineThickness; x <= scaledOutlineThickness; x += step)
            {
                for (float y = -scaledOutlineThickness; y <= scaledOutlineThickness; y += step)
                {
                    Vector2 offset = new(x, y);
                    GUI.Label(new Rect(rect.position + offset, rect.size), outlineText, TextGUIStyle);
                }
            }

            GUI.contentColor = defaultColor;
        }

        public void ApplyToGUIStyle(float scale)
        {
            if (TextGUIStyle == null)
                return;

            TextGUIStyle.alignment = Anchor;
            TextGUIStyle.fontSize = Mathf.CeilToInt(FontSize * scale);
            TextGUIStyle.normal.textColor = TextColor;
            TextGUIStyle.fontStyle = SettingsHelper.MapFontStyle(Bold, Italic);
        }

        public void UpdateFontFamily()
        {
            if (TextGUIStyle == null)
                return;

            try
            {
                if (TextGUIStyle.font != null)
                    UnityEngine.Object.Destroy(TextGUIStyle.font);

                if (FontFamilyName.NullOrEmpty())
                {
                    TextGUIStyle.font = null;
                    return;
                }

                TextGUIStyle.font = Font.CreateDynamicFontFromOSFont(FontFamilyName, 16);
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
        }

        public Vector2 CalcSize(string text)
        {
            if (text.NullOrEmpty())
                return Vector2.zero;

            return TextGUIStyle.CalcSize(new GUIContent(text));
        }

        public void ExposeData()
        {
            TextGUIStyle ??= new GUIStyle(Text.CurFontStyle);

            Scribe_Values.Look(ref Format, "Format", "");

            Scribe_Values.Look(ref FontFamilyName, "FontFamilyName", "");
            Scribe_Values.Look(ref Anchor, "Anchor", TextAnchor.MiddleCenter);
            Scribe_Values.Look(ref FontSize, "FontSize", 20);
            Scribe_Values.Look(ref TextColor, "TextColor", Color.white);

            Scribe_Values.Look(ref Bold, "Bold", false);
            Scribe_Values.Look(ref Italic, "Italic", false);

            Scribe_Values.Look(ref OutlineThickness, "OutlineThickness", 1f);
            Scribe_Values.Look(ref OutlineColor, "OutlineColor", Color.black);
        }
    }
}
