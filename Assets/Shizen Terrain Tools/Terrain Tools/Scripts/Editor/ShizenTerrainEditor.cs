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

        #region Toolbar
        private static int toolbarTab = 0;

        private static string[] toolbarNames = new string[] { "Height Tools","Object Tools", "Texture Tools", "Detail Tools" };

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
            _tempEditor.minSize = new Vector2(725,600);
            bgColor = new Color(0.9f, 0.9f, 0.9f);
            _tempEditor.Show();
            _tempEditor = null;
        }
        void OnEnable()
        {
            heightLayersOpen = EditorPrefs.GetBool("ShizenHeightLayersOpen");
        }

        void OnDisable()
        {
            EditorPrefs.SetBool("ShizenHeightLayersOpen", heightLayersOpen);
        }

        private void OnDestroy()
        {
         
        }

        void OnGUI()
        {
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 40;
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
                    if (GUIButton("Create a new Height Layer"))
                    {
                        ShizenTerrain.Heights.Add(new HeightLayer(ShizenTerrain.Heights.Count));
                    }
                }
                EditorGUILayout.EndScrollView();
          
            }

            ShowHeightLayerCombinations();
        }

        protected void ShowHeightLayerVariables(HeightLayer _hLayer)
        {
            //SerializedProperty _serializedLayer = SerializedTerrain.FindProperty("Heights").GetArrayElementAtIndex(_hLayer.Position);
            //for (int i = 0; i < ShizenTerrain.Heights.Count; i++)
            //{
            //    if(_serializedLayer.get)
            //    _serializedLayer.Next(true);
            //}
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Layers")))
            {
                using (new GUILayout.VerticalScope())
                {
                    //_hLayer.Name = GUITextField("Layer Name:", _hLayer.Name);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIContent selectedContent;
                        if (_hLayer.expandedInEditor)
                            selectedContent = new GUIContent(baseSkin.FindStyle("OpenCloseSymbolsLight").hover.background);
                        else selectedContent = new GUIContent(baseSkin.FindStyle("OpenCloseSymbolsLight").normal.background);
                        if (GUILayout.Button(selectedContent,baseSkin.FindStyle("SymbolButton")))
                            _hLayer.expandedInEditor = !_hLayer.expandedInEditor;
                        _hLayer.Name =
                            EditorGUILayout.TextField(_hLayer.Name, baseSkin.textField);
                    }
                    if (_hLayer.expandedInEditor)
                    {
                        EditorGUI.indentLevel = 1;
                        _hLayer.LayerProperties.Frequency = GUIFloatField("Frequency", _hLayer.LayerProperties.Frequency);
                        _hLayer.LayerProperties.Amplitude = GUIMinMaxFloatField("Amplitude", _hLayer.LayerProperties.Amplitude, 0.1f, 2f);
                        _hLayer.LayerProperties.Octaves = GUIMinMaxIntField("Octaves", _hLayer.LayerProperties.Octaves, 1, 8);
                        _hLayer.LayerProperties.Lacunarity = GUIMinMaxFloatField("Lacunarity", _hLayer.LayerProperties.Lacunarity, 1, 4);
                        _hLayer.LayerProperties.Persistance = GUIMinMaxFloatField("Persistance", _hLayer.LayerProperties.Persistance, 0f, 1f);
                        _hLayer.LayerProperties.Offset = GUIVector3Field("Offset", _hLayer.LayerProperties.Offset);
                        _hLayer.LayerProperties.Opacity = GUISliderField("Opacity",_hLayer.LayerProperties.Opacity, 0, 1);
                        EditorGUI.indentLevel = 0;
                       
                    }
                }
                //GUILayout.FlexibleSpace();
                if (_hLayer.expandedInEditor)
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
                            //Debug.Log("The saved map of " + _hLayer.Name+"'s dimensions are: "+_hLayer.SavedMap.width+"x "+_hLayer.SavedMap.height+"y");
                            EditorGUI.DrawPreviewTexture(new Rect(EditorGUILayout.GetControlRect().position + new Vector2(6, -200), new Vector2(200, 200)), _hLayer.SavedMap);
                            //EditorGUILayout.PropertyField(_serializedLayer.FindPropertyRelative("SavedMap"));

                        }
                        GUILayout.Space(10);
                        if (GUILayout.Button("Regenerate"))
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
            EditorGUILayout.LabelField("Layer Combinations", baseSkin.FindStyle("Heading"));
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Layers")))
            {
                using (new GUILayout.VerticalScope())
                {
                    for (int i = 0; i < ShizenTerrain.Heights.Count; i++)
                    {
                        EditorGUILayout.Separator();
                    }
                }
            }
            GUILayout.Space(5);
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

        protected int BoolToInt(bool _value)
        {
            if (_value)
                return 1;
            else
                return 0;
        }

        protected string GUITextField(string label, string text)
        {
            //var _textWidth = baseSkin.label.CalcSize(new GUIContent(_label));
            //Debug.Log("Text width for " + _label + " is " + _textWidth);
            var _value = "";
            //using (new EditorGUILayout.P())
            GUIContent _useLabel = new GUIContent(label);

            _value = EditorGUILayout.TextField(_useLabel, text, baseSkin.textField);

            return _value;
        }

        protected bool GUIButton(string label)
        {
            return GUILayout.Button(label, baseSkin.button);
        }

        protected void GUIVector2Field(string label, ref Vector2 value)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                value.x = EditorGUILayout.FloatField("x", value.x, baseSkin.GetStyle("floatField"));
                value.y = EditorGUILayout.FloatField("y", value.y, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
        }
        protected Vector3 GUIVector3Field(string label, Vector3 value)
        {
            Vector3 _fieldvalue = value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue.x = EditorGUILayout.FloatField("x", value.x, baseSkin.GetStyle("floatField"));
                _fieldvalue.y = EditorGUILayout.FloatField("y", value.y, baseSkin.GetStyle("floatField"));
                _fieldvalue.y = EditorGUILayout.FloatField("z", value.z, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            return _fieldvalue;
        }
        protected float GUIFloatField(string label, float value)
        {
            float _fieldvalue = 0;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue = EditorGUILayout.FloatField(" ", value, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            return _fieldvalue;
        }

        protected float GUIMinMaxFloatField(string label, float value, float minVal, float maxVal)
        {
            float _fieldvalue = minVal;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue = EditorGUILayout.FloatField(" ", value, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            if (_fieldvalue > maxVal)
                _fieldvalue = maxVal;
            if (_fieldvalue < minVal)
                _fieldvalue = minVal;
            return _fieldvalue;
        }
        protected int GUIMinMaxIntField(string label, int value, int minVal, int maxVal)
        {
            int _fieldvalue = value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                _fieldvalue = EditorGUILayout.IntField(" ", value, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            if (_fieldvalue > maxVal)
                _fieldvalue = maxVal;
            if (_fieldvalue < minVal)
                _fieldvalue = minVal;
            return _fieldvalue;
        }
        protected float GUISliderField(string label, float value, float fromValue, float toValue)
        {
            float _fieldValue= value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(label, baseSkin.label);
                //GUILayout.Space(5);
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(fromValue.ToString(), baseSkin.FindStyle("SliderValues"));
                    _fieldValue = GUILayout.HorizontalSlider(value, fromValue, toValue, GUILayout.MinWidth(100));
                    EditorGUILayout.PrefixLabel(toValue.ToString(), baseSkin.FindStyle("SliderValues"));
                    //_fieldValue = EditorGUILayout.LabelField( value, baseSkin.GetStyle("floatField"));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
            }
            return _fieldValue;
        }
    }
    
}
