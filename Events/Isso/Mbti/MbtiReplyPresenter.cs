using System.Text.RegularExpressions;
using Approvers.King.Common;

namespace Approvers.King.Events.Isso;

public partial class MbtiReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var mbti = FindMbtiType(Message.Content);
        if (mbti == null)
        {
            return;
        }

        if (IsInCooldown())
        {
            return;
        }

        var master = MasterManager.IssoMbtiMaster.Find(mbti);
        if (master == null)
        {
            return;
        }

        await SendReplyAsync(master.Message);
        AppCache.Instance.IssoMbtiLastReplyTimesByChannelId[Message.Channel.Id] = TimeManager.GetNow();
    }

    // 余計な記法を除去してマスタキーへ正規化する
    private static string? FindMbtiType(string content)
    {
        var match = MbtiRegex().Match(content);
        if (match.Success == false)
        {
            return null;
        }

        var value = match.Groups["mbti"].Value;
        return value.ToUpper();
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

    [GeneratedRegex(@"\b(?<mbti>[EI][NS][FT][JP])(?:-[AT])?\b", RegexOptions.IgnoreCase)]
    private static partial Regex MbtiRegex();
}
