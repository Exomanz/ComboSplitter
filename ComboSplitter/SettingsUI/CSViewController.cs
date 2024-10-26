using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using System;
using Zenject;

namespace ComboSplitter.SettingsUI
{
    [ViewDefinition("ComboSplitter.SettingsUI.main.bsml")]
    internal class CSViewController : IInitializable, IDisposable
    {
        [Inject] private readonly CSConfig config;

        public void Initialize()
        {
            GameplaySetup.Instance.AddTab("ComboSplitter", "ComboSplitter.SettingsUI.main.bsml", this);
        }

        public void Dispose()
        {
            GameplaySetup.Instance.RemoveTab("ComboSplitter");
        }

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
