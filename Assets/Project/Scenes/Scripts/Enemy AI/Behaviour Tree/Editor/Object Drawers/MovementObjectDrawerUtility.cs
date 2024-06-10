using System.Reflection;
using UnityEngine;

public static class MovementObjectDrawerUtility
{
    public struct FieldContent
    {
        public FieldInfo Field;
        public string Name;
        public string Tooltip;

        public readonly GUIContent GetGUIContent() => new(Name, Tooltip);
    }

    public static string GetTooltip(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName);

        if (field == null)
            return string.Empty;

        var toolTip = field.GetCustomAttribute<BehaviorDesigner.Runtime.Tasks.TooltipAttribute>();
        if (toolTip != null)
            return toolTip.mTooltip;

        return string.Empty;
    }
}