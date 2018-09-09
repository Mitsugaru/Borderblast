using UnityEngine;

namespace Borderblast.Map
{
    /// <summary>
    /// Static utility class to help with hex map math
    /// </summary>
    public static class HexMetrics
    {
        public const float outerRadius = 10f;

        public const float innerRadius = outerRadius * 0.866025404f;

        public static Vector3[] corners = {
            new Vector3(0f, 0f, outerRadius),
            new Vector3(innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(0f, 0f, -outerRadius),
            new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(0f, 0f, outerRadius)
        };

        /// <summary>
        /// Inner fraction region of the hexagon that is solid
        /// </summary>
        public const float solidFactor = 0.75f;

        /// <summary>
        /// Outer blend region
        /// </summary>
        public const float blendFactor = 1f - solidFactor;

        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return corners[(int)direction];
        }

        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return corners[(int)direction + 1];
        }

        /// <summary>
        /// Gets the first corner vertex of the solid region of the hexagon
        /// </summary>
        /// <param name="direction">Direction of the hex region</param>
        /// <returns>Solid region first vertex point of the corresponding direction</returns>
        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return corners[(int)direction] * solidFactor;
        }

        /// <summary>
        /// Gets the second corner vertex of the solid region of the hexagon
        /// </summary>
        /// <param name="direction">Direction of the hex region</param>
        /// <returns>Solid region second vertex point of the corresponding direction</returns>
        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return corners[(int)direction + 1] * solidFactor;
        }

        /// <summary>
        /// Calculate the bridge between the cell and the neighbor at the given direction
        /// for the appropriate blend section.
        /// </summary>
        /// <param name="direction">Neighbor cell direction</param>
        /// <returns>Bridge offset vector</returns>
        public static Vector3 GetBridge(HexDirection direction)
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
        }
    }
}