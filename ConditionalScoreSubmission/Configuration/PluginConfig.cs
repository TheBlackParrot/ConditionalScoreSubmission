using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using JetBrains.Annotations;

// ReSharper disable RedundantDefaultMemberInitializer

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace ConditionalScoreSubmission.Configuration;

[UsedImplicitly]
internal class PluginConfig
{
    public static PluginConfig Instance { get; set; } = null!;

    public virtual bool CapMistakeCount { get; set; } = false;
    public virtual int MaximumMistakeCount { get; set; } = 10;
    
    public virtual bool CapAccuracy { get; set; } = false;
    public virtual float MinimumAccuracy { get; set; } = 0.9f;
    
    public virtual bool DisableNoFailSubmission { get; set; } = false;
    
    public virtual bool DisableNonFullCombos { get; set; } = false;
    
    public virtual bool JustDisableSubmission { get; set; } = false;
}