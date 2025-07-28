using ConditionalScoreSubmission.Configuration;
using JetBrains.Annotations;
using SiraUtil.Affinity;
using SiraUtil.Submissions;
using Zenject;

namespace ConditionalScoreSubmission.Managers;

[UsedImplicitly]
internal class ResultsManager : IInitializable, IAffinity
{
    private static PluginConfig Config => PluginConfig.Instance;
    
    private readonly Submission _submission;
    private Ticket? _ticket;

    [Inject]
    internal ResultsManager(Submission submission)
    {
        _submission = submission;
    }

    public void Initialize()
    {
        _ticket = _submission.DisableScoreSubmission("ConditionalScoreSubmission");
    }
    
    [AffinityPostfix]
    [AffinityPatch(typeof(StandardLevelFinishedController), nameof(StandardLevelFinishedController.StartLevelFinished))]
    // ReSharper disable once InconsistentNaming
    private void FillLevelCompletionResultsPatch(StandardLevelFinishedController __instance)
    {
        Plugin.Log.Info("StartLevelFinished called");
        
        LevelCompletionResults results = __instance._prepareLevelCompletionResults.FillLevelCompletionResults(
            LevelCompletionResults.LevelEndStateType.Incomplete, LevelCompletionResults.LevelEndAction.None);
        
        GameEnergyCounter gameEnergyCounter = __instance._prepareLevelCompletionResults._gameEnergyCounter;
        GameplayModifiers gameplayModifiers = __instance._prepareLevelCompletionResults._gameplayModifiers;
        IScoreController scoreController = __instance._prepareLevelCompletionResults._scoreController;
        
        string reason = string.Empty;

        if (Config.JustDisableSubmission)
        {
            reason = "Score submission was disabled";
            goto finish;
        }
        
        if (gameEnergyCounter._didReach0Energy && gameplayModifiers.noFailOn0Energy && Config.DisableNoFailSubmission)
        {
            reason = "Failed with No Fail enabled";
            goto finish;
        }

        if (Config.CapAccuracy &&
            scoreController.modifiedScore / (double)scoreController.immediateMaxPossibleModifiedScore < Config.MinimumAccuracy)
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
            if (_ticket == null)
            {
                return;
            }
            
            if (!string.IsNullOrEmpty(reason))
            {
                _ticket.AddReason(reason);
            }
            else
            {
                _submission.Remove(_ticket);
            }
    }
}
