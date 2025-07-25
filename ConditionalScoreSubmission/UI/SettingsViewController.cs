using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using ConditionalScoreSubmission.Configuration;
using JetBrains.Annotations;
using Zenject;

namespace ConditionalScoreSubmission.UI;

[UsedImplicitly]
internal class SettingsViewController : IInitializable, IDisposable
{
    private static PluginConfig Config => PluginConfig.Instance;
    
    private readonly GameplaySetup _gameplaySetup;

    public string PercentageFormatter(float x) => x.ToString("0.0%");

    private SettingsViewController(GameplaySetup gameplaySetup)
    {
        _gameplaySetup = gameplaySetup;
    }
    
    public void Initialize()
    {
        _gameplaySetup.AddTab("ScoreSubmission", "ConditionalScoreSubmission.UI.BSML.Settings.bsml", this);
    }

    public void Dispose()
    {
        _gameplaySetup.RemoveTab("ScoreSubmission");
    }

    protected bool CapMistakeCount
    {
        get => Config.CapMistakeCount;
        set => Config.CapMistakeCount = value;
    }
    protected int MaximumMistakeCount
    {
        get => Config.MaximumMistakeCount;
        set => Config.MaximumMistakeCount = value;
    }

    protected bool CapAccuracy
    {
        get => Config.CapAccuracy;
        set => Config.CapAccuracy = value;
    }
    protected float MinimumAccuracy
    {
        get => Config.MinimumAccuracy;
        set => Config.MinimumAccuracy = value;
    }

    protected bool DisableNoFailSubmission
    {
        get => Config.DisableNoFailSubmission;
        set => Config.DisableNoFailSubmission = value;
    }

    protected bool DisableNonFullCombos
    {
        get => Config.DisableNonFullCombos;
        set => Config.DisableNonFullCombos = value;
    }
}