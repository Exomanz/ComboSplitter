using BeatSaberMarkupLanguage.Attributes;

namespace ComboSplitter.SettingsUI
{
    [ViewDefinition("ComboSplitter.SettingsUI.main.bsml")]
    internal class CSViewController : PersistentSingleton<CSViewController>
    {
        CSConfig config => Plugin.pConfig;

        [UIValue("Enabled")]
        protected bool Enabled
        {
            get => config.Enabled;
            set => config.Enabled = value;
        }

        [UIValue("ColScheme")]
        protected bool UseSaberColorScheme
        {
            get => config.UseSaberColorScheme;
            set => config.UseSaberColorScheme = value;
        }
    }
}
