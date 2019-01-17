using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Shizen.Base.Properties;

namespace Shizen.Editors.Drawers
{
    public class IntValueDrawer 
    {
        public static int Draw(GUIContent label, int value, GUISkin skin, bool asPrefix, float prefixWidth, bool spaceAfterValue)
        {
            int _fieldvalue = 0;
            using (new GUILayout.HorizontalScope(skin.FindStyle("FieldValue")))
            {
                if (!asPrefix)
                {
                    EditorGUILayout.LabelField(label, skin.label);
                    _fieldvalue = EditorGUILayout.IntField(" ", value, skin.GetStyle("floatField"), GUILayout.MinWidth(50));
                }
                else
                {
                    float baseWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = prefixWidth;
                    _fieldvalue = EditorGUILayout.IntField(label, value, skin.GetStyle("floatField"), GUILayout.MinWidth(50));
                    EditorGUIUtility.labelWidth = baseWidth;
                }
                if(spaceAfterValue)
                    GUILayout.FlexibleSpace();
            }
            return _fieldvalue;
        }
        public static int Draw(GUIContent label, int value, GUISkin skin, bool asPrefix, float prefixWidth)
        {
            return Draw(label, value, skin, asPrefix, prefixWidth,true);
        }
        public static int Draw(GUIContent label, int value, GUISkin skin, bool asPrefix)
        {
            return Draw(label, value, skin, asPrefix, 55);
        }
        public static int Draw(GUIContent label, int value, int minValue, int maxValue, GUISkin skin, bool asPrefix)
        {
            int _fieldValue = Draw(label, value, skin, asPrefix);
            if (_fieldValue > maxValue)
                _fieldValue = maxValue;
            if (_fieldValue < minValue)
                _fieldValue = minValue;
            return _fieldValue;
        }
    }
}
