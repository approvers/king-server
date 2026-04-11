using Approvers.King.Common;

namespace Approvers.King;

public static class Program
{
    /// <summary>
    /// エントリーポイント
    /// </summary>
    private static void Main(string[] args)
    {
        BuildAsync(args).GetAwaiter().GetResult();
    }

    private static async Task BuildAsync(string[] args)
    {
        await InitializeModulesAsync();
        await InitializeStatesAsync();
        RegisterEvents();

        // 永久に待つ
        await Task.Delay(-1);
    }

    /// <summary>
    /// 共通基盤系を初期化する
    /// </summary>
    private static async Task InitializeModulesAsync()
    {
        TimeManager.Instance.Initialize();
        await MasterManager.FetchAsync();
        SchedulerManager.Initialize();
        await DiscordManager.InitializeAsync();
    }

    private static async Task InitializeStatesAsync()
    {
        await using var app = AppService.CreateSession();

        var gacha = await app.GetDefaultGachaAsync();
        if (gacha.GachaItems.Count == 0)
        {
            gacha.ShuffleMessageRates();
        }

        if (gacha.HitProbability == Multiplier.Zero)
        {
            gacha.ShuffleRareReplyRate();
        }

        await app.SaveChangesAsync();
    }

    /// <summary>
    /// 全てのイベントを登録する
    /// </summary>
    private static void RegisterEvents()
    {
        DiscordManager.IssoBot.RegisterEvents();
        DiscordManager.EyesBot.RegisterEvents();
        DiscordManager.LoxyBot.RegisterEvents();
        SchedulerManager.RegisterEvents();
    }
}
