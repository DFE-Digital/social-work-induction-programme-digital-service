using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Sww.Ecf.Frontend.Models;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var name = Enum.GetName(type, enumValue);
        if (name == null)
            return enumValue.ToString();

        var field = type.GetField(name);
        if (field == null)
            return name;

        if (
            Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute
            {
                Name: not null
            } attr
        )
        {
            return attr.Name;
        }

        return name;
    }

    public static string? GetDescription(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var field = type.GetField(enumValue.ToString());
        if (field == null)
            return null;

        var attr = field.GetCustomAttribute<DisplayAttribute>();
        return attr?.Description;
    }
}
