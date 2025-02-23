using ComboSplitter.Services;
using Zenject;

namespace ComboSplitter.Installers
{
    public class CSAppInstaller : Installer
    {
        private readonly CSConfig _config;

        [Inject] public CSAppInstaller(CSConfig config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.Bind<CSConfig>().FromInstance(_config).AsCached();
            Container.BindInterfacesAndSelfTo<ComboDataProcessor>().AsSingle();
        }
    }
}
