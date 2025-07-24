using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Devsu.Application.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Get the name of enum in a text based in the [Display] Attribute
    /// </summary>
    /// <param name="enumValue">Enum</param>
    /// <returns>string</returns>
    public static string GetDisplay(this Enum enumValue)
        => enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? enumValue.ToString();

}