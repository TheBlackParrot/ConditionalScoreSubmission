using ConditionalScoreSubmission.Managers;
using JetBrains.Annotations;
using Zenject;

namespace ConditionalScoreSubmission.Installers;

[UsedImplicitly]
internal class GameInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<ResultsManager>().AsSingle();
    }
}