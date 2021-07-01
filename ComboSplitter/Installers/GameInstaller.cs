using ComboSplitter.Services;
using Zenject;

namespace ComboSplitter.Installers
{
    public class GameInstaller : Installer<GameInstaller>
    {
        private Config _config => Plugin.XConfig;

        public override void InstallBindings() {
            if (_config.Enabled) Container.BindInterfacesAndSelfTo<CustomComboPanelController>().AsSingle();
        }
    }
}
