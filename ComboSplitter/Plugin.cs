using BeatSaberMarkupLanguage.GameplaySetup;
using ComboSplitter.Installers;
using ComboSplitter.SettingsUI;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPAConfig = IPA.Config.Config;
using SiraUtil.Zenject;

namespace ComboSplitter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static CSConfig pConfig { get; private set; }

        [Init] public Plugin(IPAConfig iConfig, Zenjector zenjector)
        {
            pConfig = iConfig.Generated<CSConfig>();

            zenjector.On<PCAppInit>().Pseudo(Container => 
                Container.Bind<CSConfig>().FromInstance(pConfig).AsCached());

            zenjector.OnGame<CSGameInstaller>().Expose<ComboUIController>().ShortCircuitForMultiplayer().ShortCircuitForTutorial();
            zenjector.OnGame<CSGameInstaller>(false).Expose<ComboUIController>().OnlyForMultiplayer();
        }

        [OnEnable]
        public void Enable()
        {
            GameplaySetup.instance.AddTab("ComboSplitter", "ComboSplitter.SettingsUI.main.bsml", CSViewController.instance);
        }

        [OnDisable] public void Disable()
        {
            GameplaySetup.instance.RemoveTab("ComboSplitter");
        }
    }
}
