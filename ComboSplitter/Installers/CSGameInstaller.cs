using ComboSplitter.Services;
using UnityEngine;
using Zenject;

namespace ComboSplitter.Installers
{
    public class CSGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<CustomComboPanelController>().FromNewComponentOn(new GameObject("CustomComboPanelController")).AsSingle().NonLazy();
        }
    }
}
