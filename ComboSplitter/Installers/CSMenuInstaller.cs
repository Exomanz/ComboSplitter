using ComboSplitter.Services;
using ComboSplitter.SettingsUI;
using UnityEngine;
using Zenject;

namespace ComboSplitter.Installers
{
    public class CSMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            GameObject controllerObject = new GameObject("SpecificHandComboHoverHintController");
            Container.Bind<SpecificHandComboHoverHintController>().FromNewComponentOn(controllerObject).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CSViewController>().AsSingle();
        }
    }
}
