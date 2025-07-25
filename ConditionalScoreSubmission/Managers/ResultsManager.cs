using System.Diagnostics.CodeAnalysis;
using ConditionalScoreSubmission.Configuration;
using SiraUtil.Affinity;
using SiraUtil.Submissions;

namespace ConditionalScoreSubmission.Managers;

internal class ResultsManager : IAffinity
{
    private static PluginConfig Config => PluginConfig.Instance;
    private readonly Submission _submission;

    internal ResultsManager(Submission submission)
    {
        _submission = submission;
    }
    
    [AffinityPostfix]
    [AffinityPatch(typeof(PrepareLevelCompletionResults), nameof(PrepareLevelCompletionResults.FillLevelCompletionResults))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private void FillLevelCompletionResults(PrepareLevelCompletionResults __instance, LevelCompletionResults __result)
    {
        string reason = string.Empty;
        
        if (__result.energy == 0 && __result.gameplayModifiers.noFailOn0Energy && Config.DisableNoFailSubmission)
        {
            reason = "Failed with No Fail enabled";
            goto finish;
        }

        if (Config.CapAccuracy &&
            __result.modifiedScore / (double)__instance._scoreController.immediateMaxPossibleModifiedScore < Config.MinimumAccuracy)
        {
            reason = $"Accuracy was below {Config.MinimumAccuracy * 100:0.0}%";
            goto finish;
        }

        if (Config.DisableNonFullCombos && !__result.fullCombo)
        {
            reason = "Not a full combo";
            goto finish;
        }

        if (Config.CapMistakeCount &&
            __result.notGoodCount + __result.missedCount >= Config.MaximumMistakeCount)
        {
            reason = $"At least {Config.MaximumMistakeCount} mistake{(Config.MaximumMistakeCount != 1 ? "s were" : " was")} made (detected {__result.notGoodCount + __result.missedCount})";
        }
        
        finish:
            if (!string.IsNullOrEmpty(reason))
            {
                _submission.DisableScoreSubmission("ConditionalScoreSubmission", reason);
            }
    }
}
