using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events.Isso;

public class AngryPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        var messageContent = Message.Content.ToLower();

        // Angryは実際に含まれる語だけで反応させる
        var matchedAngry = MasterManager.IssoAngryMaster
            .GetAll(angry => messageContent.Contains(angry.Key.ToLower()))
            .OrderByDescending(angry => angry.Order)
            .FirstOrDefault();

        if (matchedAngry == null)
        {
            return;
        }

        await SendAngryReplyAsync(matchedAngry);
    }

    private async Task SendAngryReplyAsync(IssoAngry angry)
    {
        var replyMessage = $"今 ***\"{angry.Word}\"*** って言ったか？{MasterManager.IssoSettingMaster.CommonAngryFormat}";
        await SendReplyAsync(replyMessage);
    }

    private async Task SendReplyAsync(string message)
    {
        var replyMaxDelay = NumberUtility.GetSecondsFromMilliseconds(MasterManager.IssoSettingMaster.ReplyMaxDuration);
        await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(replyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            var typingMaxDelay =
                NumberUtility.GetSecondsFromMilliseconds(MasterManager.IssoSettingMaster.TypingMaxDuration);
            await Task.Delay(TimeSpan.FromSeconds(RandomManager.GetRandomFloat(typingMaxDelay)));
            await Message.ReplyAsync(message);
        }
    }
}
