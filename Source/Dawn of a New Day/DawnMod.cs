using UnityEngine;
using Verse;

namespace DawnNewDay
{
    public class DawnMod : Mod
    {
        public static DawnSettings s_Settings;
        public static ModContentPack s_ModContent;

        public DawnMod(ModContentPack content)
            : base(content)
        {
            s_Settings = GetSettings<DawnSettings>();
            s_ModContent = content;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (s_Settings == null)
            {
                DawnData.Error("DawnSettings is null");
                return;
            }

            s_Settings.DoWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() => DawnData.ModName;
    }
}
