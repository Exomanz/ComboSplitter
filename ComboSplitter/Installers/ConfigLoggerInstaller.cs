using IPA.Logging;
using SiraUtil;
using Zenject;

namespace ComboSplitter.Installers
{
    public class ConfigLoggerInstaller : Installer<ConfigLoggerInstaller>
    {
        readonly Config _config;
        readonly Logger _logger;

        public ConfigLoggerInstaller(Config config, Logger logger)
        {
            _config = config;
            _logger = logger;

            logger.Info("Injecting Config and Logger systems...");
        }

        public override void InstallBindings()
        {
            Container.Bind<Config>().FromInstance(_config).AsCached();
            Container.BindLoggerAsSiraLogger(_logger);
        }
    }
}
