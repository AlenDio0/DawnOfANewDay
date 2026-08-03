using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnMod : Mod
    {
        public static DawnSettings Settings;
        public static ModContentPack ModContent;

        public DawnMod(ModContentPack content)
            : base(content)
        {
            Settings = GetSettings<DawnSettings>();
            ModContent = content;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            if (Settings == null)
            {
                DawnData.Error("DawnSettings is null");
                return;
            }

            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory() => DawnData.ModName;
    }
}
