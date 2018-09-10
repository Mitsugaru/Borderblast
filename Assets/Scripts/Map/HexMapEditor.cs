using UnityEngine;
using UnityEngine.EventSystems;

namespace Borderblast.Map
{
    /// <summary>
    /// In-game editor of the HexGrid
    /// </summary>
    public class HexMapEditor : MonoBehaviour
    {
        /// <summary>
        /// Available color array
        /// </summary>
        public Color[] colors;

        /// <summary>
        /// Hex grid map reference
        /// </summary>
        public HexGrid hexGrid;

        /// <summary>
        /// Active chosen color
        /// </summary>
        private Color activeColor;

        /// <summary>
        /// Active chosen elevation
        /// </summary>
        private int activeElevation;

        private bool applyColor;

        private bool applyElevation = true;

        private int brushSize;

        private void Awake()
        {
            SelectColor(0);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            Ray inputRay = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                EditCells(hexGrid.GetCell(hit.point));
            }
        }

        private void EditCells(HexCell center)
        {
            int centerX = center.coordinates.X;
            int centerZ = center.coordinates.Z;

            for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
            {
                for (int x = centerX - r; x <= centerX + brushSize; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
            for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
            {
                for (int x = centerX - brushSize; x <= centerX + r; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        private void EditCell(HexCell cell)
        {
            if (cell)
            {
                if (applyColor)
                {
                    cell.Color = activeColor;
                }
                if (applyElevation)
                {
                    cell.Elevation = activeElevation;
                }
            }
        }

        /// <summary>
        /// Select a color from the array
        /// </summary>
        /// <param name="index">Array index</param>
        public void SelectColor(int index)
        {
            applyColor = index >= 0;
            if (applyColor)
            {
                activeColor = colors[index];
            }
        }

        /// <summary>
        /// Set the elevation, rounded to integer
        /// </summary>
        /// <param name="elevation">Elevation</param>
        public void SetElevation(float elevation)
        {
            activeElevation = (int)elevation;
        }

        public void SetApplyElevation(bool toggle)
        {
            applyElevation = toggle;
        }

        public void SetBrushSize(float size)
        {
            brushSize = (int)size;
        }

        public void ShowUI(bool visible)
        {
            hexGrid.ShowUI(visible);
        }
    }
}