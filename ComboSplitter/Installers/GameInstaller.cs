using ComboSplitter.Services;
using System.Reflection;
using UnityEngine.SceneManagement;
using Zenject;

namespace ComboSplitter.Installers
{
    public class GameInstaller : Installer<GameInstaller>
    {
        private Config _config => Plugin.XConfig;

        public override void InstallBindings()
        {
            bool isMultiplayer = SceneManager.GetSceneByName("MultiplayerGameplay").isLoaded;
            if (isMultiplayer || !_config.Enabled)
            {
                if (isMultiplayer) return;
                if (!_config.Enabled)
                {
                    Plugin.harmonyID.UnpatchAll(Plugin.harmonyID.Id);
                    return;
                }
            }

            Plugin.harmonyID.PatchAll(Assembly.GetExecutingAssembly());
            Container.BindInterfacesAndSelfTo<CustomComboPanelController>().AsSingle();
        }
    }
}
