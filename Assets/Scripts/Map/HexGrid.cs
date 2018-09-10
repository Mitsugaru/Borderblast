using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Borderblast.Map
{
    /// <summary>
    /// Represents a hex grid
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// Chunk count width
        /// </summary>
        public int chunkCountX = 4;

        /// <summary>
        /// Chunk count height
        /// </summary>
        public int chunkCountZ = 3;

        /// <summary>
        /// Default cell color
        /// </summary>
        public Color defaultColor = Color.white;

        /// <summary>
        /// Interacted / touched cell color
        /// </summary>
        public Color touchedColor = Color.magenta;

        /// <summary>
        /// Cell script prefab
        /// </summary>
        public HexCell cellPrefab;

        /// <summary>
        /// Text cell label prefab
        /// </summary>
        public Text cellLabelPrefab;

        /// <summary>
        /// Grid chunk prefab
        /// </summary>
        public HexGridChunk chunkPrefab;

        /// <summary>
        /// TODO remove with perlin noise generation
        /// </summary>
        public Texture2D noiseSource;

        /// <summary>
        /// Cells, width wise
        /// </summary>
        private int cellCountX = 6;

        /// <summary>
        /// Cells, height wise
        /// </summary>
        private int cellCountZ = 6;

        private HexGridChunk[] chunks;

        /// <summary>
        /// Array of cell instances
        /// </summary>
        private HexCell[] cells;

        private void Awake()
        {
            HexMetrics.noiseSource = noiseSource;

            cellCountX = chunkCountX * HexMetrics.chunkSizeX;
            cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

            CreateChunks();
            CreateCells();
        }

        private void CreateChunks()
        {
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            for (int z = 0, i = 0; z < chunkCountZ; z++)
            {
                for (int x = 0; x < chunkCountX; x++)
                {
                    HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                    chunk.transform.SetParent(transform);
                }
            }
        }

        private void CreateCells()
        {
            cells = new HexCell[cellCountZ * cellCountX];

            for (int z = 0, i = 0; z < cellCountZ; z++)
            {
                for (int x = 0; x < cellCountX; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        private void OnEnable()
        {
            HexMetrics.noiseSource = noiseSource;
        }

        /// <summary>
        /// Get target HexCell at the given position 
        /// </summary>
        /// <param name="position">Position from raycast</param>
        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
            return cells[index];
        }

        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= cellCountZ)
            {
                return null;
            }
            int x = coordinates.X + z / 2;
            if (x < 0 || x >= cellCountX)
            {
                return null;
            }
            return cells[x + z * cellCountX];
        }

        public void ShowUI(bool visible)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].ShowUI(visible);
            }
        }

        /// <summary>
        /// Create a cell
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="z">Y location</param>
        /// <param name="i">Index in grid array</param>
        private void CreateCell(int x, int z, int i)
        {
            Vector3 pos = new Vector3((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), 0f, z * (HexMetrics.outerRadius * 1.5f));

            HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.localPosition = pos;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = defaultColor;
            cell.name = cell.coordinates.ToString();

            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();

            cell.uiRect = label.rectTransform;
            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);

            // Setting cell neighbors
            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if(z > 0)
            {
                // Bitwise AND operator for even numbers
                if((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                    if(x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                    if (x < cellCountX - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                    }
                }
            }
        }

        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x / HexMetrics.chunkSizeX;
            int chunkZ = z / HexMetrics.chunkSizeZ;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

            int localX = x - chunkX * HexMetrics.chunkSizeX;
            int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }
    }
}