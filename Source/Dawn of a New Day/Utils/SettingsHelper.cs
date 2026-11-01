using RimWorld;
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

        public readonly static Color[] ColorPresets = [Color.white, Color.gray, Color.black, Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan];

        public static Color LabeledRadioColorPresets(this Listing_Standard listing, ref Color currentColor, string label, float labelWidthPct = 0.25f)
        {
            Rect rowRect = listing.GetRect(DawnData.TinyRowHeight);

            Rect labelRect = rowRect.LeftPart(labelWidthPct).CenteredOnYIn(rowRect);
            WithAnchor(TextAnchor.MiddleLeft, () => Widgets.Label(labelRect, label));

            float boxSize = rowRect.height;
            for (int i = 0; i < ColorPresets.Length; i++)
            {
                Color color = ColorPresets[i];

                Rect boxRect = new(labelRect.xMax + (i * (boxSize * 1.5f)), rowRect.y, boxSize, boxSize);
                Widgets.DrawBoxSolid(boxRect.ContractedBy(4f), color);

                if (currentColor == color)
                    Widgets.DrawBox(boxRect.ContractedBy(2f), -2);

                if (Widgets.ButtonInvisible(boxRect))
                    currentColor = color;
            }

            return currentColor;
        }

        public static void LabeledTextFieldNumeric<T>(this Listing_Standard listing, string label, ref T value, ref string buffer, float min, float max, float labelWidthPct = 0.9f, float middleWidthPct = 1f, float height = DawnData.TinyRowHeight) where T : struct
        {
            Rect entryRect = listing.LabelWithWidget(label, labelWidthPct, middleWidthPct, height);
            Widgets.TextFieldNumeric(entryRect, ref value, ref buffer, min, max);
        }

        public static void LabeledTextEntry(this Listing_Standard listing, string label, ref string value, float labelWidthPct = 0.75f, float middleWidthPct = 1f, float height = DawnData.TinyRowHeight)
        {
            Rect entryRect = listing.LabelWithWidget(label, labelWidthPct, middleWidthPct, height);
            value = Widgets.TextField(entryRect, value);
        }

        public static Rect LabelWithWidget(this Listing_Standard listing, string label, float labelWidthPct, float middleWidthPct, float height = DawnData.TinyRowHeight)
        {
            Rect rowRect = listing.GetRect(height);

            Rect labelRect = rowRect.LeftPart(labelWidthPct).CenteredOnYIn(rowRect);
            WithAnchor(TextAnchor.MiddleLeft, () => Widgets.Label(labelRect, label));

            return rowRect.RightPart(1f - labelWidthPct).MiddlePart(middleWidthPct, 1f);
        }

        public static void LabeledCheckboxTable(this Listing_Standard listing, List<string> labels, ref Dictionary<string, bool> table, int columns = 3)
        {
            Rect currentRow = Rect.zero;
            int rowCount = 0;

            foreach (string category in labels)
            {
                if (rowCount == 0)
                    currentRow = listing.GetRect(DawnData.TinyRowHeight);

                float width = currentRow.width / columns;
                Rect column = new(currentRow.x + (width * rowCount), currentRow.y, width, currentRow.height);

                bool selected = table.TryGetValue(category, false);
                Widgets.CheckboxLabeled(column.MiddlePart(0.8f, 1f), category, ref selected);
                table[category] = selected;

                rowCount++;
                if (rowCount >= columns)
                    rowCount = 0;
            }
        }

        public static bool SectionButton(this Listing_Standard listing, string label, ref bool open, bool gapLine = true)
        {
            if (gapLine)
                listing.GapLine(4f);

            Rect headerRect = listing.GetRect(DawnData.TinyRowHeight);
            float iconSize = headerRect.height;

            Rect iconRect = new(headerRect.x, headerRect.y + (headerRect.height - iconSize) / 2f, iconSize, iconSize);
            GUI.DrawTexture(iconRect.ContractedBy(4f), open ? TexButton.Minus : TexButton.Plus);

            const float cSpaceX = 15f;

            WithAnchor(TextAnchor.MiddleLeft,
                () => Widgets.Label(new Rect(iconRect.xMax + cSpaceX, headerRect.y, headerRect.width - iconRect.width, headerRect.height), label));

            if (Mouse.IsOver(headerRect))
                Widgets.DrawHighlight(headerRect);

            if (Widgets.ButtonInvisible(headerRect, true))
                open = !open;

            if (open)
                listing.Gap(4f);

            return open;
        }

        public static void BeginIndentation(this Listing_Standard listing, float gap = 48f)
        {
            listing.Indent(gap);
            listing.ColumnWidth -= gap;
        }

        public static void EndIndentation(this Listing_Standard listing, float gap = 48f)
        {
            listing.Outdent(gap);
            listing.ColumnWidth += gap;
        }

        public static Listing_Standard Indented(this Listing_Standard listing, Action action, float gap = 32f)
        {
            BeginIndentation(listing, gap);

            action?.Invoke();

            EndIndentation(listing, gap);

            return listing;
        }

        public static void With<T>(Func<T> getter, Action<T> setter, T value, Action action)
        {
            if (getter == null || setter == null || action == null)
                return;

            T originalValue = getter.Invoke();
            setter.Invoke(value);

            try
            {
                action.Invoke();
            }
            finally
            {
                setter.Invoke(originalValue);
            }
        }

        public static void WithColor(Color value, Action action) =>
            With(() => GUI.color, set => GUI.color = set, value, action);
        public static void WithContentColor(Color value, Action action) =>
            With(() => GUI.contentColor, set => GUI.contentColor = set, value, action);
        public static void WithAnchor(TextAnchor value, Action action) =>
            With(() => Text.Anchor, set => Text.Anchor = set, value, action);
        public static void WithFont(GameFont value, Action action) =>
            With(() => Text.Font, set => Text.Font = set, value, action);
        public static void WithFontStyle(FontStyle value, Action action) =>
            With(() => Text.CurFontStyle.fontStyle, set => Text.CurFontStyle.fontStyle = set, value, action);
    }
}
