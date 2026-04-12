#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class IssoSettingMaster : MasterTable<string, Setting>
{
    protected string GetString(string key)
    {
        var record = Find(key);
        if (record == null)
        {
            LogManager.LogError("Setting not found: " + key);
            return string.Empty;
        }

        return record.Value;
    }

    protected int GetInt(string key)
    {
        var value = GetString(key);
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        if (int.TryParse(value, out var result) == false)
        {
            LogManager.LogError("Failed to parse setting: " + key);
            return 0;
        }

        return result;
    }
    
    /// <summary>
    /// botの返信を遅延させる最大時間(ms)
    /// </summary>
    public int ReplyMaxDuration => GetInt(nameof(ReplyMaxDuration));

    /// <summary>
    /// botの返信時に入力中状態にする最大時間(ms)
    /// </summary>
    public int TypingMaxDuration => GetInt(nameof(TypingMaxDuration));

    /// <summary>
    /// 毎日リセットの0:00からのオフセット時間(ms)
    /// </summary>
    public int DailyResetTime => GetInt(nameof(DailyResetTime));

    /// <summary>
    /// 毎月リセットの日
    /// </summary>
    public int MonthlyResetDay => GetInt(nameof(MonthlyResetDay));

    /// <summary>
    /// 創造主の誕生月
    /// </summary>
    public int BirthdayMonth => GetInt(nameof(BirthdayMonth));

    /// <summary>
    /// 創造主の誕生日
    /// </summary>
    public int BirthdayDay => GetInt(nameof(BirthdayDay));

    /// <summary>
    /// 確率返信の最大確率(千分率)
    /// </summary>
    public int MaxRareReplyProbabilityPermillage => GetInt(nameof(MaxRareReplyProbabilityPermillage));

    public Multiplier MaxRareReplyProbability => Multiplier.FromPermillage(MaxRareReplyProbabilityPermillage);

    /// <summary>
    /// 確率返信の抽選単位(千分率)
    /// </summary>
    public int RareReplyProbabilityStepPermillage => GetInt(nameof(RareReplyProbabilityStepPermillage));

    public Multiplier RareReplyProbabilityStep => Multiplier.FromPermillage(RareReplyProbabilityStepPermillage);

    /// <summary>
    /// 単発ガチャ1回の価格
    /// </summary>
    public int PricePerGachaOnce => GetInt(nameof(PricePerGachaOnce));

    /// <summary>
    /// 10連ガチャ1回の価格
    /// </summary>
    public int PricePerGachaTenTimes => GetInt(nameof(PricePerGachaTenTimes));

    /// <summary>
    /// 単発確定ガチャ1回の価格
    /// </summary>
    public int PricePerGachaOnceCertain => GetInt(nameof(PricePerGachaOnceCertain));

    /// <summary>
    /// 購入情報表示時の表示ユーザー数
    /// </summary>
    public int PurchaseInfoRankingViewUserCount => GetInt(nameof(PurchaseInfoRankingViewUserCount));

    /// <summary>
    /// スロット1回の価格
    /// </summary>
    public int PricePerSlotOnce => GetInt(nameof(PricePerSlotOnce));

    /// <summary>
    /// スロット実行時にdiscordに送信するメッセージ上の、リール回転中の絵文字フォーマット
    /// </summary>
    public string SlotReelRollingFormat => GetString(nameof(SlotReelRollingFormat));

    /// <summary>
    /// スロット実行時にdiscordに送信するメッセージ上の、レバーの絵文字フォーマット
    /// </summary>
    public string SlotLeverFormat => GetString(nameof(SlotLeverFormat));

    /// <summary>
    /// 各ユーザーの1日あたりのスロット実行制限回数
    /// </summary>
    public int UserSlotExecuteLimitPerDay => GetInt(nameof(UserSlotExecuteLimitPerDay));

    /// <summary>
    /// スロットの調子の最大値(千分率)
    /// </summary>
    public int SlotMaxConditionOffsetPermillage => GetInt(nameof(SlotMaxConditionOffsetPermillage));

    public Multiplier SlotMaxConditionOffset => Multiplier.FromPermillage(SlotMaxConditionOffsetPermillage);

    /// <summary>
    /// スロットの調子の最小値(千分率)
    /// </summary>
    public int SlotMinConditionOffsetPermillage => GetInt(nameof(SlotMinConditionOffsetPermillage));

    public Multiplier SlotMinConditionOffset => Multiplier.FromPermillage(SlotMinConditionOffsetPermillage);

    /// <summary>
    /// スロットの次に同じ出目が確定する確率の最大値(千分率)
    /// </summary>
    public int SlotRepeatPermillageUpperBound => GetInt(nameof(SlotRepeatPermillageUpperBound));

    public Multiplier SlotRepeatUpperBound => Multiplier.FromPermillage(SlotRepeatPermillageUpperBound);

    /// <summary>
    /// 汎用的な笑顔の絵文字フォーマット
    /// </summary>
    public string CommonSmileFormat => GetString(nameof(CommonSmileFormat));

    /// <summary>
    /// 汎用的な存在しないメッセージ
    /// </summary>
    public string CommonMissingMessage => GetString(nameof(CommonMissingMessage));

    /// <summary>
    /// 汎用的な怒り表現のフォーマット
    /// </summary>
    public string CommonAngryFormat => GetString(nameof(CommonAngryFormat));

    /// <summary>
    ///MBTI返信のチャンネル単位クールタイム(ms)
    /// </summary>
    public int MbtiReplyCooldownDuration => GetInt(nameof(MbtiReplyCooldownDuration));

    /// <summary>
    ///MBTI返信で使う共通解説メッセージ
    /// </summary>
    public string MbtiReplyMessage => GetString(nameof(MbtiReplyMessage));
}
