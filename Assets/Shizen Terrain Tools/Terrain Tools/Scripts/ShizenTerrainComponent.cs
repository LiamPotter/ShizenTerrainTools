using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

namespace Shizen
{
    /// <summary>
    /// The component to be added to whatever Terrain that needs to be edited
    /// </summary>
    [RequireComponent(typeof(Terrain))]
    public class ShizenTerrainComponent : ShizenComponent
    {
        private Terrain terrain;

        public Terrain Terrain { get { return terrain; } }

        public ShizenTerrain ShizenTerrain;

        public ShizenTerrainComponent()
        {
            openButtonLabel = "Open Shizen Terrain Tools";
        }
        public override bool Open()
        {
            terrain = GetComponent<Terrain>();
            Debug.Log("Opening Terrain Tools...");
            if (terrain == null)
            {
                Debug.LogError("Couldn't get Terrain! Aborting...");
                return false;
            }
            if(ShizenTerrain == null)
            {
                Debug.LogError("No Shizen Terrain assigned! Create a new Shizen Terrain by right-clicking the folder hierarchy" +
                    " and selecting Create/Shizen/Terrain!");
                return false;
            }
            return true;
        }
        public bool ResetTerrainHeights()
        {
            TerrainData tData = Terrain.terrainData;
            int xRes = tData.heightmapWidth;
            int yRes = tData.heightmapHeight;
            float[,] heights = tData.GetHeights(0, 0, xRes, yRes);



            for (int x = 0; x < xRes; x++)
            {
                for (int y = 0; y < yRes; y++)
                {
                    //Set the height at x and y coordinates to the red value of our heightmap
                    //Since the heightmap is greyscale, we can use red, green or blue as we want
                    heights[x, y] = 0;
                }
            }
            tData.SetHeights(0, 0, heights);
            return true;
        }
        public bool ApplyHeightsToTerrain()
        {
            Texture2D heightMap;
            try
            {
                heightMap = GetShizenHeightMap(ShizenTerrain);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error in applying heights to terrain: "+ex);
                return false;
                throw;
            }
            try
            {
                SetTerrainHeightsFromHeightmap(Terrain,heightMap);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error in applying heights to terrain: " + ex);
                return false;
                throw;
            }
            return true;
        }

        private Texture2D GetShizenHeightMap(ShizenTerrain shizenTerrain)
        {
            return shizenTerrain.FinalHeightmap;
        }

        private void SetTerrainHeightsFromHeightmap(Terrain terrain,Texture2D heightMap)
        {
            TerrainData tData = terrain.terrainData;
            int resolution = tData.heightmapResolution;
            int xRes = tData.heightmapWidth;
            int yRes = tData.heightmapHeight;
            float[,] heights = tData.GetHeights(0,0, xRes, yRes);

            Debug.Log("heightmap format is "+UnityEngine.Terrain.heightmapTextureFormat);

            for (int x = 0; x < xRes; x++)
            {
                for (int y = 0; y < yRes; y++)
                {
                    //Set the height at x and y coordinates to the red value of our heightmap
                    //Since the heightmap is greyscale, we can use red, green or blue as we want
                    heights[x, y] =heightMap.GetPixel(x, y).r;
                }
            }
            tData.SetHeights(0, 0, heights);
        }
        private float ColorAverage(Color inputColor)
        {
            float combination = inputColor.r + inputColor.g + inputColor.b;
            return combination / 3;
        }
    }
}
