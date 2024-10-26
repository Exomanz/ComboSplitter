using Zenject;

namespace ComboSplitter.Installers
{
    public class AppInstaller : Installer<AppInstaller>
    {
        private CSConfig _config;

        [Inject] public AppInstaller(CSConfig config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.Bind<CSConfig>().FromInstance(_config).AsCached();
        }
    }
}
