using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shizen
{
    /// <summary>
    /// The Height Layer Class
    /// </summary>
    [System.Serializable]
    public class HeightLayer: ShizenScript
    {
        public int Position;
        public Texture2D SavedMap;
        public HeightLayerProps LayerProperties;
        public bool expandedInEditor=false;
        public HeightLayer(int _position)
        {
            Name = string.Format("Height Layer {0}", _position);
            Position = _position;
            LayerProperties = new HeightLayerProps();
        }
    }
}