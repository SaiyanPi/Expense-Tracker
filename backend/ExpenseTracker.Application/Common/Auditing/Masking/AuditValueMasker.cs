using System.Text.Json;

namespace ExpenseTracker.Application.Common.Auditing.Masking;

public static class AuditValueMasker
{
    private const string AmountMask = "[CHANGED]";

    public static string? MaskSensitiveJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (dictionary is null)
            return json;

        foreach (var key in dictionary.Keys.ToList())
        {
            if (IsSensitive(key))
            {
                dictionary[key] = AmountMask;
            }
        }

        return JsonSerializer.Serialize(dictionary);
    }

    private static bool IsSensitive(string fieldName)
    {
        return AuditSensitiveFields.Names
            .Any(s => fieldName.Contains(s, StringComparison.OrdinalIgnoreCase));
    }
}