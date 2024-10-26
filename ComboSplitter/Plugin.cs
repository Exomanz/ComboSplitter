using ComboSplitter.Installers;
using ComboSplitter.SettingsUI;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace ComboSplitter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        [Init] public Plugin(IPALogger logger, IPAConfig config, Zenjector zenjector)
        {
            zenjector.UseLogger(logger);
            zenjector.Expose<ComboUIController>("Environment");

            zenjector.Install<AppInstaller>(Location.App, config.Generated<CSConfig>());
            zenjector.Install(Location.Menu, (diContainer) =>
            {
                diContainer.BindInterfacesTo<CSViewController>().AsSingle();
            });
            zenjector.Install<CSGameInstaller>(Location.Player);
        }
    }
}
