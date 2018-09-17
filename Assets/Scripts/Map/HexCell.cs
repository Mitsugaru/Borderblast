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
        private Color color;

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if(color == value)
                {
                    return;
                }
                color = value;
                Refresh();
            }
        }

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
        private int elevation = int.MinValue;

        /// <summary>
        /// Elevation property
        /// 
        /// TODO probably change it to an internal setter
        /// </summary>
        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                if(elevation == value)
                {
                    return;
                }
                elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;

                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;

                if (
                hasOutgoingRiver &&
                elevation < GetNeighbor(outgoingRiver).elevation)
                {
                    RemoveOutgoingRiver();
                }
                if (hasIncomingRiver && elevation > GetNeighbor(incomingRiver).elevation)
                {
                    RemoveIncomingRiver();
                }

                Refresh();
            }
        }

        /// <summary>
        /// Parent chunk reference
        /// </summary>
        public HexGridChunk chunk;

        public float StreamBedY
        {
            get
            {
                return (elevation + HexMetrics.streamBedElevationOffset) * HexMetrics.elevationStep;
            }
        }

        public float RiverSurfaceY
        {
            get
            {
                return (elevation + HexMetrics.riverSurfaceElevationOffset) * HexMetrics.elevationStep;
            }
        }

        private bool hasIncomingRiver, hasOutgoingRiver;
        private HexDirection incomingRiver, outgoingRiver;


        public bool HasIncomingRiver
        {
            get
            {
                return hasIncomingRiver;
            }
        }

        public bool HasOutgoingRiver
        {
            get
            {
                return hasOutgoingRiver;
            }
        }

        public HexDirection IncomingRiver
        {
            get
            {
                return incomingRiver;
            }
        }

        public HexDirection OutgoingRiver
        {
            get
            {
                return outgoingRiver;
            }
        }

        public bool HasRiver
        {
            get
            {
                return hasIncomingRiver || hasOutgoingRiver;
            }
        }

        public bool HasRiverBeginOrEnd
        {
            get
            {
                return hasIncomingRiver != hasOutgoingRiver;
            }
        }

        public bool HasRiverThroughEdge(HexDirection direction)
        {
            return
                hasIncomingRiver && incomingRiver == direction ||
                hasOutgoingRiver && outgoingRiver == direction;
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

        public void SetOutgoingRiver(HexDirection direction)
        {
            if (hasOutgoingRiver && outgoingRiver == direction)
            {
                return;
            }

            HexCell neighbor = GetNeighbor(direction);
            if (!neighbor || elevation < neighbor.elevation)
            {
                return;
            }

            RemoveOutgoingRiver();
            if (hasIncomingRiver && incomingRiver == direction)
            {
                RemoveIncomingRiver();
            }

            hasOutgoingRiver = true;
            outgoingRiver = direction;
            RefreshSelfOnly();

            neighbor.RemoveIncomingRiver();
            neighbor.hasIncomingRiver = true;
            neighbor.incomingRiver = direction.Opposite();
            neighbor.RefreshSelfOnly();
        }

        public void RemoveOutgoingRiver()
        {
            if (!hasOutgoingRiver)
            {
                return;
            }
            hasOutgoingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(outgoingRiver);
            neighbor.hasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        public void RemoveIncomingRiver()
        {
            if (!hasIncomingRiver)
            {
                return;
            }
            hasIncomingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(incomingRiver);
            neighbor.hasOutgoingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        public void RemoveRiver()
        {
            RemoveOutgoingRiver();
            RemoveIncomingRiver();
        }

        private void Refresh()
        {
            if (chunk)
            {
                chunk.Refresh();
                for (int i = 0; i < neighbors.Length; i++)
                {
                    HexCell neighbor = neighbors[i];
                    if (neighbor != null && neighbor.chunk != chunk)
                    {
                        neighbor.chunk.Refresh();
                    }
                }
            }
        }

        private void RefreshSelfOnly()
        {
            chunk.Refresh();
        }
    }
}