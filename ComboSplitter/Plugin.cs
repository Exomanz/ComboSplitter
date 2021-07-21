using BeatSaberMarkupLanguage.GameplaySetup;
using ComboSplitter.Installers;
using ComboSplitter.SettingsUI;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
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

        [Init] public Plugin(IPAConfig iConfig, IPALogger iLogger, Zenjector zenjector)
        {
            Logger = iLogger;
            XConfig = iConfig.Generated<Config>();

            zenjector.OnGame<GameInstaller>().Expose<ComboUIController>().ShortCircuitForTutorial().ShortCircuitForMultiplayer();
        }

        [OnEnable]
        public void Enable()
        {
            if (harmonyID is null) harmonyID = new Harmony("bs.Exomanz.ComboSplitter");
            harmonyID.PatchAll(Assembly.GetExecutingAssembly());
            GameplaySetup.instance.AddTab("ComboSplitter", "ComboSplitter.SettingsUI.mainSettings.bsml", BaseSettingsClass.instance);
        }

        [OnDisable] public void Disable()
        {
            GameplaySetup.instance.RemoveTab("ComboSplitter");
            harmonyID.UnpatchAll(harmonyID.Id);
            harmonyID = null;
        }
    }
}
