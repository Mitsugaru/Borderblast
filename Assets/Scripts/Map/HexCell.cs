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
        /// Label transform
        /// </summary>
        public RectTransform uiRect;

        /// <summary>
        /// Cell color
        /// </summary>
        public Color color;

        /// <summary>
        /// Local position property
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }
        
        /// <summary>
        /// Cell elevation
        /// </summary>
        private int elevation;

        /// <summary>
        /// Elevation property
        /// </summary>
        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;

                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
            }
        }

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

        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }

        public HexEdgeType GetEdgeType(HexCell otherCell)
        {
            return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
        }
    }
}