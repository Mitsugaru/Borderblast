using UnityEngine;

namespace Borderblast.Map
{
    /// <summary>
    /// Static utility class to help with hex map math
    /// </summary>
    public static class HexMetrics
    {
        public const float outerToInner = 0.866025404f;
        public const float innerToOuter = 1f / outerToInner;

        public const float outerRadius = 10f;

        public const float innerRadius = outerRadius * outerToInner;

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
        public const float solidFactor = 0.8f;

        /// <summary>
        /// Outer blend region
        /// </summary>
        public const float blendFactor = 1f - solidFactor;

        /// <summary>
        /// Number of units per elevation level
        /// </summary>
        public const float elevationStep = 3f;

        public const int terracesPerSlope = 2;

        public const int terraceSteps = terracesPerSlope * 2 + 1;

        public const float horizontalTerraceStepSize = 1f / terraceSteps;

        public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

        public const float streamBedElevationOffset = -1.75f;

        public const float riverSurfaceElevationOffset = -0.5f;

        /// <summary>
        /// Unit strength of cell perturbation
        /// </summary>
        public const float cellPerturbStrength = 4f;

        /// <summary>
        /// Unit strength of elevation perturbation
        /// </summary>
        public const float elevationPerturbStrength = 1.5f;

        /// <summary>
        /// Scale the perlin noise sampling across the texture
        /// </summary>
        public const float noiseScale = 0.003f;

        /// <summary>
        /// Chunk size
        /// </summary>
        public const int chunkSizeX = 5, chunkSizeZ = 5;

        /// <summary>
        /// TODO change to use perlin noise generation
        /// </summary>
        public static Texture2D noiseSource;

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

        /// <summary>
        /// Custom interpolation for terrace steps
        /// 
        /// TODO we really should not be modifying a
        /// however, current calls expect a to be modified
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            float h = step * HexMetrics.horizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }

        /// <summary>
        /// Color interpolation for terrace steps
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Color TerraceLerp(Color a, Color b, int step)
        {
            float h = step * HexMetrics.horizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }

        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {
                return HexEdgeType.Flat;
            }
            else if (Mathf.Abs(elevation1 - elevation2) <= 1)
            {
                return HexEdgeType.Slope;
            }
            return HexEdgeType.Cliff;
        }

        /// <summary>
        /// TODO Change this to return information from perlin noise generation
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector4 SampleNoise(Vector3 position)
        {
            return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
        }

        /// <summary>
        /// Vector of the middle of a solid edge by averaging two adjacent corner vectors
        /// and applying the solid factor
        /// </summary>
        /// <param name="direction">Edge direction</param>
        /// <returns></returns>
        public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) * (0.5f * solidFactor);
        }

        public static Vector3 Perturb(Vector3 position)
        {
            Vector4 sample = SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
            position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
            return position;
        }
    }
}