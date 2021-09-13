using ComboSplitter.Services;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace ComboSplitter.Installers
{
    public class CSGameInstaller : Installer<CSGameInstaller>
    {
#pragma warning disable CS0649
        [Inject] CSConfig config;
 
        public override void InstallBindings()
        {
            if (!config.Enabled) return;

            Container.Bind<CustomComboPanelController>().FromNewComponentOn(
                new GameObject("CustomComboPanelController")).AsSingle().NonLazy();
        }
    }
#pragma warning disable CS0649
}
