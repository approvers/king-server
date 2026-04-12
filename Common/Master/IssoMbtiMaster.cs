using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618

namespace Approvers.King.Common;

public class IssoMbtiMaster : MasterTable<string, IssoMbti>
{
}

/// <summary>
///MBTIごとの解説メッセージ
/// </summary>
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class IssoMbti : MasterRecord<string>
{
    [field: MasterStringValue("mbti")]
    public override string Key { get; }

    [field: MasterStringValue("message")]
    public string Message { get; }
}
