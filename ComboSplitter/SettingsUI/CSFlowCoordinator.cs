using BeatSaberMarkupLanguage;
using HMUI;
using Zenject;

namespace ComboSplitter.SettingsUI
{
    internal class CSFlowCoordinator : FlowCoordinator
    {
        MainFlowCoordinator _main;
        BaseSettingsClass _settings;

        [Inject] public void Construct(MainFlowCoordinator main, BaseSettingsClass settings)
        {
            _main = main;
            _settings = settings;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("ComboSplitter", ViewController.AnimationType.In);
                showBackButton = true;
                ProvideInitialViewControllers(_settings);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);
            _main.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical, false);
        }
    }
}
