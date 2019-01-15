using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.IO;


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
                        if (GUIButton("Create your first Height Layer"))
                        {
                            ShizenTerrain.Heights.Add(new HeightLayer(0));
                        }
                        return;
                    }
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUIStyle.none, baseSkin.verticalScrollbar, baseSkin.scrollView);
                    for (int i = 0; i < ShizenTerrain.Heights.Count; i++)
                    {
                        ShowHeightLayerVariables(ShizenTerrain.Heights[i]);
                    }
                    if (ShizenTerrain.Heights.Count < 6)
                    {
                        GUILayout.Space(5);
                        if (GUIButton("Create a new Height Layer"))
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
            if (!finalHeightLayerOpen)
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
                //_hLayer.Randomized = GUISliderBoolField(_hLayer.Randomized, "Manual", "Random");
                //GUILayout.Space(25);
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
                        if (_hLayer.LayerProperties.Frequency.Randomized)
                            DisplayHeightMinMaxFloatFields("Frequency", 0.1f, 100, _hLayer.LayerProperties.Frequency);
                        else
                            DisplayHeightPropertyFloat("Frequency", _hLayer.LayerProperties.Frequency);

                        if(_hLayer.LayerProperties.Amplitude.Randomized)
                            DisplayHeightMinMaxFloatFields("Amplitude", 0.1f, 2f, _hLayer.LayerProperties.Amplitude);
                        else
                            DisplayHeightPropertyFloatMinMax("Amplitude", 0.1f, 2f, _hLayer.LayerProperties.Amplitude);

                        if (_hLayer.LayerProperties.Octaves.Randomized)
                            DisplayHeightMinMaxIntFields("Octaves", 1, 8, _hLayer.LayerProperties.Octaves);
                        else
                            DisplayHeightPropertyIntMinMax("Octaves", 1, 8, _hLayer.LayerProperties.Octaves);

                        if(_hLayer.LayerProperties.Lacunarity.Randomized)
                            DisplayHeightMinMaxFloatFields("Lacunarity", 1f, 4f, _hLayer.LayerProperties.Lacunarity);
                        else
                            DisplayHeightPropertyFloatMinMax("Lacunarity", 1, 4, _hLayer.LayerProperties.Lacunarity);

                        if(_hLayer.LayerProperties.Persistance.Randomized)
                            DisplayHeightMinMaxFloatFields("Persistance", 0, 1f, _hLayer.LayerProperties.Persistance);
                        else
                            DisplayHeightPropertyFloatMinMax("Persistance", 0, 1, _hLayer.LayerProperties.Persistance);

                        if(_hLayer.LayerProperties.Offset.Randomized)
                            DisplayHeightMinMaxVectorFields("Offset", -100f, 100f, _hLayer.LayerProperties.Offset);
                        else
                            DisplayHeightPropertyVector3("Offset", _hLayer.LayerProperties.Offset);    
                        
                        using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
                            _hLayer.LayerProperties.Opacity = GUISliderField("Opacity", _hLayer.LayerProperties.Opacity, 0, 1, 100, false);
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
                        if (GUILayout.Button("Regenerate", baseSkin.FindStyle("CenteredButton")))
                        {
                            GenerateHeightTexture(_hLayer);
                            Debug.Log("Regenerating " + _hLayer.Name);
                            lastLayer = _hLayer;
                        }
                    }
                }
                //
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
                    if (GUILayout.Button("Combine Height Layers", baseSkin.FindStyle("CenteredButton")))
                    {
                        ShizenTerrain.FinalHeightmap = MainAlgorithms.CombineHeightLayers(ShizenTerrain.Heights);
                    }
                    if (ShizenTerrain.Heights != null)
                    {
                        for (int i = 0; i < ShizenTerrain.Heights.Count; i++)
                        {
                            ShizenTerrain.Heights[i].LayerProperties.Opacity = GUISliderField(string.Format("{0} Opacity", ShizenTerrain.Heights[i].Name)
                                , ShizenTerrain.Heights[i].LayerProperties.Opacity, 0, 1, 100, true);
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
            if (!heightLayersOpen)
                GUILayout.FlexibleSpace();
        }


        private void GenerateHeightTexture(HeightLayer hLayer)
        {
            hLayer.SavedMap = MainAlgorithms.GeneratedSimplexTexture(Terrain, hLayer.LayerProperties);
        }

        private void GenerateHeightTexture()
        {
            if (lastLayer == null)
                return;
            lastLayer.SavedMap = MainAlgorithms.GeneratedSimplexTexture(Terrain, lastLayer.LayerProperties);
        }

       

        private void DisplayHeightPropertyFloat(string label, HeightLayerProperty heightLayerProperty)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
            {
                DisplayHeightPropertyRandomButton(heightLayerProperty);
                heightLayerProperty.SetFloat(GUIFloatField(label, heightLayerProperty.Float));
                GUILayout.FlexibleSpace();
            }
        }

        private void DisplayHeightPropertyFloatMinMax(string label, float min, float max, HeightLayerProperty heightLayerProperty)
        {

            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
            {
                DisplayHeightPropertyRandomButton(heightLayerProperty);
                heightLayerProperty.SetFloat(GUIMinMaxFloatField(label, heightLayerProperty.Float, min, max));
                GUILayout.FlexibleSpace();
            }
        }

        private void DisplayHeightPropertyIntMinMax(string label, int minValue, int maxValue, HeightLayerProperty heightLayerProperty)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
            {
                DisplayHeightPropertyRandomButton(heightLayerProperty);
                heightLayerProperty.SetInt(GUIMinMaxIntField(label, heightLayerProperty.Int, minValue, maxValue));
                GUILayout.FlexibleSpace();
            }
        }

        private void DisplayHeightPropertyVector3(string label, HeightLayerProperty heightLayerProperty)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
            {
                DisplayHeightPropertyRandomButton(heightLayerProperty);
                heightLayerProperty.SetVector3(GUIVector3Field(label,heightLayerProperty.Vector3));
                GUILayout.FlexibleSpace();
            }
        }
        private void DisplayHeightPropertyRandomButton(HeightLayerProperty heightLayerProperty)
        {
            heightLayerProperty.SetRandomized(GUIToggleButton(
                  new GUIContent(baseSkin.FindStyle("RandomManualIcons").hover.background),
                  new GUIContent(baseSkin.FindStyle("RandomManualIcons").normal.background),
                  "This property is set to manual.",
                  "This property is set to random.",
                  heightLayerProperty.Randomized));
        }
        protected void DisplayHeightMinMaxIntFields(string label, int min, int max, HeightLayerProperty heightLayerProperty)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
            {
                DisplayHeightPropertyRandomButton(heightLayerProperty);
                using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
                {
                    EditorGUILayout.LabelField(label, baseSkin.label);
                    heightLayerProperty.SetMinMaxInts(
                        GUIMinMaxIntFieldPrefix("Min", heightLayerProperty.IntMin,
                            min, heightLayerProperty.IntMax),
                        GUIMinMaxIntFieldPrefix("Max", heightLayerProperty.IntMax,
                            heightLayerProperty.IntMin, max));
                    //heightLayerProperty.IntMin = GUIMinMaxIntFieldPrefix("Min", heightLayerProperty.IntMin, 
                    //    min, heightLayerProperty.IntMax);
                    //heightLayerProperty.IntMax = GUIMinMaxIntFieldPrefix("Max", heightLayerProperty.IntMax, 
                    //    heightLayerProperty.IntMin, max);
                    //GUILayout.FlexibleSpace();
                }
            
            }
        }
        protected void DisplayHeightMinMaxFloatFields(string label, float min, float max, HeightLayerProperty heightLayerProperty)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
            {
                DisplayHeightPropertyRandomButton(heightLayerProperty);
                using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
                {
                    EditorGUILayout.LabelField(label, baseSkin.label);
                    heightLayerProperty.SetMinMaxFloats(
                        GUIMinMaxFloatFieldPrefix("Min", heightLayerProperty.FloatMin,
                            min, heightLayerProperty.FloatMax),
                         GUIMinMaxFloatFieldPrefix("Max", heightLayerProperty.FloatMax,
                            heightLayerProperty.FloatMin, max));
                    //heightLayerProperty.FloatMin = GUIMinMaxFloatFieldPrefix("Min", heightLayerProperty.FloatMin,
                    //  min, heightLayerProperty.FloatMax);
                    //heightLayerProperty.FloatMax = GUIMinMaxFloatFieldPrefix("Max", heightLayerProperty.FloatMax, 
                    //    heightLayerProperty.FloatMin, max);
                    //GUILayout.FlexibleSpace();
                }
            }
        }
        protected void DisplayHeightMinMaxVectorFields(string label, float min, float max, HeightLayerProperty heightLayerProperty)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("HeightLayerProperty")))
            {
                DisplayHeightPropertyRandomButton(heightLayerProperty);
                using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
                {
                    EditorGUILayout.LabelField(label, baseSkin.label);
                    heightLayerProperty.SetMinMaxVector3s(
                        GUIMinMaxFloatFieldPrefix("Min", heightLayerProperty.FloatMin,
                            min, heightLayerProperty.FloatMax),
                         GUIMinMaxFloatFieldPrefix("Max", heightLayerProperty.FloatMax,
                            heightLayerProperty.FloatMin, max));
                    //heightLayerProperty.FloatMin = GUIMinMaxFloatFieldPrefix("Min", heightLayerProperty.FloatMin,
                    //  min, heightLayerProperty.FloatMax);
                    //heightLayerProperty.FloatMax = GUIMinMaxFloatFieldPrefix("Max", heightLayerProperty.FloatMax, 
                    //    heightLayerProperty.FloatMin, max);
                    //GUILayout.FlexibleSpace();
                }
            }
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

        protected bool GUIButton(string label)
        {
            return GUILayout.Button(label, baseSkin.button);
        }
        public bool GUIToggleButton(GUIContent labelOff,GUIContent labelOn,string toolTipOff,string toolTipOn, bool value)
        {
            bool _fieldValue = value;
            labelOff.tooltip = toolTipOff;
            labelOn.tooltip = toolTipOn;
            GUIContent selectedLabel;
            if (_fieldValue)
                selectedLabel = labelOn;
            else selectedLabel = labelOff;
            
            if(GUILayout.Button(selectedLabel, baseSkin.FindStyle("ToggleButton")))
            {
                _fieldValue = !_fieldValue;
            }

            return _fieldValue;
        }
        protected void GUIVector2Field(string label, ref Vector2 value)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                value.x = EditorGUILayout.FloatField("x", value.x, baseSkin.GetStyle("floatField"));
                value.y = EditorGUILayout.FloatField("y", value.y, baseSkin.GetStyle("floatField"));
            }
        }
        protected Vector3 GUIVector3Field(string label, Vector3 value)
        {
            Vector3 _fieldvalue = value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue.x = EditorGUILayout.FloatField("x", value.x, baseSkin.GetStyle("floatField"));
                _fieldvalue.y = EditorGUILayout.FloatField("y", value.y, baseSkin.GetStyle("floatField"));
                _fieldvalue.z = EditorGUILayout.FloatField("z", value.z, baseSkin.GetStyle("floatField"));
            }
            return _fieldvalue;
        }
        protected float GUIFloatField(string label, float value)
        {
            float _fieldvalue = 0;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue = EditorGUILayout.FloatField(value, baseSkin.GetStyle("floatField"));
            }
            return _fieldvalue;
        }

        protected float GUIMinMaxFloatField(string label, float value, float minVal, float maxVal)
        {
            float _fieldvalue = minVal;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue = EditorGUILayout.FloatField(value, baseSkin.GetStyle("floatField"));
               
            }
            if (_fieldvalue > maxVal)
                _fieldvalue = maxVal;
            if (_fieldvalue < minVal)
                _fieldvalue = minVal;
            return _fieldvalue;
        }
        protected float GUIMinMaxFloatFieldPrefix(string label, float value, float minVal, float maxVal)
        {
            float _fieldvalue = minVal;
            EditorGUIUtility.labelWidth = 55;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                _fieldvalue = EditorGUILayout.FloatField(label,value, baseSkin.GetStyle("floatField"));

            }
            EditorGUIUtility.labelWidth = baseLabelWidth;
            if (_fieldvalue > maxVal)
                _fieldvalue = maxVal;
            if (_fieldvalue < minVal)
                _fieldvalue = minVal;
            return _fieldvalue;
        }
        protected int GUIMinMaxIntField(string label, int value, int minVal, int maxVal)
        {
            int _fieldvalue = value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue = EditorGUILayout.IntField(value, baseSkin.GetStyle("floatField"));
            }
            if (_fieldvalue > maxVal)
                _fieldvalue = maxVal;
            if (_fieldvalue < minVal)
                _fieldvalue = minVal;
            return _fieldvalue;
        }
        protected int GUIMinMaxIntFieldPrefix(string label, int value, int minVal, int maxVal)
        {
            int _fieldvalue = value;
            EditorGUIUtility.labelWidth = 55;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                _fieldvalue = EditorGUILayout.IntField(label,value, baseSkin.GetStyle("floatField"));
            }
            EditorGUIUtility.labelWidth = baseLabelWidth;
            if (_fieldvalue > maxVal)
                _fieldvalue = maxVal;
            if (_fieldvalue < minVal)
                _fieldvalue = minVal;
            return _fieldvalue;
        }
        protected float GUISliderField(string label, float value, float fromValue, float toValue, int sliderWidth,bool sliderOnRight)
        {
            float _fieldValue= value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("FieldValue")))
            {
                GUIContent labelContent = new GUIContent(label);
               
                EditorGUILayout.LabelField(labelContent, baseSkin.FindStyle("SliderLabel"),GUILayout.Width(baseSkin.FindStyle("SliderLabel").CalcSize(labelContent).x));
                if (sliderOnRight)
                    GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(fromValue.ToString(), baseSkin.FindStyle("SliderValues"));
                    _fieldValue = GUILayout.HorizontalSlider(value, fromValue, toValue, GUILayout.Width(sliderWidth));
                    GUILayout.Label(toValue.ToString(), baseSkin.FindStyle("SliderValues"));
                    //EditorGUILayout.LabelField( value.ToString(), baseSkin.GetStyle("floatField"));
                    //GUILayout.FlexibleSpace();
                }
                if (!sliderOnRight)
                    GUILayout.FlexibleSpace();
            }
            return _fieldValue;
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
