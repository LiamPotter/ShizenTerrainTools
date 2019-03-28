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
        private ShizenTerrainComponent shizenTerrainComponent;
        private static GUIStyle style;
        public override void OnInspectorGUI()
        {
            if (!shizenTerrainComponent)
                shizenTerrainComponent = (ShizenTerrainComponent)target;
            if (style==null)
            {
                style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 12;
                style.padding = new RectOffset(8, 8, 8, 0);
            }
            GUILayout.Label("Shizen Terrain", style);
            shizenTerrainComponent.ShizenTerrain = (ShizenTerrain)EditorGUILayout.ObjectField(shizenTerrainComponent.ShizenTerrain, typeof(ShizenTerrain),false);
            if (GUILayout.Button(shizenTerrainComponent.OpenButtonLabel))
            {
                if (shizenTerrainComponent.Open())
                    ShizenTerrainEditor.Initialize(shizenTerrainComponent.Terrain, shizenTerrainComponent.ShizenTerrain);
            }
            if (GUILayout.Button("Reset Terrain"))
            {
                shizenTerrainComponent.ResetTerrainHeights();
            }
            if (GUILayout.Button("Apply Heights"))
            {
                shizenTerrainComponent.ApplyHeightsToTerrain();
            }
        }
    }
}
