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
            if(neighbor == null)
            {
                return;
            }

            Vector3 bridge = HexMetrics.GetBridge(direction);
            Vector3 v3 = v1 + bridge;
            Vector3 v4 = v2 + bridge;

            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.color, neighbor.color);

            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
                AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
            }
        }
    }
}