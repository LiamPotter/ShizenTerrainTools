using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Shizen.Editors
{
    /// <summary>
    /// Custome Editor for the ShizenTerrainComponent
    /// </summary>
    [CustomEditor(typeof(ShizenTerrainComponent))]
    public class ShizenTerrainCompEditor : Editor
    {
        private ShizenTerrainComponent sComponent;
        private static GUIStyle style;
        public override void OnInspectorGUI()
        {
            if (!sComponent)
                sComponent = (ShizenTerrainComponent)target;
            if (style==null)
            {
                style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 12;
                style.padding = new RectOffset(8, 8, 8, 0);
            }
            GUILayout.Label("Shizen Terrain", style);
            sComponent.ShizenTerrain = (ShizenTerrain)EditorGUILayout.ObjectField(sComponent.ShizenTerrain, typeof(ShizenTerrain),false);
            if (GUILayout.Button(sComponent.OpenButtonLabel))
            {
                if (sComponent.Open())
                    ShizenTerrainEditor.Initialize(sComponent.Terrain, sComponent.ShizenTerrain);
            }
        }
    }
}
