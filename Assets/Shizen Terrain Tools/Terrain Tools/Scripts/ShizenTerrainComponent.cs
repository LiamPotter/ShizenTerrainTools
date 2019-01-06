using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shizen
{
    /// <summary>
    /// The component to be added to whatever Terrain that needs to be edited
    /// </summary>
    [RequireComponent(typeof(Terrain))]
    public class ShizenTerrainComponent : ShizenComponent
    {
        private Terrain terrain;

        public Terrain Terrain { get { return terrain; } set { } }

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

    }
}
