using System.Reflection;

namespace Approvers.King.Common;

/// <summary>
/// マスタデータを管理する
/// </summary>
public class MasterManager : Singleton<MasterManager>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    [MasterTable("isso_angry")] private IssoAngryMaster _issoAngryMaster;
    public static IssoAngryMaster IssoAngryMaster => Instance._issoAngryMaster;

    [MasterTable("channel")] private ChannelMaster _channelMaster;
    public static ChannelMaster ChannelMaster => Instance._channelMaster;

    [MasterTable("isso_random_message")] private IssoRandomMessageMaster _issoRandomMessageMaster;
    public static IssoRandomMessageMaster IssoRandomMessageMaster => Instance._issoRandomMessageMaster;

    [MasterTable("isso_trigger_phrase")] private IssoTriggerPhraseMaster _issoTriggerPhraseMaster;
    public static IssoTriggerPhraseMaster IssoTriggerPhraseMaster => Instance._issoTriggerPhraseMaster;

    [MasterTable("isso_slot_item")] private IssoSlotItemMaster _issoSlotItemMaster;
    public static IssoSlotItemMaster IssoSlotItemMaster => Instance._issoSlotItemMaster;

    [field: MasterTable("common_setting")] private CommonSettingMaster _commonSettingMaster;
    public static CommonSettingMaster CommonSettingMaster => Instance._commonSettingMaster;

    [field: MasterTable("isso_setting")] private IssoSettingMaster _issoSettingMaster;
    public static IssoSettingMaster IssoSettingMaster => Instance._issoSettingMaster;

    [field: MasterTable("eyes_setting")] private EyesSettingMaster _eyesSettingMaster;
    public static EyesSettingMaster EyesSettingMaster => Instance._eyesSettingMaster;

    [field: MasterTable("loxy_setting")] private LoxySettingMaster _loxySettingMaster;
    public static LoxySettingMaster LoxySettingMaster => Instance._loxySettingMaster;

    [field: MasterTable("ichiyo_setting")] private IchiyoSettingMaster _ichiyoSettingMaster;
    public static IchiyoSettingMaster IchiyoSettingMaster => Instance._ichiyoSettingMaster;

    [field: MasterTable("ichiyo_user")] private IchiyoUserMaster _ichiyoUserMaster;
    public static IchiyoUserMaster IchiyoUserMaster => Instance._ichiyoUserMaster;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    /// <summary>
    /// マスタデータをスプレッドシートから読み込んで更新する
    /// </summary>
    public static async Task FetchAsync()
    {
        var dict = new Dictionary<FieldInfo, string>();
        var masterFields = Instance.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in masterFields)
        {
            var tableAttributes = field.GetCustomAttributes().FirstTypeOrDefault<Attribute, MasterTableAttribute>();
            if (tableAttributes == null) continue;
            dict.Add(field, tableAttributes.KeyName);
        }

        var sheets = await GoogleSheetManager.GetAllSheetsAsync(dict.Values);
        foreach (var (field, sheetName) in dict)
        {
            var masterType = field.FieldType;
            var masterGenericArguments = masterType.BaseType!.GetGenericArguments();
            var keyType = masterGenericArguments[0];
            var recordType = masterGenericArguments[1];
            var master = typeof(MasterManager)
                .GetMethod(nameof(BuildMaster), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(masterType, keyType, recordType)
                .Invoke(null, [sheets[sheetName]]);
            field.SetValue(Instance, master);
        }
    }

    private static TMaster BuildMaster<TMaster, TKey, TRecord>(GoogleSheet sheet)
        where TMaster : MasterTable<TKey, TRecord>, new()
        where TRecord : MasterRecord<TKey>, new()
        where TKey : notnull
    {
        var records = Enumerable.Range(0, sheet.RowCount - 1).Select(_ => new TRecord()).ToList();
        var keyDictionary = Enumerable.Range(0, sheet.ColumnCount).ToDictionary(i => sheet.Get(0, i), i => i);
        var fields = typeof(TRecord).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var propertyAttributes = field.GetCustomAttributes();
            foreach (var propertyAttribute in propertyAttributes)
            {
                switch (propertyAttribute)
                {
                    case MasterIntValueAttribute integerValueAttribute:
                    {
                        if (keyDictionary.TryGetValue(integerValueAttribute.KeyName, out var column) == false)
                        {
                            LogManager.LogError("Key not found: " + sheet.SheetName + "/" + integerValueAttribute.KeyName);
                            continue;
                        }

                        for (var row = 1; row < sheet.RowCount; row++)
                        {
                            var value = sheet.Get(row, column);
                            if (int.TryParse(value, out var intValue) == false)
                            {
                                LogManager.LogError("Invalid value type (integer): " + sheet.SheetName + "/" + integerValueAttribute.KeyName + $"[{row}] = {value}");
                                continue;
                            }

                            field.SetValue(records[row - 1], intValue);
                        }

                        break;
                    }
                    case MasterFloatValueAttribute floatValueAttribute:
                    {
                        if (keyDictionary.TryGetValue(floatValueAttribute.KeyName, out var column) == false)
                        {
                            LogManager.LogError("Key not found: " + sheet.SheetName + "/" + floatValueAttribute.KeyName);
                            continue;
                        }

                        for (var row = 1; row < sheet.RowCount; row++)
                        {
                            var value = sheet.Get(row, column);
                            if (float.TryParse(value, out var floatValue) == false)
                            {
                                LogManager.LogError("Invalid value type (float): " + sheet.SheetName + "/" + floatValueAttribute.KeyName + $"[{row}] = {value}");
                                continue;
                            }

                            field.SetValue(records[row - 1], floatValue);
                        }

                        break;
                    }
                    case MasterStringValueAttribute stringValueAttribute:
                    {
                        if (keyDictionary.TryGetValue(stringValueAttribute.KeyName, out var column) == false)
                        {
                            LogManager.LogError("Key not found: " + sheet.SheetName + "/" + stringValueAttribute.KeyName);
                            continue;
                        }

                        for (var row = 1; row < sheet.RowCount; row++)
                        {
                            var value = sheet.Get(row, column);
                            field.SetValue(records[row - 1], value);
                        }

                        break;
                    }
                    case MasterBoolValueAttribute booleanValueAttribute:
                    {
                        if (keyDictionary.TryGetValue(booleanValueAttribute.KeyName, out var column) == false)
                        {
                            LogManager.LogError("Key not found: " + sheet.SheetName + "/" + booleanValueAttribute.KeyName);
                            continue;
                        }

                        for (var row = 1; row < sheet.RowCount; row++)
                        {
                            var value = sheet.Get(row, column);
                            if (bool.TryParse(value, out var boolValue) == false)
                            {
                                LogManager.LogError("Invalid value type (bool): " + sheet.SheetName + "/" + booleanValueAttribute.KeyName + $"[{row}] = {value}");
                                continue;
                            }

                            field.SetValue(records[row - 1], boolValue);
                        }

                        break;
                    }
                    case MasterEnumValueAttribute enumValueAttribute:
                    {
                        if (keyDictionary.TryGetValue(enumValueAttribute.KeyName, out var column) == false)
                        {
                            LogManager.LogError("Key not found: " + sheet.SheetName + "/" + enumValueAttribute.KeyName);
                            continue;
                        }

                        for (var row = 1; row < sheet.RowCount; row++)
                        {
                            var value = sheet.Get(row, column);
                            if (Enum.TryParse(enumValueAttribute.EnumType, value, out var enumValue) == false)
                            {
                                LogManager.LogError($"Invalid value type ({enumValueAttribute.EnumType.Name}): " + sheet.SheetName + "/" + enumValueAttribute.KeyName + $"[{row}] = {value}");
                                continue;
                            }

                            field.SetValue(records[row - 1], enumValue);
                        }

                        break;
                    }
                }
            }
        }

        var master = new TMaster();
        master.Set(records);
        return master;
    }
}

public class MasterBoolValueAttribute : Attribute
{
    public MasterBoolValueAttribute(string keyName)
    {
        KeyName = keyName;
    }

    public string KeyName { get; }
}

public class MasterEnumValueAttribute : Attribute
{
    public MasterEnumValueAttribute(string keyName, Type enumType)
    {
        KeyName = keyName;
        EnumType = enumType;
    }

    public string KeyName { get; }
    public Type EnumType { get; }
}

public class MasterFloatValueAttribute : Attribute
{
    public MasterFloatValueAttribute(string keyName)
    {
        KeyName = keyName;
    }

    public string KeyName { get; }
}

public class MasterIntValueAttribute : Attribute
{
    public MasterIntValueAttribute(string keyName)
    {
        KeyName = keyName;
    }

    public string KeyName { get; }
}

public class MasterStringValueAttribute : Attribute
{
    public MasterStringValueAttribute(string keyName)
    {
        KeyName = keyName;
    }

    public string KeyName { get; }
}

public class MasterStringCollectionValueAttribute : Attribute
{
    public MasterStringCollectionValueAttribute(string keyName)
    {
        KeyName = keyName;
    }

    public string KeyName { get; }
}

public class MasterTableAttribute : Attribute
{
    public MasterTableAttribute(string keyName)
    {
        KeyName = keyName;
    }

    public string KeyName { get; }
}
