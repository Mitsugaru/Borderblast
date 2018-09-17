using System;
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
        public bool useCollider, useColors, useUVCoordinates;

        /// <summary>
        /// Mesh reference
        /// </summary>
        private Mesh hexMesh;

        /// <summary>
        /// List of verticies
        /// </summary>
        [NonSerialized]
        private List<Vector3> vertices;

        /// <summary>
        /// List of triangle indices
        /// </summary>
        [NonSerialized]
        private List<int> triangles;

        /// <summary>
        /// List of colors per indices
        /// </summary>
        [NonSerialized]
        private List<Color> colors;

        /// <summary>
        /// UV Coordinates
        /// </summary>
        [NonSerialized]
        private List<Vector2> uvs;

        /// <summary>
        /// Physics mesh collider
        /// </summary>
        private MeshCollider meshCollider;

        private void Awake()
        {
            if (useCollider)
            {
                meshCollider = GetComponent<MeshCollider>();
            }
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            hexMesh.name = "Hex Mesh";
        }

        public void Clear()
        {
            hexMesh.Clear();
            vertices = ListPool<Vector3>.Get();
            if (useColors)
            {
                colors = ListPool<Color>.Get();
            }
            if(useUVCoordinates)
            {
                uvs = ListPool<Vector2>.Get();
            }
            triangles = ListPool<int>.Get();
        }

        public void Apply()
        {
            hexMesh.SetVertices(vertices);
            ListPool<Vector3>.Add(vertices);
            if (useColors)
            {
                hexMesh.SetColors(colors);
                ListPool<Color>.Add(colors);
            }
            if (useUVCoordinates)
            {
                hexMesh.SetUVs(0, uvs);
                ListPool<Vector2>.Add(uvs);
            }
            hexMesh.SetTriangles(triangles, 0);
            ListPool<int>.Add(triangles);
            hexMesh.RecalculateNormals();
            if (useCollider)
            {
                meshCollider.sharedMesh = hexMesh;
            }
        }

        /// <summary>
        /// Generate a single triangle for the given verticies
        /// and automatically add it to the list
        /// </summary>
        /// <param name="v1">Vertex 1</param>
        /// <param name="v2">Vertex 2</param>
        /// <param name="v3">Vertex 3</param>
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(HexMetrics.Perturb(v1));
            vertices.Add(HexMetrics.Perturb(v2));
            vertices.Add(HexMetrics.Perturb(v3));
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// Shortcut method for single color triangle
        /// </summary>
        /// <param name="color">Color for entire triangle</param>
        public void AddTriangleColor(Color color)
        {
            AddTriangleColor(color, color, color);
        }

        /// <summary>
        /// Add the given color to the mesh list
        /// </summary>
        /// <param name="color">Color to add</param>
        public void AddTriangleColor(Color c1, Color c2, Color c3)
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
        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(HexMetrics.Perturb(v1));
            vertices.Add(HexMetrics.Perturb(v2));
            vertices.Add(HexMetrics.Perturb(v3));
            vertices.Add(HexMetrics.Perturb(v4));
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        /// <summary>
        /// Shortcut method to add a quad of a single color
        /// </summary>
        /// <param name="color">Primary color for quad</param>
        public void AddQuadColor(Color color)
        {
            AddQuadColor(color, color, color, color);
        }

        /// <summary>
        /// Shortcut method to add a quad of two colors
        /// </summary>
        /// <param name="c1">First color</param>
        /// <param name="c2">Second Color</param>
        public void AddQuadColor(Color c1, Color c2)
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
        public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }

        public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
        }

        public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
            uvs.Add(uv4);
        }

        public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
        {
            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMax, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
        }
    }
}