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
        /// Cells, width wise
        /// </summary>
        public int width = 6;

        /// <summary>
        /// Cells, height wise
        /// </summary>
        public int height = 6;

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
        /// TODO remove with perlin noise generation
        /// </summary>
        public Texture2D noiseSource;

        /// <summary>
        /// Array of cell instances
        /// </summary>
        private HexCell[] cells;

        /// <summary>
        /// Hex grid canvas
        /// </summary>
        private Canvas gridCanvas;

        /// <summary>
        /// Hex grid mesh
        /// </summary>
        private HexMesh hexMesh;

        private void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();
            hexMesh = GetComponentInChildren<HexMesh>();

            HexMetrics.noiseSource = noiseSource;

            cells = new HexCell[height * width];

            for(int z = 0, i = 0; z < height; z++)
            {
                for(int x = 0; x < width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        private void OnEnable()
        {
            HexMetrics.noiseSource = noiseSource;
        }

        private void Start()
        {
            hexMesh.Triangulate(cells);
        }

        /// <summary>
        /// Get target HexCell at the given position 
        /// </summary>
        /// <param name="position">Position from raycast</param>
        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            return cells[index];
        }

        /// <summary>
        /// Triangulate the grid map
        /// </summary>
        public void Refresh()
        {
            hexMesh.Triangulate(cells);
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
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = pos;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = defaultColor;
            cell.name = cell.coordinates.ToString();

            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();

            cell.uiRect = label.rectTransform;
            cell.Elevation = 0;

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
                    cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                    if(x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                    if (x < width - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                    }
                }
            }
        }
    }
}