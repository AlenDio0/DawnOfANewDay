using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DawnNewDay.Utils
{
    public static class SettingsHelper
    {
        public static FloatMenu CreateFloatMenu<T>(IEnumerable<T> values, Func<T, FloatMenuOption> optionFactory)
        {
            var options = new List<FloatMenuOption>();

            foreach (var value in values)
                options.Add(optionFactory(value));

            return new FloatMenu(options);
        }

        public static string Fallback(this string str, string fallback) => !str.NullOrEmpty() ? str : fallback;

        public static float SnapToStep(float value, float step) => Mathf.Round(value / step) * step;

        public static FontStyle MapFontStyle(bool bold, bool italic)
        {
            return bold && italic ? FontStyle.BoldAndItalic :
                bold ? FontStyle.Bold : italic ? FontStyle.Italic :
                FontStyle.Normal;
        }

        public readonly static Color[] ColorPresets = new[] { Color.white, Color.gray, Color.black, Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };

        public static Color LabeledRadioColorPresets(this Listing_Standard listing, ref Color currentColor, string label, float labelWidthPct = 0.25f)
        {
            Rect rowRect = listing.GetRect(30f);

            Rect labelRect = rowRect.LeftPart(labelWidthPct).CenteredOnYIn(rowRect);

            TextAnchor defaultAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(labelRect, label);
            Text.Anchor = defaultAnchor;

            const float cBoxSize = 24f;
            for (int i = 0; i < ColorPresets.Length; i++)
            {
                Color color = ColorPresets[i];

                Rect boxRect = new Rect(labelRect.xMax + (i * (cBoxSize * 1.5f)), rowRect.y, cBoxSize, cBoxSize);
                Widgets.DrawBoxSolid(boxRect, color);

                if (currentColor == color)
                    Widgets.DrawBox(boxRect, -2);

                if (Widgets.ButtonInvisible(boxRect))
                    currentColor = color;
            }

            return currentColor;
        }

        public static void LabeledTextFieldNumeric<T>(this Listing_Standard listing, string label, ref T value, ref string buffer, float min, float max, float labelWidthPct = 0.9f, float middleWidthPct = 1f, float height = 30f) where T : struct
        {
            Rect entryRect = listing.LabelWithWidget(label, labelWidthPct, middleWidthPct, height);
            Widgets.TextFieldNumeric(entryRect, ref value, ref buffer, min, max);
        }

        public static void LabeledTextEntry(this Listing_Standard listing, string label, ref string value, float labelWidthPct = 0.75f, float middleWidthPct = 1f, float height = 30f)
        {
            Rect entryRect = listing.LabelWithWidget(label, labelWidthPct, middleWidthPct, height);
            value = Widgets.TextField(entryRect, value);
        }

        public static Rect LabelWithWidget(this Listing_Standard listing, string label, float labelWidthPct, float middleWidthPct, float height = 30f)
        {
            Rect rowRect = listing.GetRect(height);
            Rect labelRect = rowRect.LeftPart(labelWidthPct).CenteredOnYIn(rowRect);

            TextAnchor defaultAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(labelRect, label);
            Text.Anchor = defaultAnchor;

            return rowRect.RightPart(1f - labelWidthPct).MiddlePart(middleWidthPct, 1f);
        }

        public static bool SectionButton(this Listing_Standard listing, string label, ref bool open)
        {
            listing.GapLine();
            Rect headerRect = listing.GetRect(30f);

            const float cSymbolSize = 20f;
            Rect symbolRect = new Rect(headerRect.x, headerRect.y + (headerRect.height - cSymbolSize) / 2f, cSymbolSize, cSymbolSize);
            GUI.DrawTexture(symbolRect, open ? TexButton.Minus : TexButton.Plus);

            TextAnchor defaultAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;

            const float cSpaceX = 15f;
            Widgets.Label(new Rect(symbolRect.xMax + cSpaceX, headerRect.y, headerRect.width - symbolRect.width, headerRect.height), label);

            Text.Anchor = defaultAnchor;

            if (Mouse.IsOver(headerRect))
                Widgets.DrawHighlight(headerRect);

            if (Widgets.ButtonInvisible(headerRect, true))
                open = !open;

            if (open)
                listing.Gap();

            return open;
        }

        public static void BeginIndentation(this Listing_Standard listing, float gap = 64f)
        {
            listing.Indent(gap);
            listing.ColumnWidth -= gap;
        }

        public static void EndIndentation(this Listing_Standard listing, float gap = 64f)
        {
            listing.Outdent(gap);
            listing.ColumnWidth += gap;
        }

        public static Listing_Standard Indented(this Listing_Standard listing, Action action, float gap = 64f)
        {
            BeginIndentation(listing, gap);

            action?.Invoke();

            EndIndentation(listing, gap);

            return listing;
        }
    }
}
