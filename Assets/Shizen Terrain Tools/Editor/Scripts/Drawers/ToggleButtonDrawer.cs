using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Shizen.Base.Properties;

namespace Shizen.Editors.Drawers
{
    public class ToggleButtonDrawer
    {
        public static bool Draw(GUIContent offLabel, GUIContent onLabel, string offToolTip, string onToolTip,GUIStyle style, bool value)
        {
            bool _fieldValue = value;
            offLabel.tooltip = offToolTip;
            onLabel.tooltip = onToolTip;
            GUIContent selectedLabel;
            if (_fieldValue)
                selectedLabel = onLabel;
            else selectedLabel = offLabel;

            if (GUILayout.Button(selectedLabel, style))
            {
                _fieldValue = !_fieldValue;
            }

            return _fieldValue;
        }
        public static bool Draw(GUIContent offLabel, GUIContent onLabel, GUIStyle style, bool value)
        {
            return Draw(offLabel, onLabel, null, null, style, value);
        }
        public static bool Draw(GUIContent label,string toolTip, GUIStyle style, bool value)
        {
            return Draw(label, label, toolTip, toolTip, style, value);
        }
        public static bool Draw(GUIContent label, GUIStyle style, bool value)
        {
            return Draw(label, label, null, null, style, value);
        }
    }
}
