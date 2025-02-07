using ComboSplitter.Services;
using UnityEngine;
using Zenject;

namespace ComboSplitter.Installers
{
    public class CSGameInstaller : Installer
    {
#pragma warning disable CS8618, CS0649
        [Inject] private readonly CSConfig config;
#pragma warning restore CS8618, CS0649

        public override void InstallBindings()
        {
            if (!config.Enabled) return;
            Container.Bind<CustomComboPanelController>().FromNewComponentOn(new GameObject("CustomComboPanelController")).AsSingle().NonLazy();
        }
    }
}
