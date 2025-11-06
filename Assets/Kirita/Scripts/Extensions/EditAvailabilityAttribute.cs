using System;
using UnityEngine;

namespace MS.Extensions
{
    public enum EditAvailabilityMode
    {
        AlwaysDisabled,
        PlayModeOnly,
        EditModeOnly
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EditAvailabilityAttribute : PropertyAttribute
    {
        public EditAvailabilityMode Mode { get; }

        public EditAvailabilityAttribute(EditAvailabilityMode mode)
        {
            Mode = mode;
        }
    }
}