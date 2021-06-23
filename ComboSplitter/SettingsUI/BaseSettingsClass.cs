using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;

namespace ComboSplitter.SettingsUI
{
    [ViewDefinition("ComboSplitter.SettingsUI.Views.mainSettings.bsml")]
    //[HotReload(RelativePathToLayout = @"..\SettingsUI\Views\mainSettings.bsml")]
    internal class BaseSettingsClass : BSMLAutomaticViewController
    {
        Config _config;

        [Inject] public void Construct(Config config) => _config = config;

        protected bool Enabled
        {
            get => _config.Enabled;
            set => _config.Enabled = value;
        }

        protected bool UseSaberColorScheme
        {
            get => _config.UseSaberColorScheme;
            set => _config.UseSaberColorScheme = value;
        }
    }
}
