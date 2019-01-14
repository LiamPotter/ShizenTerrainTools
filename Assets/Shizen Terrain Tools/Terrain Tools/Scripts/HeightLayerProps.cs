using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shizen
{
    /// <summary>
    /// The Height Layer Properties
    /// </summary>
    [System.Serializable]
    public class HeightLayerProps
    {
        public HeightLayerProperty Frequency;

        public HeightLayerProperty Amplitude;

        public HeightLayerProperty Octaves;

        public HeightLayerProperty Lacunarity;

        public HeightLayerProperty Persistance;

        public HeightLayerProperty Offset;

        public float Opacity;

        public HeightLayerProps()
        {
            Frequency = new HeightLayerProperty(4f);
            Amplitude = new HeightLayerProperty(1f);
            Octaves = new HeightLayerProperty((int)2);
            Lacunarity = new HeightLayerProperty(2f);
            Persistance = new HeightLayerProperty(0.5f);
            Offset = new HeightLayerProperty(Vector3.zero);
            Opacity = 1f;
        }
    }
}
