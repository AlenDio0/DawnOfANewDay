using DawnNewDay.Utils;
using Verse;

namespace DawnNewDay.Compatibility
{
    public class DawnTextConfig : IExposable
    {
        public bool AddText = false;

        public int MaximumDays = 15;
        public string TextFormat = "";
        public DawnText TextStyle = null;

        private string MaximumTimeBuffer;

        public DawnTextConfig() { }

        public DawnTextConfig(bool addText, int maximumDays, string textFormat, DawnText textStyle)
        {
            AddText = addText;
            MaximumDays = maximumDays;
            TextFormat = textFormat;
            TextStyle = textStyle;
        }

        public void DoContents(Listing_Standard listing, float Scale)
        {
            listing.CheckboxLabeled(DawnTranslation.Label_AddText, ref AddText);

            if (AddText)
            {
                listing.LabeledTextFieldNumeric(DawnTranslation.Label_MaximumDays, ref MaximumDays, ref MaximumTimeBuffer, 1, 1200);

                listing.Gap();

                listing.Label(DawnTranslation.Label_TextFormat);
                TextFormat = listing.TextEntry(TextFormat, 2);

                listing.Gap();

                TextStyle?.DoContents(listing, Scale);
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref AddText, "AddText", false);
            Scribe_Values.Look(ref MaximumDays, "MaximumDays", 15);
            Scribe_Values.Look(ref TextFormat, "TextFormat", "");
            Scribe_Deep.Look(ref TextStyle, "TextStyle");
        }
    }
}
