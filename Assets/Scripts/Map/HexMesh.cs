using System;
using System.Collections;
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
        private List<Vector3> verticies = new List<Vector3>();

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
            verticies.Clear();
            triangles.Clear();
            colors.Clear();

            for(int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }

            hexMesh.vertices = verticies.ToArray();
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
            Vector3 center = cell.transform.localPosition;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[i + 1]);
                AddTriangleColor(cell.color);
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
            int vertexIndex = verticies.Count;
            verticies.Add(v1);
            verticies.Add(v2);
            verticies.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// Add the given color to the mesh list
        /// </summary>
        /// <param name="color">Color to add</param>
        private void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
    }
}