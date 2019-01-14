using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shizen
{
    [CreateAssetMenu(fileName ="Shizen Terrain",menuName ="Shizen/Terrain")]
    public class ShizenTerrain :ShizenScriptable
    {
        public List<HeightLayer> Heights;
        public List<ShizenScript> Textures;
        public List<ShizenScript> Details;

        public Texture2D FinalHeightmap;
    }
}
