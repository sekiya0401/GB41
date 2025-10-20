using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)] 
public class ShowIfEnumAttribute : PropertyAttribute
{
    public string EnumFieldName { get; }
    public int[] EnumValues { get; }

    public ShowIfEnumAttribute(string enumFieldName, params object[] values)
    {
        this.EnumFieldName = enumFieldName;
        this.EnumValues = Array.ConvertAll(values, v => (int)v);
    }
}
