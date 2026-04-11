using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class IssoTriggerPhraseMaster : MasterTable<string, IssoTriggerPhrase>;

public enum TriggerType
{
    Unknown,
    GachaExecute,
    GachaGet,
    GachaRanking,
    SlotExecute,
    SlotRanking,
    MasterShortcut,
}

/// <summary>
/// discordのメッセージからイベントを発動する文言
/// </summary>
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class IssoTriggerPhrase : MasterRecord<string>
{
    public override string Key => Id;

    [field: MasterStringValue("id")]
    public string Id { get; }

    [field: MasterEnumValue("trigger_type", typeof(TriggerType))]
    public TriggerType TriggerType { get; }

    [field: MasterStringValue("phrase")]
    public string Phrase { get; }
}
