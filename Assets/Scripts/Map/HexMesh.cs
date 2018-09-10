using System.Collections.Generic;
using UnityEngine;

namespace Borderblast.Map
{
    /// <summary>
    /// Mesh for the hex map
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        /// <summary>
        /// Mesh reference
        /// </summary>
        private Mesh hexMesh;

        /// <summary>
        /// List of verticies
        /// </summary>
        private List<Vector3> vertices = new List<Vector3>();

        /// <summary>
        /// List of triangle indices
        /// </summary>
        private List<int> triangles = new List<int>();

        /// <summary>
        /// List of colors per indices
        /// </summary>
        private List<Color> colors = new List<Color>();

        /// <summary>
        /// Physics mesh collider
        /// </summary>
        private MeshCollider meshCollider;

        private void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            hexMesh.name = "Hex Mesh";
        }

        /// <summary>
        /// Triangulate the entire map of HexCells
        /// </summary>
        /// <param name="cells">Array of HexCells</param>
        public void Triangulate(HexCell[] cells)
        {
            hexMesh.Clear();
            vertices.Clear();
            triangles.Clear();
            colors.Clear();

            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }

            hexMesh.vertices = vertices.ToArray();
            hexMesh.triangles = triangles.ToArray();
            hexMesh.colors = colors.ToArray();
            hexMesh.RecalculateNormals();

            meshCollider.sharedMesh = hexMesh;
        }

        /// <summary>
        /// Triangulate a specific HexCell
        /// </summary>
        /// <param name="cell">Cell to triangulate</param>
        private void Triangulate(HexCell cell)
        {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Triangulate(d, cell);
            }
        }

        /// <summary>
        /// Triangulate a section of the hexagon for the given direction
        /// </summary>
        /// <param name="direction">Direction of the hexagon</param>
        /// <param name="cell">Specific cell to triangulate</param>
        private void Triangulate(HexDirection direction, HexCell cell)
        {
            Vector3 center = cell.transform.localPosition;
            Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
            Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
            AddTriangle(center, v1, v2);
            AddTriangleColor(cell.color);

            // Only add necessary connections between tiles
            if (direction <= HexDirection.SE)
            {
                TriangulateConnection(direction, cell, v1, v2);
            }
        }

        /// <summary>
        /// Generate a single triangle for the given verticies
        /// and automatically add it to the list
        /// </summary>
        /// <param name="v1">Vertex 1</param>
        /// <param name="v2">Vertex 2</param>
        /// <param name="v3">Vertex 3</param>
        private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// Shortcut method for single color triangle
        /// </summary>
        /// <param name="color">Color for entire triangle</param>
        private void AddTriangleColor(Color color)
        {
            AddTriangleColor(color, color, color);
        }

        /// <summary>
        /// Add the given color to the mesh list
        /// </summary>
        /// <param name="color">Color to add</param>
        private void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }

        /// <summary>
        /// Add a quad with the given verticies
        /// </summary>
        /// <param name="v1">Vertex 1</param>
        /// <param name="v2">Vertex 2</param>
        /// <param name="v3">Vertex 3</param>
        /// <param name="v4">Vertex 4</param>
        private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        /// <summary>
        /// Shortcut method to add a quad of two colors
        /// </summary>
        /// <param name="c1">First color</param>
        /// <param name="c2">Second Color</param>
        private void AddQuadColor(Color c1, Color c2)
        {
            AddQuadColor(c1, c1, c2, c2);
        }

        /// <summary>
        /// Add colors for a quad
        /// </summary>
        /// <param name="c1">Vertex 1 color</param>
        /// <param name="c2">Vertex 2 color</param>
        /// <param name="c3">Vertex 3 color</param>
        /// <param name="c4">Vertex 4 color</param>
        private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }

        /// <summary>
        /// Create the connection / bridge piece between cells
        /// </summary>
        /// <param name="direction">Direction of neighbor cell</param>
        /// <param name="cell">Current cell</param>
        /// <param name="v1">Cell first solid corner</param>
        /// <param name="v2">Cell second solid corner</param>
        private void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
        {
            HexCell neighbor = cell.GetNeighbor(direction);

            //Ignore bridges for edges
            if (neighbor == null)
            {
                return;
            }

            Vector3 bridge = HexMetrics.GetBridge(direction);
            Vector3 v3 = v1 + bridge;
            Vector3 v4 = v2 + bridge;
            v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;

            if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
            {
                TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
            }
            else
            {
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(cell.color, neighbor.color);
            }

            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
                v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;

                if (cell.Elevation <= neighbor.Elevation)
                {
                    if (cell.Elevation <= nextNeighbor.Elevation)
                    {
                        TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
                    }
                    else
                    {
                        TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
                    }
                }
                else if (neighbor.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(v4, neighbor, v5, nextNeighbor, v2, cell);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
                }
            }
        }

        private void TriangulateEdgeTerraces(Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
        Vector3 endLeft, Vector3 endRight, HexCell endCell)
        {
            Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
            Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
            Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

            AddQuad(beginLeft, beginRight, v3, v4);
            AddQuadColor(beginCell.color, c2);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                Vector3 v1 = v3;
                Vector3 v2 = v4;
                Color c1 = c2;
                v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
                v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
                c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2);
            }

            AddQuad(v3, v4, endLeft, endRight);
            AddQuadColor(c2, endCell.color);
        }

        private void TriangulateCorner(Vector3 bottom, HexCell bottomCell,
        Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
            HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

            if (leftEdgeType == HexEdgeType.Slope)
            {
                if (rightEdgeType == HexEdgeType.Slope)
                {
                    TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                }
                else if (rightEdgeType == HexEdgeType.Flat)
                {
                    TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
                }
                TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
            }
            else if (rightEdgeType == HexEdgeType.Slope)
            {
                if (leftEdgeType == HexEdgeType.Flat)
                {
                    TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                }
            }
            else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                if (leftCell.Elevation < rightCell.Elevation)
                {
                    TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
                }
            }
            else
            {
                AddTriangle(bottom, left, right);
                AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
            }
        }

        private void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
            Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
            Color c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
            Color c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);

            AddTriangle(begin, v3, v4);
            AddTriangleColor(beginCell.color, c3, c4);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                Vector3 v1 = v3;
                Vector3 v2 = v4;
                Color c1 = c3;
                Color c2 = c4;
                v3 = HexMetrics.TerraceLerp(begin, left, i);
                v4 = HexMetrics.TerraceLerp(begin, right, i);
                c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
                c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2, c3, c4);
            }

            AddQuad(v3, v4, left, right);
            AddQuadColor(c3, c4, leftCell.color, rightCell.color);
        }

        private void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            float b = Mathf.Abs(1f / (rightCell.Elevation - beginCell.Elevation));
            Vector3 boundary = Vector3.Lerp(begin, right, b);
            Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

            TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangle(left, right, boundary);
                AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
            }
        }

        private void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            float b = Mathf.Abs(1f / (leftCell.Elevation - beginCell.Elevation));
            Vector3 boundary = Vector3.Lerp(begin, left, b);
            Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

            TriangulateBoundaryTriangle(right, rightCell, begin, beginCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangle(left, right, boundary);
                AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
            }
        }

        private void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell, Vector3 boundary, Color boundaryColor)
        {
            Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
            Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

            AddTriangle(begin, v2, boundary);
            AddTriangleColor(beginCell.color, c2, boundaryColor);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                Vector3 v1 = v2;
                Color c1 = c2;
                v2 = HexMetrics.TerraceLerp(begin, left, i);
                c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
                AddTriangle(v1, v2, boundary);
                AddTriangleColor(c1, c2, boundaryColor);
            }

            AddTriangle(v2, left, boundary);
            AddTriangleColor(c2, leftCell.color, boundaryColor);
        }
    }
}