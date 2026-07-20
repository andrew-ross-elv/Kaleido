using System.ComponentModel;
using System.Reflection;

namespace Kaleido.Extensions;

public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
    {
        if (value == null) return string.Empty;

        var member = value.GetType().GetMember(value.ToString());

        if (member.Length > 0)
        {
            var attribute = member[0]
                .GetCustomAttribute<DescriptionAttribute>();

            if (attribute != null)
            {
                return attribute.Description;
            }
        }

        return value.ToString().ToLowerInvariant();
    }

    public static bool TryParseFromDescription<TEnum>(
        string? value,
        out TEnum result)
        where TEnum : struct, Enum
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.Trim();

        foreach (TEnum item in Enum.GetValues(typeof(TEnum)))
        {
            var enumValue = item as Enum;

            if (enumValue == null)
            {
                continue;
            }

            if (string.Equals(
                    enumValue.GetDescription(),
                    text,
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    enumValue.ToString(),
                    text,
                    StringComparison.OrdinalIgnoreCase))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    public static bool TryParseFromDescription(
        Type enumType,
        string? value,
        out object? result)
    {
        result = null;

        if (!enumType.IsEnum)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.Trim();

        foreach (var item in Enum.GetValues(enumType))
        {
            if (item is not Enum enumValue)
            {
                continue;
            }

            if (string.Equals(
                    enumValue.GetDescription(),
                    text,
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    enumValue.ToString(),
                    text,
                    StringComparison.OrdinalIgnoreCase))
            {
                result = item;
                return true;
            }
        }

        return false;
    }
}