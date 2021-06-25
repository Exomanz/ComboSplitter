using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using SemVer;
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
            set
            {
                _config.Enabled = value;
                if (_config.Enabled is true) Plugin.harmonyID.PatchAll();
                else Plugin.harmonyID.UnpatchAll(Plugin.harmonyID.Id);
            }
        }

        protected bool UseSaberColorScheme
        {
            get => _config.UseSaberColorScheme;
            set => _config.UseSaberColorScheme = value;
        }
    }
}
