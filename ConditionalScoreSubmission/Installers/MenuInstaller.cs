using ConditionalScoreSubmission.UI;
using JetBrains.Annotations;
using Zenject;

namespace ConditionalScoreSubmission.Installers;

[UsedImplicitly]
internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SettingsViewController>().AsSingle();
    }
}