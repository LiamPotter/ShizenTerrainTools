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
        public float Frequency;

        public float Amplitude;

        public int Octaves;

        public float Lacunarity;

        public float Persistance;

        public Vector3 Offset;

        public HeightLayerProps()
        {
            Frequency = 4;
            Amplitude = 1;
            Octaves = 2;
            Lacunarity = 2;
            Persistance = 0.5f;
            Offset = Vector3.zero;
        }
    }
}
