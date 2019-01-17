using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Shizen.Base.Properties;

namespace Shizen.Editors.Drawers
{
    public static class FloatSliderDrawer
    {
        public static float Draw(GUIContent label,float value,float minimumValue,float maximumValue,GUISkin skin,bool showValue, bool sliderOnRight,float sliderWidth)
        {
            float _fieldValue = 0;
            using (new GUILayout.HorizontalScope(skin.FindStyle("FieldValue")))
            {              
                EditorGUILayout.LabelField(label, skin.FindStyle("SliderLabel"), 
                    GUILayout.Width(skin.FindStyle("SliderLabel").CalcSize(label).x));
                if (sliderOnRight)
                    GUILayout.FlexibleSpace();
                else
                    GUILayout.Space(20);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(minimumValue.ToString(), skin.FindStyle("SliderValues"));
                    _fieldValue = GUILayout.HorizontalSlider(value, minimumValue, maximumValue, GUILayout.Width(sliderWidth));
                    GUILayout.Label(maximumValue.ToString(), skin.FindStyle("SliderValues"));
                    _fieldValue = (float)System.Math.Round(_fieldValue, 2);
                    if(showValue)
                    {
                        GUIContent valueContent = new GUIContent(_fieldValue.ToString());
                        EditorGUILayout.LabelField(valueContent, skin.FindStyle("SliderLabel"),
                            GUILayout.Width(skin.FindStyle("SliderLabel").CalcSize(valueContent).x));
                    }
                }

                if (!sliderOnRight)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(10);
                }
                else GUILayout.Space(10);
            }
            return _fieldValue;
        }
        public static float Draw(GUIContent label, float value, float minimumValue, float maximumValue, GUISkin skin,bool showValue, bool sliderOnRight)
        {
            return Draw(label, value, minimumValue, maximumValue, skin, showValue, sliderOnRight, 100);
        }
        public static float Draw(GUIContent label, float value, float minimumValue, float maximumValue, GUISkin skin,bool showValue)
        {
            return Draw(label, value, minimumValue, maximumValue, skin, showValue, false,100);
        }
        public static float Draw(GUIContent label, float value, float minimumValue, float maximumValue, GUISkin skin)
        {
            return Draw(label, value, minimumValue, maximumValue, skin,false, false, 100);
        }
    }
}
