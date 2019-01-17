using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shizen.Base.Properties;
namespace Shizen
{
    /// <summary>
    /// The Height Layer Properties
    /// </summary>
    [System.Serializable]
    public class HeightLayerProperties
    {
        public FloatProperty Frequency;

        public FloatProperty Amplitude;

        public IntProperty Octaves;

        public FloatProperty Lacunarity;

        public FloatProperty Persistance;

        public Vector3Property Offset;

        public float Opacity;

        public HeightLayerProperties()
        {
            Frequency = new FloatProperty("Frequency",4f);
            Amplitude = new FloatProperty("Amplitude",1f);
            Octaves = new IntProperty("Octaves",2);
            Lacunarity = new FloatProperty("Lacunarity",2f);
            Persistance = new FloatProperty("Persistance",0.5f);
            Offset = new Vector3Property("Offset");
            Opacity = 1f;
        }
        public void DoRandomValueGeneration()
        {
            Frequency.CreateRandomValue();
            Amplitude.CreateRandomValue();
            Octaves.CreateRandomValue();
            Lacunarity.CreateRandomValue();
            Persistance.CreateRandomValue();
            Offset.CreateRandomValue();
        }

    }
}
