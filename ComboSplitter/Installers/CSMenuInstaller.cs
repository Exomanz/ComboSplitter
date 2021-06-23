using ComboSplitter.SettingsUI;
using SiraUtil;
using Zenject;

namespace ComboSplitter.Installers
{
    public class CSMenuInstaller : Installer<CSMenuInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<CSFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

            Container.Bind<BaseSettingsClass>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<CSMenuButtonManager>().AsSingle();
        }
    }
}
