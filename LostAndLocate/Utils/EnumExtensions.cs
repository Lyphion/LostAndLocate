using System.ComponentModel;

namespace LostAndLocate.Utils;

/// <summary>
/// Extension class for accessing the <see cref="DescriptionAttribute"/>.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Receives the message from the <see cref="DescriptionAttribute"/> of an enum.
    /// </summary>
    /// <param name="e">Value from which to get the description</param>
    /// <typeparam name="T">The Enum type</typeparam>
    /// <returns>The description or the name if no <see cref="DescriptionAttribute"/> was found</returns>
    public static string GetDescription<T>(this T e) where T : Enum
    {
        var type = typeof(T);
        var fieldInfo = type.GetField(e.ToString());

        var attributes = (DescriptionAttribute[]) fieldInfo?
            .GetCustomAttributes(typeof(DescriptionAttribute), false)!;

        return attributes.Length > 0 ? attributes.First().Description : e.ToString();
    }
}