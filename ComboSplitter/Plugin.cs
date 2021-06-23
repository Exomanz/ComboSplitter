using ComboSplitter.Installers;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;

namespace ComboSplitter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static IPALogger Logger { get; set; }
        internal static Harmony harmonyID = null;

        [Init] public Plugin(IPAConfig iConfig, PluginMetadata metadata, IPALogger iLogger, Zenjector zenjector)
        {
            Logger = iLogger;
            Config config = iConfig.Generated<Config>();
            config.Version = metadata.Version;

            zenjector.OnApp<ConfigLoggerInstaller>().WithParameters(config, iLogger);
            zenjector.OnMenu<CSMenuInstaller>();
            zenjector.OnGame<GameInstaller>().Expose<ComboUIController>().OnlyForStandard();
        }

        [OnEnable] public void Enable()
        {
            if (harmonyID is null) harmonyID = new Harmony("bs.Exomanz.ComboSplitter");
            harmonyID.PatchAll();
        }

        [OnDisable] public void Disable()
        {
            harmonyID.UnpatchAll(harmonyID.Id);
            harmonyID = null;
        }

    }
}
