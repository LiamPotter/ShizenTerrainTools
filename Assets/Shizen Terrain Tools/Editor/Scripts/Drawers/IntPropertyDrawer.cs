using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Shizen.Base.Properties;

namespace Shizen.Editors.Drawers
{

    public class IntPropertyDrawer
    {
        public static void Draw(IntProperty property, GUISkin skin, int minimumValue, int maximumValue)
        {
            using (new GUILayout.HorizontalScope(skin.FindStyle("Property")))
            {
                property.SetRandomized(
                    ToggleButtonDrawer.Draw(
                        new GUIContent(skin.FindStyle("RandomManualIcons").hover.background),
                        new GUIContent(skin.FindStyle("RandomManualIcons").normal.background),
                        "This property is set to manual.",
                        "This property is set to random.",
                        skin.FindStyle("ToggleButton"),
                        property.Randomized));
                if (property.Randomized)
                {
                    EditorGUILayout.LabelField(property.Name, skin.label);
                    property.SetMinMax(
                        IntValueDrawer.Draw(
                            new GUIContent("Min"), property.Min, minimumValue, property.Max, skin, true),
                        IntValueDrawer.Draw(
                            new GUIContent("Max"), property.Max, property.Min, maximumValue, skin, true));
                }
                else
                {
                    property.SetValue(
                       IntValueDrawer.Draw(new GUIContent(property.Name),
                       property.Value, minimumValue, maximumValue, skin, false));
                }
            }
        }

    }
}