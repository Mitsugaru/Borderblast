using UnityEngine;

namespace Borderblast.Map
{
    public class HexGridChunk : MonoBehaviour
    {

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

            cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
            ShowUI(false);
        }

        private void LateUpdate()
        {
            hexMesh.Triangulate(cells);
            enabled = false;
        }

        public void AddCell(int index, HexCell cell)
        {
            cells[index] = cell;
            cell.chunk = this;
            cell.transform.SetParent(transform, false);
            cell.uiRect.SetParent(gridCanvas.transform, false);
        }

        public void Refresh()
        {
            enabled = true;
        }

        public void ShowUI(bool visible)
        {
            gridCanvas.gameObject.SetActive(visible);
        }
    }
}