using ConditionalScoreSubmission.Configuration;
using ConditionalScoreSubmission.Installers;
using IPA;
using IPA.Config.Stores;
using JetBrains.Annotations;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;
using IPAConfig = IPA.Config.Config;

namespace ConditionalScoreSubmission;

[Plugin(RuntimeOptions.DynamicInit)]
[NoEnableDisable]
[UsedImplicitly]
internal class Plugin
{
    internal static IPALogger Log { get; private set; } = null!;

    [Init]
    public Plugin(IPALogger ipaLogger, IPAConfig ipaConfig, Zenjector zenjector)
    {
        Log = ipaLogger;
        zenjector.UseLogger(Log);
        
        PluginConfig c = ipaConfig.Generated<PluginConfig>();
        PluginConfig.Instance = c;
        
        zenjector.Install<MenuInstaller>(Location.Menu);
        zenjector.Install<GameInstaller>(Location.Player);
        
        Log.Info("Plugin loaded");
    }
}