using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borderblast.Map
{
    /// <summary>
    /// Represents a cell in the Hex map
    /// </summary>
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// Cell coordinates
        /// </summary>
        public HexCoordinates coordinates;

        /// <summary>
        /// Cell color
        /// </summary>
        public Color color;
    }
}