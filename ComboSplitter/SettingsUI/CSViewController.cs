using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using System;
using Zenject;

namespace ComboSplitter.SettingsUI
{
    [ViewDefinition("ComboSplitter.SettingsUI.main.bsml")]
    internal class CSViewController : IInitializable, IDisposable
    {
#pragma warning disable CS8618, CS0649
        [Inject] private readonly CSConfig config;
#pragma warning restore CS8618, CS0649

        public void Initialize()
        {
            GameplaySetup.Instance.AddTab("ComboSplitter", "ComboSplitter.SettingsUI.main.bsml", this);
        }

        public void Dispose()
        {
            GameplaySetup.Instance.RemoveTab("ComboSplitter");
        }

        [UIValue("enabled")]
        protected bool Enabled
        {
            get => config.Enabled;
            set => config.Enabled = value;
        }

        [UIValue("colorScheme")]
        protected bool UseSaberColorScheme
        {
            get => config.UseSaberColorScheme;
            set => config.UseSaberColorScheme = value;
        }

        [UIValue("showHoverHint")]
        protected bool ShowResultsHoverHint
        {
            get => config.ShowResultsHoverHint;
            set => config.ShowResultsHoverHint = value;
        }

        [UIValue("colorSchemeInHint")]
        protected bool UseColorSchemeInHoverHint
        {
            get => config.UseColorSchemeInHoverHint;
            set => config.UseColorSchemeInHoverHint = value;
        }

        [UIValue("percentageInHint")]
        protected bool ShowPercentageInHoverHint
        {
            get => config.ShowPercentageInHoverHint;
            set => config.ShowPercentageInHoverHint = value;
        }

        [UIValue("missInfoInHint")]
        protected bool ShowMissInfoInHoverHint
        {
            get => config.ShowMissInfoInHoverHint;
            set => config.ShowMissInfoInHoverHint = value;
        }

        [UIValue("extendMissInfoInHint")]
        protected bool ExtendMissInfoInHoverHint
        {
            get => config.ExtendMissInfoInHoverHint;
            set => config.ExtendMissInfoInHoverHint = value;
        }
    }
}
