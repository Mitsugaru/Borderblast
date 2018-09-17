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

        public OptionalToggle riverMode, roadMode;

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

        private bool isDrag;

        private HexDirection dragDirection;

        /// <summary>
        /// To help determine where we are dragging from
        /// </summary>
        private HexCell previousCell;

        public enum OptionalToggle
        {
            Ignore, Yes, No
        }

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
            else
            {
                previousCell = null;
            }
        }

        private void HandleInput()
        {
            Ray inputRay = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                HexCell currentCell = hexGrid.GetCell(hit.point);
                if(previousCell && previousCell != currentCell)
                {
                    isDrag = ValidateDrag(currentCell);
                }
                EditCells(currentCell);
                previousCell = currentCell;
            }
            else
            {
                previousCell = null;
            }
        }

        /// <summary>
        /// Validate the drag direction.
        /// 
        /// Note: When you're moving the cursor along cell edges, you might end up quickly
        /// oscillating between those cells. This can indeed produce jittery drags, but
        /// it's not that bad.
        /// 
        /// You could alleviate this by remembering the previous drag. Then prevent the
        /// next drag from immediately going in the opposite direction.
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns></returns>
        private bool ValidateDrag(HexCell currentCell)
        {
            bool valid = false;
            for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
            {
                if (previousCell.GetNeighbor(dragDirection) == currentCell)
                {
                    valid = true;
                    break;
                }
            }
            return valid;
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
                if(riverMode == OptionalToggle.No)
                {
                    cell.RemoveRiver();
                }
                if (roadMode == OptionalToggle.No)
                {
                    cell.RemoveRoads();
                }
                else if(isDrag)
                {
                    HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                    if(otherCell && riverMode == OptionalToggle.Yes)
                    {
                        otherCell.SetOutgoingRiver(dragDirection);
                    }
                    if(otherCell && roadMode == OptionalToggle.Yes)
                    {
                        otherCell.AddRoad(dragDirection);
                    }
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

        public void SetRiverMode(int mode)
        {
            riverMode = (OptionalToggle)mode;
        }

        public void SetRoadMode(int mode)
        {
            roadMode = (OptionalToggle)mode;
        }
    }
}