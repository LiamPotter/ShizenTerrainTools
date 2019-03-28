using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Shizen.Base.Properties;

namespace Shizen.Editors.Drawers
{
    public class FloatValueDrawer
    {
        public static float Draw(GUIContent label, float value, GUISkin skin, bool asPrefix, float prefixWidth,bool spaceAfterValue)
        {
            float _fieldvalue = 0;
            using (new GUILayout.HorizontalScope(skin.FindStyle("FieldValue")))
            {
                if (!asPrefix)
                {
                    EditorGUILayout.LabelField(label, skin.label);
                    _fieldvalue = EditorGUILayout.FloatField(" ", value, skin.GetStyle("floatField"), GUILayout.MinWidth(50));
                }
                else
                {
                    float baseWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = prefixWidth;
                    _fieldvalue = EditorGUILayout.FloatField(label, value, skin.GetStyle("floatField"), GUILayout.MinWidth(50));
                    EditorGUIUtility.labelWidth = baseWidth;
                }
                if(spaceAfterValue)
                GUILayout.FlexibleSpace();
            }
            return _fieldvalue;
        }
        public static float Draw(GUIContent label, float value, GUISkin skin, bool asPrefix, float prefixWidth)
        {
            return Draw(label, value, skin, asPrefix, prefixWidth,true);
        }
        public static float Draw(GUIContent label,float value, GUISkin skin, bool asPrefix)
        {
            return Draw(label, value, skin, asPrefix, 55);
        }
        public static float Draw(GUIContent label, float value,float minValue,float maxValue, GUISkin skin,bool asPrefix)
        {
            float _fieldValue = Draw(label, value, skin, asPrefix);
            if (_fieldValue > maxValue)
                _fieldValue = maxValue;
            if (_fieldValue < minValue)
                _fieldValue = minValue;
            return _fieldValue;
        }
    }
}
