using ComboSplitter.Services;
using Zenject;

namespace ComboSplitter.Installers
{
    public class GameInstaller : Installer<GameInstaller>
    {
        readonly Config _config;
        public GameInstaller(Config config) => _config = config;

        public override void InstallBindings()
        {
            if (_config.Enabled) Container.BindInterfacesAndSelfTo<CustomComboPanelController>().AsSingle();
        }
    }
}
