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

        private static string[] toolbarNames = new string[] { "Height Tools", "Texture Tools", "Detail Tools" };

        private float _selectedToolbarWidth;

        private float _selectedToolbarHeight = 40;

        private float _selectedPosition;

        #endregion

        public static void Initialize(Terrain _terrain, ShizenTerrain _shizenTerrain)
        {
            ShizenTerrainEditor _tempEditor = (ShizenTerrainEditor)GetWindow(typeof(ShizenTerrainEditor));
            _tempEditor.Terrain = _terrain;
            _tempEditor.ShizenTerrain = _shizenTerrain;
            _tempEditor.SerializedTerrain = new SerializedObject(_shizenTerrain);
            GUIContent titleContent = new GUIContent("Shizen Terrain Editor");
            _tempEditor.titleContent = titleContent;
            _tempEditor.minSize = new Vector2(625, 400);
            bgColor = new Color(0.9f, 0.9f, 0.9f);
            _tempEditor.Show();
        }
        void OnEnable()
        {
            //Undo.undoRedoPerformed += GenerateHeightTexture;
            heightLayersOpen = EditorPrefs.GetBool("ShizenHeightLayersOpen");
        }
        void OnDisable()
        {
            EditorPrefs.SetBool("ShizenHeightLayersOpen", heightLayersOpen);
            //Undo.undoRedoPerformed -= GenerateHeightTexture;
        }
        void OnGUI()
        {
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 40;
            StyleSetUp();
            EditorGUI.DrawRect(new Rect(new Vector2(_selectedPosition, 0), new Vector2(_selectedToolbarWidth, _selectedToolbarHeight + 5)), bgColor);
            toolbarTab = GUILayout.Toolbar(toolbarTab, toolbarNames, baseSkin.FindStyle("Toolbar"));
            EditorGUI.DrawRect(new Rect(new Vector2(0, _selectedToolbarHeight), new Vector2(position.width, position.height - _selectedToolbarHeight)), bgColor);
            switch (toolbarTab)
            {
                case 0:
                    ShowHeightTools();
                    break;
                case 1:
                    ShowTextureTools();
                    break;
                case 2:
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
            _selectedToolbarWidth = position.width / 3;
            _selectedPosition = 0;
            EditorGUILayout.Space();
            EditorGUIUtility.SetIconSize(Vector2.one * 10);
            GUIContent heightContent;
            if (heightLayersOpen)
            {
                heightContent = new GUIContent("Height Layers", baseSkin.FindStyle("UpDownArrows").hover.background);
            }
            else
                heightContent = new GUIContent("Height Layers", baseSkin.FindStyle("UpDownArrows").normal.background);
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
                    _hLayer.Name =
                        EditorGUILayout.TextField(_hLayer.Name, baseSkin.textField);
                    EditorGUI.indentLevel = 1;
                    _hLayer.LayerProperties.Frequency = GUIFloatField("Frequency", _hLayer.LayerProperties.Frequency);
                    _hLayer.LayerProperties.Amplitude = GUIMinMaxFloatField("Amplitude", _hLayer.LayerProperties.Amplitude, 0.1f, 2f);
                    _hLayer.LayerProperties.Octaves = GUIMinMaxIntField("Octaves", _hLayer.LayerProperties.Octaves,1,8);
                    _hLayer.LayerProperties.Lacunarity = GUIMinMaxFloatField("Lacunarity", _hLayer.LayerProperties.Lacunarity, 1, 4);
                    _hLayer.LayerProperties.Persistance = GUIMinMaxFloatField("Persistance", _hLayer.LayerProperties.Persistance, 0f, 1f);
                    _hLayer.LayerProperties.Offset = GUIVector3Field("Offset", _hLayer.LayerProperties.Offset);
                    EditorGUI.indentLevel = 0;
                    if (GUILayout.Button("Regenerate"))
                    {
                        GenerateHeightTexture(_hLayer);
                        Debug.Log("Regenerating " + _hLayer.Name);
                        lastLayer = _hLayer;
                    }
                }
                //GUILayout.FlexibleSpace();
                using (new GUILayout.VerticalScope(baseSkin.FindStyle("Image"), GUILayout.Width(240)))
                {
                    if (_hLayer.SavedMap == null)
                    {
                        GenerateHeightTexture(_hLayer);
                    }
                    EditorGUILayout.LabelField("Layer Map", baseSkin.FindStyle("labelCenter"));
                    GUILayout.Space(240);
                    //Debug.Log("The saved map of " + _hLayer.Name+"'s dimensions are: "+_hLayer.SavedMap.width+"x "+_hLayer.SavedMap.height+"y");
                    EditorGUI.DrawPreviewTexture(new Rect(EditorGUILayout.GetControlRect().position + new Vector2(5, -220), new Vector2(220, 220)), _hLayer.SavedMap);
                    //EditorGUILayout.PropertyField(_serializedLayer.FindPropertyRelative("SavedMap"));

                }
                //
            }
        }

        protected void ShowTextureTools()
        {
            _selectedPosition = position.width / 3;
        }
        protected void ShowDetailTools()
        {
            _selectedPosition = (position.width / 3) + (position.width / 3);
        }

        private void GenerateHeightTexture(HeightLayer _hLayer)
        {
            _hLayer.SavedMap = MainAlgorithms.GeneratedSimplexTexture(Terrain, _hLayer.LayerProperties);
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

        protected string GUITextField(string _label, string _text)
        {
            //var _textWidth = baseSkin.label.CalcSize(new GUIContent(_label));
            //Debug.Log("Text width for " + _label + " is " + _textWidth);
            var _value = "";
            //using (new EditorGUILayout.P())
            GUIContent _useLabel = new GUIContent(_label);

            _value = EditorGUILayout.TextField(_useLabel, _text, baseSkin.textField);

            return _value;
        }

        protected bool GUIButton(string _label)
        {
            return GUILayout.Button(_label, baseSkin.button);
        }

        protected void GUIVector2Field(string _label, ref Vector2 _value)
        {
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(_label, baseSkin.label);
                _value.x = EditorGUILayout.FloatField("x", _value.x, baseSkin.GetStyle("floatField"));
                _value.y = EditorGUILayout.FloatField("y", _value.y, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
        }
        protected Vector3 GUIVector3Field(string _label, Vector3 _value)
        {
            Vector3 fieldvalue = _value;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(_label, baseSkin.label);
                fieldvalue.x = EditorGUILayout.FloatField("x", _value.x, baseSkin.GetStyle("floatField"));
                fieldvalue.y = EditorGUILayout.FloatField("y", _value.y, baseSkin.GetStyle("floatField"));
                fieldvalue.y = EditorGUILayout.FloatField("z", _value.z, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            return fieldvalue;
        }
        protected float GUIFloatField(string _label, float _value)
        {
            float fieldvalue = 0;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(_label, baseSkin.label);
                fieldvalue = EditorGUILayout.FloatField(" ", _value, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            return fieldvalue;
        }

        protected float GUIMinMaxFloatField(string _label, float _value, float _minVal, float _maxVal)
        {
            float fieldvalue = _minVal;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(_label, baseSkin.label);
                fieldvalue = EditorGUILayout.FloatField(" ", _value, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            if (fieldvalue > _maxVal)
                fieldvalue = _maxVal;
            if (fieldvalue < _minVal)
                fieldvalue = _minVal;
            return fieldvalue;
        }
        protected int GUIMinMaxIntField(string _label, int _value, int _minVal, int _maxVal)
        {
            int fieldvalue = _minVal;
            using (new GUILayout.HorizontalScope(baseSkin.FindStyle("Vector2Field")))
            {
                EditorGUILayout.LabelField(_label, baseSkin.label);
                fieldvalue = EditorGUILayout.IntField(" ", _value, baseSkin.GetStyle("floatField"));
                GUILayout.FlexibleSpace();
            }
            if (fieldvalue > _maxVal)
                fieldvalue = _maxVal;
            if (fieldvalue < _minVal)
                fieldvalue = _minVal;
            return fieldvalue;
        }
    }
    
}
