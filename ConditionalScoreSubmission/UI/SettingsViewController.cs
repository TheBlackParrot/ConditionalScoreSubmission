using System;
using System.ComponentModel;
using BeatSaberMarkupLanguage.GameplaySetup;
using ConditionalScoreSubmission.Configuration;
using JetBrains.Annotations;
using Zenject;

namespace ConditionalScoreSubmission.UI;

[UsedImplicitly]
internal class SettingsViewController : INotifyPropertyChanged, IInitializable, IDisposable
{
    private static PluginConfig Config => PluginConfig.Instance;
    public event PropertyChangedEventHandler? PropertyChanged;
    
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
        set
        {
            Config.CapMistakeCount = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapMistakeCount)));
        }
    }

    protected int MaximumMistakeCount
    {
        get => Config.MaximumMistakeCount;
        set => Config.MaximumMistakeCount = value;
    }

    protected bool CapAccuracy
    {
        get => Config.CapAccuracy;
        set
        {
            Config.CapAccuracy = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapAccuracy)));
        }
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

    protected bool JustDisableSubmission
    {
        get => Config.JustDisableSubmission;
        set => Config.JustDisableSubmission = value;
    }
}