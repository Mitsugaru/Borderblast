using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borderblast.Map
{
    /// <summary>
    /// Represents coordinates for a hex grid system
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates
    {
        [SerializeField]
        private int x, z;

        /// <summary>
        /// X Coordinate
        /// </summary>
        public int X
        {
            get
            {
                return x;
            }
        }

        /// <summary>
        /// Derived Y Coordinate
        /// </summary>
        public int Y
        {
            get
            {
                return -X - Z;
            }
        }

        /// <summary>
        /// Z Coordinate
        /// </summary>
        public int Z
        {
            get
            {
                return z;
            }
        }

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        /// <summary>
        /// Translate from the rectangular row offset into axial coordinates
        /// </summary>
        /// <param name="x">Rectangular X coordinate</param>
        /// <param name="z">Rectangular Z coordinate</param>
        /// <returns>Relative HexCoordinates in axial coordinate system</returns>
        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
        }

        public string ToStringOnSeparateLines()
        {
            return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
        }

        /// <summary>
        /// Convert from local grid position to hex position
        /// </summary>
        /// <param name="position">Grid position</param>
        /// <returns>HexCoordinates</returns>
        public static HexCoordinates FromPosition(Vector3 position)
        {
            float x = position.x / (HexMetrics.innerRadius * 2f);
            float y = -x;

            float offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset;

            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);

            // Adjustment for the edge between hexagons
            // Discard the coordinate with the largest rounding delta
            // and reconstruct it from the other two
            if (iX + iY + iZ != 0)
            {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);

                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }

            return new HexCoordinates(iX, iZ);
        }
    }
}