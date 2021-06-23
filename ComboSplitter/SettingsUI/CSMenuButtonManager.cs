using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using System;
using Zenject;

namespace ComboSplitter.SettingsUI
{
    internal class CSMenuButtonManager : IInitializable, IDisposable
    {
        readonly MainFlowCoordinator _main;
        readonly CSFlowCoordinator _modFlow;
        readonly MenuButton _button;

        public CSMenuButtonManager(MainFlowCoordinator main, CSFlowCoordinator modFlow)
        {
            _main = main;
            _modFlow = modFlow;
            _button = new MenuButton("ComboSplitter", SummonFlowCoordinator);
        }

        public void Initialize() => MenuButtons.instance.RegisterButton(_button);

        public void Dispose()
        {
            if (BSMLParser.IsSingletonAvailable && MenuButtons.IsSingletonAvailable)
                MenuButtons.instance.UnregisterButton(_button);
        }

        private void SummonFlowCoordinator() => _main.PresentFlowCoordinator(_modFlow);
    }
}
