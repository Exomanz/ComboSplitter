using ComboSplitter.Services;
using UnityEngine;
using Zenject;

namespace ComboSplitter.Installers
{
    public class CSGameInstaller : Installer<CSGameInstaller>
    {
        [Inject] private readonly CSConfig config;

        public override void InstallBindings()
        {
            if (!config.Enabled) return;

            Container.Bind<CustomComboPanelController>().FromNewComponentOn(new GameObject("CustomComboPanelController")).AsSingle().NonLazy();
        }
    }
}
