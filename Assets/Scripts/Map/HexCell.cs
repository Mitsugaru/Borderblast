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

        [SerializeField]
        public HexCell[] neighbors;

        /// <summary>
        /// Cell color
        /// </summary>
        public Color color;

        public HexCell()
        {
            coordinates = new HexCoordinates(0, 0);
        }

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }
    }
}