using System;
using ConditionalScoreSubmission.Configuration;
using JetBrains.Annotations;
using SiraUtil.Submissions;
using Zenject;

namespace ConditionalScoreSubmission.Managers;

[UsedImplicitly]
internal class ResultsManager : IInitializable, IDisposable
{
    private static PluginConfig Config => PluginConfig.Instance;
    
    private readonly Submission _submission;
    private Ticket? _ticket;
    
    private static StandardLevelScenesTransitionSetupDataSO _standardLevelScenesTransitionSetupDataSo = null!;
    private static ScoreController _scoreController = null!;

    [Inject]
    internal ResultsManager(Submission submission, StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupDataSo, ScoreController scoreController)
    {
        _submission = submission;
        _standardLevelScenesTransitionSetupDataSo = standardLevelScenesTransitionSetupDataSo;
        _scoreController = scoreController;
    }

    public void Initialize()
    {
        _ticket = _submission.DisableScoreSubmission("ConditionalScoreSubmission");
        _standardLevelScenesTransitionSetupDataSo.didFinishEvent += StandardLevelScenesTransitionSetupDataSoOnDidFinishEvent;
    }

    public void Dispose()
    {
        _standardLevelScenesTransitionSetupDataSo.didFinishEvent -= StandardLevelScenesTransitionSetupDataSoOnDidFinishEvent;
    }

    private void StandardLevelScenesTransitionSetupDataSoOnDidFinishEvent(
        StandardLevelScenesTransitionSetupDataSO transitionSetupData, LevelCompletionResults results)
    {
        string reason = string.Empty;

        if (Config.JustDisableSubmission)
        {
            reason = "Score submission was disabled";
            goto finish;
        }
        
        if (results.energy == 0 && results.gameplayModifiers.noFailOn0Energy && Config.DisableNoFailSubmission)
        {
            reason = "Failed with No Fail enabled";
            goto finish;
        }

        if (Config.CapAccuracy &&
            results.modifiedScore / (double)_scoreController.immediateMaxPossibleModifiedScore < Config.MinimumAccuracy)
        {
            reason = $"Accuracy was below {Config.MinimumAccuracy * 100:0.0}%";
            goto finish;
        }

        if (Config.DisableNonFullCombos && !results.fullCombo)
        {
            reason = "Not a full combo";
            goto finish;
        }

        if (Config.CapMistakeCount &&
            results.notGoodCount + results.missedCount >= Config.MaximumMistakeCount)
        {
            reason = $"At least {Config.MaximumMistakeCount} mistake{(Config.MaximumMistakeCount != 1 ? "s were" : " was")} made (detected {results.notGoodCount + results.missedCount})";
        }
        
        finish:
            if (!string.IsNullOrEmpty(reason))
            {
                _ticket?.AddReason(reason);
            }
            else
            {
                if (_ticket != null) _submission.Remove(_ticket);
            }
    }
}
