using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shizen
{
    /// <summary>
    /// The Height Layer Class
    /// </summary>
    [System.Serializable]
    public class HeightLayer
    {
        public string Name;
        public int Position;
        public Texture2D SavedMap;
        public HeightLayerProperties LayerProperties;
        public bool Randomized=false;
        public bool ExpandedInEditor=false;

        public enum placeholderPresets
        {
            Custom,
            Plains,
            Hills,
            Mountains,
            Cliffs,
            Rocks,
            Forest
        }

        public placeholderPresets PlaceholderPreset;

        public HeightLayer(int _position)
        {
            Name = string.Format("Height Layer {0}", _position);
            Position = _position;
            LayerProperties = new HeightLayerProperties();
        }
    }
}