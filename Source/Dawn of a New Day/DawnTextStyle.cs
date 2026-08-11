using DawnNewDay.Dialogs;
using DawnNewDay.Utils;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnTextStyle : IExposable
    {
        public GUIStyle TextGUIStyle = new(Text.CurFontStyle);

        public string FontFamilyName = "";
        public int FontSize = 24;
        public Color TextColor = Color.white;

        public bool Bold = false;
        public bool Italic = false;

        public float OutlineThickness = 1f;
        public Color OutlineColor = Color.black;

        private static readonly Regex WholeColorTagRegex = new(@"</?color[^<>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DawnTextStyle()
        {
            TextGUIStyle.alignment = TextAnchor.MiddleCenter;
            TextGUIStyle.font = null;

            TextGUIStyle.richText = true;
        }

        public DawnTextStyle(int fontSize, bool bold)
        {
            TextGUIStyle.alignment = TextAnchor.MiddleCenter;
            TextGUIStyle.font = null;

            TextGUIStyle.richText = true;

            FontSize = fontSize;
            Bold = bold;
        }

        public void DoContents(Listing_Standard listing, float scale)
        {
            try
            {
                if (listing.ButtonText($"{DawnTranslation.Label_FontFamily} ({FontFamilyName.Fallback(DawnTranslation.Label_DefaultFont)})"))
                {
                    Find.WindowStack.Add(new Dialog_ChooseFontFamily(font =>
                    {
                        FontFamilyName = font != DawnTranslation.Label_DefaultFont ? font : "";
                        UpdateFontFamily();
                    }, FontFamilyName));
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
                DawnData.Error($"Exception catched into DawnTextStyle.DoContents(listing: {listing})\nException: {exception}");
            }
        }

        public void Draw(Rect rect, string text, float scale)
        {
            if (OutlineThickness > 0f)
                DrawOutline(rect, text, scale);

            TextGUIStyle.normal.textColor = TextColor;
            GUI.Label(rect, text, TextGUIStyle);
        }

        private void DrawOutline(Rect rect, string text, float scale)
        {
            float scaledOutlineThickness = OutlineThickness * scale;

            TextGUIStyle.normal.textColor = OutlineColor;

            string outlineText = WholeColorTagRegex.Replace(text, "");

            float step = scaledOutlineThickness / 5f;
            for (float x = -scaledOutlineThickness; x <= scaledOutlineThickness; x += step)
            {
                for (float y = -scaledOutlineThickness; y <= scaledOutlineThickness; y += step)
                {
                    Vector2 offset = new(x, y);
                    GUI.Label(new Rect(rect.position + offset, rect.size), outlineText, TextGUIStyle);
                }
            }
        }

        public void ApplyToGUIStyle(float scale)
        {
            if (TextGUIStyle == null)
                return;

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
                DawnData.Error($"DawnTextStyle.UpdateFontFamily() Failed!\nFontFamilyName: '{FontFamilyName}'\nException: {exception}");
            }
        }

        public void ExposeData()
        {
            TextGUIStyle ??= new GUIStyle(Text.CurFontStyle);

            Scribe_Values.Look(ref FontFamilyName, "FontFamilyName", "");
            Scribe_Values.Look(ref FontSize, "FontSize", 24);
            Scribe_Values.Look(ref TextColor, "TextColor", Color.white);

            Scribe_Values.Look(ref Bold, "Bold", false);
            Scribe_Values.Look(ref Italic, "Italic", false);

            Scribe_Values.Look(ref OutlineThickness, "OutlineThickness", 1f);
            Scribe_Values.Look(ref OutlineColor, "OutlineColor", Color.black);
        }
    }
}
