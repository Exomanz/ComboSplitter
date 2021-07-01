using BeatSaberMarkupLanguage.Settings;
using ComboSplitter.Installers;
using ComboSplitter.SettingsUI;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;
using System.Reflection;

namespace ComboSplitter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static IPALogger Logger { get; private set; }
        internal static Config XConfig { get; private set; }
        internal static Harmony harmonyID = null;

        [Init] public Plugin(IPAConfig iConfig, PluginMetadata metadata, IPALogger iLogger, Zenjector zenjector)
        {
            Logger = iLogger;
            XConfig = iConfig.Generated<Config>();
            XConfig.Version = metadata.Version;

            zenjector.OnGame<GameInstaller>().Expose<ComboUIController>().OnlyForStandard();
        }

        [OnEnable]
        public void Enable()
        {
            if (harmonyID is null) harmonyID = new Harmony("bs.Exomanz.ComboSplitter");
            if (XConfig.Enabled) harmonyID.PatchAll(Assembly.GetExecutingAssembly());
            BSMLSettings.instance.AddSettingsMenu("ComboSplitter", "ComboSplitter.SettingsUI.mainSettings.bsml", BaseSettingsClass.instance);
        }

        [OnDisable] public void Disable()
        {
            harmonyID.UnpatchAll(harmonyID.Id);
            harmonyID = null;
        }
    }
}
