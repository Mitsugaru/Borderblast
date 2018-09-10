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

        private void Awake()
        {
            SelectColor(0);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                EditCell(hexGrid.GetCell(hit.point));
            }
        }

        private void EditCell(HexCell cell)
        {
            cell.color = activeColor;
            cell.Elevation = activeElevation;
            hexGrid.Refresh();
        }

        /// <summary>
        /// Select a color from the array
        /// </summary>
        /// <param name="index">Array index</param>
        public void SelectColor(int index)
        {
            activeColor = colors[index];
        }

        /// <summary>
        /// Set the elevation, rounded to integer
        /// </summary>
        /// <param name="elevation">Elevation</param>
        public void SetElevation(float elevation)
        {
            activeElevation = (int)elevation;
        }
    }
}