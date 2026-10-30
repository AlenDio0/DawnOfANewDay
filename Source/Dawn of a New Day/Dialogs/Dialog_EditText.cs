using System;
using UnityEngine;
using Verse;

namespace DawnNewDay.Dialogs
{
    public class Dialog_EditText : Window
    {
        public DawnText TextStyle { get; }
        public int Index { get; }
        public float Scale { get; }
        public FormatContext Context { get; }

        public override Vector2 InitialSize => new(900f, 600f);

        public Dialog_EditText(DawnText textStyle, int textIndex, float scale, FormatContext context)
        {
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            draggable = true;

            TextStyle = textStyle;
            Index = textIndex;
            Scale = scale;
            Context = context;
        }

        public override void DoWindowContents(Rect canva)
        {
            Listing_Standard listing = new() { maxOneColumn = true };
            listing.Begin(canva);

            try
            {
                const float cRowHeight = 50f;
                Rect previewRect = listing.GetRect(cRowHeight);

                float textScale = TextStyle.FontSize <= cRowHeight ? 1f : cRowHeight / TextStyle.FontSize;
                string label = Context.FormatText(TextStyle.Format, Index, textScale);

                TextStyle.ApplyToGUIStyle(textScale);
                TextStyle.Draw(previewRect, label, textScale);

                TextStyle.ApplyToGUIStyle(Scale);

                listing.Gap();

                TextStyle.DoContents(listing, Scale);
            }
            catch (Exception exception)
            {
                DawnData.Exception(exception);
            }
            finally
            {
                listing.End();
            }
        }
    }
}
