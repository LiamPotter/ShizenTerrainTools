using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Shizen.Editors
{
    [CustomEditor(typeof(ShizenComponent), true)]
    public class ShizenComponentEditor : Editor
    {
        private ShizenComponent sComponent;

        public override void OnInspectorGUI()
        {
            if (!sComponent)
                sComponent = (ShizenComponent)target;
            DrawDefaultInspector();
            if (GUILayout.Button(sComponent.OpenButtonLabel))
            {
                sComponent.Open();
            }
        }
    }
}
