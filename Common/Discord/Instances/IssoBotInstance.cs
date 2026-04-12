using Approvers.King.Events.Common;
using Approvers.King.Events.Isso;
using Discord;
using Discord.WebSocket;

namespace Approvers.King.Common.Instances;

public class IssoBotInstance : DiscordBotInstanceBase
{
    public IssoBotInstance() : base(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.MessageContent |
                         GatewayIntents.Guilds |
                         GatewayIntents.GuildMessages |
                         GatewayIntents.GuildVoiceStates
    })
    {
    }

    public override string DisplayName => "Isso";

    protected override string GetToken()
    {
        return EnvironmentManager.DiscordSecretIsso;
    }

    public void RegisterEvents()
    {
        Client.MessageReceived += message =>
        {
            OnMessageReceived(message);
            return Task.CompletedTask;
        };

        Client.UserVoiceStateUpdated += (user, before, after) =>
        {
            OnUserVoiceStateUpdated(user, before, after);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// トリガーとなる文言を含んでいるか
    /// </summary>
    private static bool IsContainsTriggerPhrase(string content, TriggerType triggerType)
    {
        return MasterManager.IssoTriggerPhraseMaster
            .GetAll(x => x.TriggerType == triggerType)
            .Any(x => content.Contains(x.Phrase));
    }

    private void OnMessageReceived(SocketMessage message)
    {
        // botは弾く
        if (message is not SocketUserMessage userMessage || userMessage.Author.IsBot) return;

        // チャンネルがutil_onlyの場合の判定
        var channelId = userMessage.Channel.Id.ToString();
        var channel = MasterManager.ChannelMaster.Find(channelId);
        var isUtilOnlyChannel = channel?.IsUtilOnly ?? false;

        // メッセージリンクが含まれている場合の処理
        if (userMessage.Content.Contains("discord.com/channels/") || userMessage.Content.Contains("discordapp.com/channels/"))
        {
            ExecuteMessageEventAsync<MessageLinkPresenter>(userMessage).Run();
        }

        if (userMessage.MentionedUsers.Any(x => x.Id == Client.CurrentUser.Id))
        {
            if (message.Content.EndsWith("reload"))
            {
                // マスタデータをリロード
                ExecuteMessageEventAsync<AdminMasterReloadPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaRanking))
            {
                // ガチャランキングの表示
                ExecuteMessageEventAsync<GachaRankingPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.SlotRanking))
            {
                // スロットランキングの表示
                ExecuteMessageEventAsync<SlotRankingPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaExecute))
            {
                // 10連ガチャ
                ExecuteMessageEventAsync<GachaCommandPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.GachaGet))
            {
                // 排出率を投稿する
                ExecuteMessageEventAsync<GachaInfoCommandPresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.SlotExecute))
            {
                // スロットを回す
                ExecuteMessageEventAsync<SlotExecutePresenter>(userMessage).Run();
                return;
            }

            if (IsContainsTriggerPhrase(userMessage.Content, TriggerType.MasterShortcut))
            {
                // マスターデータのURL表示
                ExecuteMessageEventAsync<MasterShortcutPresenter>(userMessage).Run();
                return;
            }

            // 返信
            ExecuteMessageEventAsync<GachaInteractReplyPresenter>(userMessage).Run();
            return;
        }

        // util_onlyチャンネルの場合、メンションなしの機能をスキップ
        if (isUtilOnlyChannel) return;

        // MBTI表記がある投稿へ解説を返す
        ExecuteMessageEventAsync<MbtiReplyPresenter>(userMessage).Run();

        // 全メッセージでAngryPresenterをトリガー
        // 実際に含まれる語だけAngryPresenter内で反応させる
        ExecuteMessageEventAsync<AngryPresenter>(userMessage).Run();
    }

    private void OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        new VoiceNotificationPresenter
        {
            User = user,
            Before = before,
            After = after
        }.RunAsync().Run();
    }
}
