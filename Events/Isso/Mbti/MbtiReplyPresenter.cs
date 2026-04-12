using System.Text.RegularExpressions;
using Approvers.King.Common;

namespace Approvers.King.Events.Isso;

public partial class MbtiReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        if (ContainsMbtiType(Message.Content) == false)
        {
            return;
        }

        if (IsInCooldown())
        {
            return;
        }

        await SendReplyAsync(MasterManager.IssoSettingMaster.MbtiReplyMessage);
        AppCache.Instance.IssoMbtiLastReplyTimesByChannelId[Message.Channel.Id] = TimeManager.GetNow();
    }

    // MBTI表記が含まれるかだけを判定して返信条件を一本化する
    private static bool ContainsMbtiType(string content)
    {
        var match = MbtiRegex().Match(content);
        return match.Success;
    }

    // チャンネル単位で最終返信時刻を見て投稿頻度を制御する
    private bool IsInCooldown()
    {
        if (AppCache.Instance.IssoMbtiLastReplyTimesByChannelId.TryGetValue(Message.Channel.Id, out var lastReplyTime) == false)
        {
            return false;
        }

        var cooldownDuration = TimeSpan.FromMilliseconds(MasterManager.IssoSettingMaster.MbtiReplyCooldownDuration);
        return TimeManager.GetNow() < lastReplyTime + cooldownDuration;
    }

    private async Task SendReplyAsync(string message)
    {
        var replyMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.IssoSettingMaster.ReplyMaxDuration);
        await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(replyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            var typingMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.IssoSettingMaster.TypingMaxDuration);
            await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(typingMaxDelay)));
            await Message.ReplyAsync(message);
        }
    }

    [GeneratedRegex(@"\b[EI][NS][FT][JP](?:-[AT])?\b", RegexOptions.IgnoreCase)]
    private static partial Regex MbtiRegex();
}
