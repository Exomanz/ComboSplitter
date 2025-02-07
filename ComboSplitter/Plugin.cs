using ComboSplitter.Installers;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace ComboSplitter
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        [Init] 
        public Plugin(IPALogger logger, IPAConfig config, Zenjector zenjector)
        {
            zenjector.Expose<CoreGameHUDController>("Environment");
            zenjector.Expose<CoreGameHUDController>("IsActiveObjects");

            zenjector.Install<CSAppInstaller>(Location.App, config.Generated<CSConfig>());
            zenjector.Install<CSMenuInstaller>(Location.Menu);
            zenjector.Install<CSGameInstaller>(Location.Player);
        }
    }
}
