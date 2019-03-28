using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.IO;
using Shizen.Editors.Drawers;

namespace Shizen.Editors
{
    /// <summary>
    /// The Main Editor GUI
    /// </summary>
    public class ShizenTerrainEditor : EditorWindow
    {
        public Terrain Terrain;

        public ShizenTerrain ShizenTerrain;

        public SerializedObject SerializedTerrain;

        public static bool IsOpen;

        private static GUISkin baseSkin;

        private static GUISkin unityBaseSkin;

        private static Color bgColor;

        private Vector2 scrollPosition;

        private static HeightLayer lastLayer;

        private static bool heightLayersOpen;

        private static bool finalHeightLayerOpen;

        private static int baseLabelWidth = 40;

        #region Toolbar
        private static int toolbarTab = 0;

        private static string[] toolbarNames = new string[] { "Height Tools", "Object Tools", "Texture Tools", "Detail Tools" };

        private float selectedToolbarWidth;

        private float selectedToolbarHeight = 40;

        private float selectedPosition;
        #endregion

        public static void Initialize(Terrain _terrain, ShizenTerrain _shizenTerrain)
        {
            if (ShizenTerrainEditor.IsOpen)
                return;
            ShizenTerrainEditor _tempEditor;
            //_tempEditor = (ShizenTerrainEditor)GetWindow(typeof(ShizenTerrainEditor));
            _tempEditor = (ShizenTerrainEditor)ScriptableObject.CreateInstance("ShizenTerrainEditor");
            _tempEditor.Terrain = _terrain;
            _tempEditor.ShizenTerrain = _shizenTerrain;
            _tempEditor.SerializedTerrain = new SerializedObject(_shizenTerrain);
            GUIContent titleContent = new GUIContent("Shizen Terrain Editor");
            _tempEditor.titleContent = titleContent;
            _tempEditor.minSize = new Vector2(725, 600);
            bgColor = new Color(0.9f, 0.9f, 0.9f);
            _tempEditor.Show();
            _tempEditor = null;
            ShizenTerrainEditor.IsOpen = true;
        }
        void OnEnable()
        {
            heightLayersOpen = EditorPrefs.GetBool("ShizenHeightLayersOpen");
            finalHeightLayerOpen = EditorPrefs.GetBool("ShizenFinalHeightOpen");
        }

        void OnDisable()
        {
            EditorPrefs.SetBool("ShizenHeightLayersOpen", heightLayersOpen);
            EditorPrefs.SetBool("ShizenFinalHeightOpen", finalHeightLayerOpen);
            ShizenTerrainEditor.IsOpen = false;
        }

        private void OnDestroy()
        {

        }

        void OnGUI()
        {
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = baseLabelWidth;
            StyleSetUp();
            EditorGUI.DrawRect(new Rect(new Vector2(selectedPosition, 0), new Vector2(selectedToolbarWidth, selectedToolbarHeight + 5)), bgColor);
            toolbarTab = GUILayout.Toolbar(toolbarTab, toolbarNames, baseSkin.FindStyle("Toolbar"));
            EditorGUI.DrawRect(new Rect(new Vector2(0, selectedToolbarHeight), new Vector2(position.width, position.height - selectedToolbarHeight)), bgColor);
            switch (toolbarTab)
            {
                case 0:
                    ShowHeightTools();
                    break;
                case 1:
                    ShowObjectTools();
                    break;
                case 2:
                    ShowTextureTools();
                    break;
                case 3:
                    ShowDetailTools();
                    break;
                default:
                    break;
            }
            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUIUtility.wideMode = false;
            StyleReset();
        }

        protected void StyleSetUp()
        {
            GUI.skin = baseSkin;
            if (bgColor.b != (0.9f))
            {
                bgColor = new Color(0.9f, 0.9f, 0.9f);
            }
            if (baseSkin == null)
            {
                baseSkin = Resources.Load<GUISkin>("UI/ShizenGUI");
                //GUI.skin = baseSkin;
            }
            EditorStyles.label.font = baseSkin.label.font;
            //EditorStyles.label.alignment = baseSkin.label.alignment;
            EditorStyles.label.fixedHeight = baseSkin.label.fixedHeight;
        }

        protected void StyleReset()
        {
            EditorStyles.label.font = EditorStyles.textField.font;
            EditorStyles.label.fixedHeight = EditorStyles.textField.fixedHeight;
            EditorStyles.label.alignment = EditorStyles.textArea.alignment;
            GUI.skin = null;
        }

        protected void ShowHeightTools()
        {
            selectedToolbarWidth = position.width / 4;
            selectedPosition = 0;
            GUILayout.Space(5);
            EditorGUIUtility.SetIconSize(Vector2.one * 20);
            GUIContent heightContent;
            GUILayout.Space(10);
            int amountofHeightLayersOpen = 0;
            using (new GUILayout.VerticalScope(baseSkin.FindStyle("MinimizableGroupBG")))
            {
                if (heightLayersOpen)
                {
                    heightContent = new GUIContent("Height Layers", baseSkin.FindStyle("OpenCloseSymbolsDark").hover.background);
                }
                else
                    heightContent = new GUIContent("Height Layers", baseSkin.FindStyle("OpenCloseSymbolsDark").normal.background);
                if (GUILayout.Button(heightContent, baseSkin.FindStyle("Heading")))
                {
                    heightLayersOpen = !heightLayersOpen;
                    EditorPrefs.SetBool("ShizenHeightLayersOpen", heightLayersOpen);
                }
                if (heightLayersOpen)
                {
                    EditorGUIUtility.SetIconSize(Vector2.zero);
                    if (ShizenTerrain.Heights == null)
                        ShizenTerrain.Heights = new List<HeightLayer>();
                    if (ShizenTerrain.Heights.Count == 0)
                    {
                        if (ButtonDrawer.Draw(new GUIContent("Create your first Height Layer"),baseSkin.button))
                        {
                            ShizenTerrain.Heights.Add(new HeightLayer(0));
                        }
                        return;
                    }
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUIStyle.none, baseSkin.verticalScrollbar, baseSkin.scrollView);
                 
                    for (int i = 0; i < ShizenTerrain.Heights.Count; i++)
                    {
                        ShowHeightLayerVariables(ShizenTerrain.Heights[i]);
                        if (ShizenTerrain.Heights[i].ExpandedInEditor)
                            amountofHeightLayersOpen++;
                    }
                    if (ShizenTerrain.Heights.Count < 6)
                    {
                        GUILayout.Space(5);
                        if (ButtonDrawer.Draw(new GUIContent("Create a new Height Layer", 
                            "Create a new Height Layer, you can have up to six"), baseSkin.button))
                        {
                            ShizenTerrain.Heights.Add(new HeightLayer(ShizenTerrain.Heights.Count));
                        }                     
                    }
                    EditorGUILayout.EndScrollView();
                    
                }
            }
            using (new GUILayout.VerticalScope(baseSkin.FindStyle("MinimizableGroupBG")))
            {
                GUIContent finalHeightContent;
                if (finalHeightLayerOpen)
                {
                    finalHeightContent = new GUIContent("Final Height Map", baseSkin.FindStyle("OpenCloseSymbolsDark").hover.background);
                }
                else
                    finalHeightContent = new GUIContent("Final Height Map", baseSkin.FindStyle("OpenCloseSymbolsDark").normal.background);
                if (GUILayout.Button(finalHeightContent, baseSkin.FindStyle("Heading")))
                {
                    finalHeightLayerOpen = !finalHeightLayerOpen;
                    EditorPrefs.SetBool("ShizenFinalHeightOpen", finalHeightLayerOpen);
                }
                if (finalHeightLayerOpen)
                    ShowHeightLayerCombinations();
            }
            if (heightLayersOpen&&amountofHeightLayersOpen <= 1)
                GUILayout.FlexibleSpace();
        }

        protected void ShowObjectTools()
        {
            selectedPosition = (position.width / 4);
        }
        protected void ShowTextureTools()
        {
            selectedPosition = (position.width / 4) * 2;
        }
        protected void ShowDetailTools()
        {
            selectedPosition = (position.width / 4) * 3;
        }

        protected void ShowHeightLayerVariables(HeightLayer _hLayer)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("LayerToolbar")))
            {
                GUIContent selectedContent;
                if (_hLayer.ExpandedInEditor)
                    selectedContent = new GUIContent(baseSkin.FindStyle("OpenCloseSymbolsLight").hover.background);
                else selectedContent = new GUIContent(baseSkin.FindStyle("OpenCloseSymbolsLight").normal.background);
                if (GUILayout.Button(selectedContent, baseSkin.FindStyle("SymbolButton")))
                    _hLayer.ExpandedInEditor = !_hLayer.ExpandedInEditor;
                _hLayer.Name =
                    EditorGUILayout.TextField(_hLayer.Name, baseSkin.textField);
                EditorGUILayout.PrefixLabel("Preset:", baseSkin.FindStyle("Enum"), baseSkin.FindStyle("EnumLabel"));
                _hLayer.PlaceholderPreset = (HeightLayer.placeholderPresets)EditorGUILayout.EnumPopup(_hLayer.PlaceholderPreset, baseSkin.FindStyle("Enum"));
                GUILayout.FlexibleSpace();
            }
            GUIStyle layerStyle;
            if (_hLayer.ExpandedInEditor)
                layerStyle = baseSkin.FindStyle("LayerValues");
            else layerStyle = baseSkin.FindStyle("LayerClosed");
            using (new GUILayout.HorizontalScope(layerStyle))
            {
                using (new GUILayout.VerticalScope())
                {
                    if (_hLayer.ExpandedInEditor)
                    {
                        EditorGUI.indentLevel = 1;
                        FloatPropertyDrawer.Draw(_hLayer.LayerProperties.Frequency,baseSkin,0.01f,100);
                        FloatPropertyDrawer.Draw(_hLayer.LayerProperties.Amplitude, baseSkin, 0.1f, 2f);
                        IntPropertyDrawer.Draw(_hLayer.LayerProperties.Octaves, baseSkin, 1, 8);
                        FloatPropertyDrawer.Draw(_hLayer.LayerProperties.Lacunarity, baseSkin, 1, 4);
                        FloatPropertyDrawer.Draw(_hLayer.LayerProperties.Persistance, baseSkin, 0, 1);
                        Vector3PropertyDrawer.Draw(_hLayer.LayerProperties.Offset, baseSkin, -100, 100);
                        using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Property")))
                            _hLayer.LayerProperties.Opacity =
                            FloatSliderDrawer.Draw(new GUIContent("Opacity"), _hLayer.LayerProperties.Opacity, 0, 1, baseSkin,true,false,200);                    
                        EditorGUI.indentLevel = 0;
                        GUILayout.Space(5);
                    }
                }
                if (_hLayer.ExpandedInEditor)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        using (new GUILayout.VerticalScope(baseSkin.FindStyle("Image"), GUILayout.Width(220)))
                        {
                            if (_hLayer.SavedMap == null)
                            {
                                GenerateHeightTexture(_hLayer);
                            }

                            EditorGUILayout.LabelField("Layer Map", baseSkin.FindStyle("labelCenter"));
                            GUILayout.Space(210);
                            EditorGUI.DrawPreviewTexture(new Rect(EditorGUILayout.GetControlRect().position + new Vector2(6, -200)
                                , new Vector2(200, 200)), _hLayer.SavedMap);
                        }
                        if(ButtonDrawer.Draw(new GUIContent("Regenerate",
                            "Regenerate this Layer's Map. WARNING: This will override the previous map if you are using randomized values."), baseSkin.FindStyle("CenteredButton")))
                        {
                            GenerateHeightTexture(_hLayer);
                            Debug.Log("Regenerating " + _hLayer.Name);
                            lastLayer = _hLayer;
                        }                       
                    }
                }
                
            }
        }

        protected void ShowHeightLayerCombinations()
        {
            if (ShizenTerrain.Heights == null)
                return;
            if (ShizenTerrain.Heights.Count == 0)
                return;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FinalHeightMap")))
            {
                using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                {
                    if (ButtonDrawer.Draw(new GUIContent("Combine Height Layers",
                           "Combine all Height Layer Maps into one final texture."), baseSkin.FindStyle("CenteredButton")))
                    {
                        ShizenTerrain.FinalHeightmap = MainAlgorithms.CombineHeightLayers(Terrain, ShizenTerrain.Heights);
                    }

                    if (ShizenTerrain.Heights != null)
                    {
                        for (int i = 0; i < ShizenTerrain.Heights.Count; i++)
                        {
                           
                            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Property")))
                                ShizenTerrain.Heights[i].LayerProperties.Opacity =
                                FloatSliderDrawer.Draw(new GUIContent(string.Format("{0} Opacity", ShizenTerrain.Heights[i].Name)), 
                                ShizenTerrain.Heights[i].LayerProperties.Opacity, 
                                0, 1, baseSkin, false, false, 100);
                        }
                    }
                }
                using (new GUILayout.VerticalScope(baseSkin.FindStyle("CenteredImage"), GUILayout.Width(325)))
                {
                    if (ShizenTerrain.FinalHeightmap != null)
                    {
                        GUILayout.Space(300);
                        EditorGUI.DrawPreviewTexture(new Rect(EditorGUILayout.GetControlRect().position + new Vector2(10, -290), new Vector2(300, 300)), ShizenTerrain.FinalHeightmap);
                    }
                }
            }
            //if (!heightLayersOpen)
            //    GUILayout.FlexibleSpace();
        }

        private void GenerateHeightTexture(HeightLayer hLayer)
        {
            hLayer.LayerProperties.DoRandomValueGeneration();
            hLayer.SavedMap = MainAlgorithms.GeneratedSimplexTexture(Terrain, hLayer.LayerProperties);
        }           
   
        protected int BoolToInt(bool _value)
        {
            if (_value)
                return 1;
            else
                return 0;
        }

        #region UI Layout Methods
        protected string GUITextField(string label, string text)
        {
            var _value = "";
            GUIContent _useLabel = new GUIContent(label);
            _value = EditorGUILayout.TextField(_useLabel, text, baseSkin.textField);
            return _value;
        }
                  
        protected bool GUISliderBoolField(bool value, string offLabel, string onLabel)
        {
            bool _fieldValue = value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("ToggleSliderBase")))
            {
                if (!_fieldValue)
                    EditorGUILayout.PrefixLabel(offLabel, baseSkin.FindStyle("ToggleSliderLabelsBold"), baseSkin.FindStyle("ToggleSliderLabelsBold"));
                else
                    EditorGUILayout.PrefixLabel(offLabel, baseSkin.FindStyle("ToggleSliderLabels"), baseSkin.FindStyle("ToggleSliderLabels"));
                GUILayout.Space(-20);
                if(GUILayout.Button("", baseSkin.FindStyle("ToggleSliderButton")))
                {
                    _fieldValue = !_fieldValue;
                }
                if (_fieldValue)
                    EditorGUILayout.PrefixLabel(onLabel, baseSkin.FindStyle("ToggleSliderLabelsBold"), baseSkin.FindStyle("ToggleSliderLabelsBold"));
                else
                    EditorGUILayout.PrefixLabel(onLabel, baseSkin.FindStyle("ToggleSliderLabels"), baseSkin.FindStyle("ToggleSliderLabels"));
                EditorGUILayout.LabelField("");
            }
            return _fieldValue;
        }
        #endregion
    }
    
}
