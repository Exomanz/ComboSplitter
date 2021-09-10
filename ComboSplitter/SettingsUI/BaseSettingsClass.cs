using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using SemVer;
using Zenject;

namespace ComboSplitter.SettingsUI
{
    [ViewDefinition("ComboSplitter.SettingsUI.mainSettings.bsml")]
    [HotReload(RelativePathToLayout = @"..\SettingsUI\mainSettings.bsml")]
    internal class BaseSettingsClass : PersistentSingleton<BaseSettingsClass>
    {
        Config _config => Plugin.XConfig;

        [UIValue("Enabled")]
        protected bool Enabled
        {
            get => _config.Enabled;
            set => _config.Enabled = value;
        }

        [UIValue("ColScheme")]
        protected bool UseSaberColorScheme
        {
            get => _config.UseSaberColorScheme;
            set => _config.UseSaberColorScheme = value;
        }
    }
}
