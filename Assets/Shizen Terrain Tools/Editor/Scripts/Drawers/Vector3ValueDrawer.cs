using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Shizen.Base.Properties;

namespace Shizen.Editors.Drawers
{
    public class Vector3ValueDrawer
    {
        public static Vector3 Draw(GUIContent label, Vector3 value, GUISkin skin)
        {
            Vector3 _fieldvalue = value;
            using (new GUILayout.HorizontalScope(skin.FindStyle("FieldValue")))
            {
                EditorGUILayout.LabelField(label, skin.label);
                _fieldvalue.x = FloatValueDrawer.Draw(new GUIContent("x"), value.x, skin, true, 40,false);
                _fieldvalue.y = FloatValueDrawer.Draw(new GUIContent("y"), value.y, skin, true, 40,false);
                _fieldvalue.z = FloatValueDrawer.Draw(new GUIContent("z"), value.z, skin, true, 40,false);
                GUILayout.FlexibleSpace();
            }
            return _fieldvalue;
        }
    }
}
