using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

[MasterTable("angry")]
public class IssoAngryMaster : MasterTable<string, IssoAngry>
{
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class IssoAngry : MasterRecord<string>
{
    [field: MasterStringValue("target")]
    public override string Key { get; }

    [field: MasterStringValue("word")]
    public string Word { get; }

    [field: MasterIntValue("order")]
    public int Order { get; }
}
