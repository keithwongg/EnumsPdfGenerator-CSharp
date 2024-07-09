using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnumsPdfGenerator.ExtensionMethods;

public static class MyExtensions
{
    public static void OpenDivContainer(this StringBuilder sb, Type e)
    {
        sb.Append($"<div id=\"{e.Name.Trim().ToLower()}\" style=\"border-style: solid; padding: 8px; margin: 4px;\">");
        //sb.Append($"<h1>{e.Namespace}</h1>");
        sb.Append($"<h3>{e.Name}</h3>");
    }

    public static void AddEachEnumValue(this StringBuilder sb, Type e)
    {
        var enumInstance = Activator.CreateInstance(e);
        if ( enumInstance == null )
        {
            throw new Exception("Enum cannot be created or initialized.");
        }
        Type enumType = enumInstance.GetType();
        var values = enumType.GetEnumValues();
        foreach (var v in values)
        {

            string displayName = v.GetType().GetMember(v.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.Name ?? " - ";

            int intValue = (int)v;
            sb.Append($"<p>{v} ({displayName}) = {intValue} </p>");
        }
    }

    public static void CloseDivContainer(this StringBuilder sb)
    {
        sb.Append("</div>");
    }


    //This is a extension class of enum
    public static string GetEnumDisplayName(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
                       .First()
                       .GetCustomAttribute<DisplayAttribute>()
                       .Name;
    }
}
