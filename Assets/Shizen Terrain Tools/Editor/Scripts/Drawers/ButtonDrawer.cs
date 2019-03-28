using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Shizen.Base.Properties;

namespace Shizen.Editors.Drawers
{
    public class ButtonDrawer
    {
        public static bool Draw(GUIContent label, GUIStyle style)
        {
            return GUILayout.Button(label, style);
        }
    }
}