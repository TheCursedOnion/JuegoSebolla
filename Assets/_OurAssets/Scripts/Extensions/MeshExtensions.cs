using CursedOnion.Helpers;
using UnityEngine;

namespace CursedOnion.Extensions
{
    public static class MeshExtensions
    {
        public static Mesh CentricCombineMeshes(this Mesh mesh, CombineInstance[] combiners, bool mergeSubMeshes = false, bool useMatrices = true)
        {
            mesh.CombineMeshes(combiners, mergeSubMeshes, useMatrices);
            mesh.CenterMesh();
            
            mesh.RecalculateTangents();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            return mesh;
        }
        public static Mesh CenterMesh(this Mesh mesh)
        {
            Vector3 center = mesh.bounds.center;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= center;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            return mesh;
        }

        public static void ColorVertices(this Mesh mesh, IntRange vertexRange, Color color)
        {
            Color[] colors = new Color[mesh.vertices.Length];

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                colors[i] = i >= vertexRange.Start && i <= vertexRange.End ? color : Color.white;
            }
            mesh.colors = colors;
        }
        public static void ColorVertices(this Mesh mesh, IntRange[] vertexRanges, Color color)
        {
            Color[] colors = new Color[mesh.vertices.Length];

            int previousIndex = 0;
            for (int j = 0; j < vertexRanges.Length; j++)
            {
                for (int i = previousIndex; i <  mesh.vertices.Length; i++)
                {
                    colors[i] = i >= vertexRanges[j].Start && i <=  vertexRanges[j].End ? color : Color.white;
                    if (i ==  vertexRanges[j].End) break;
                }

                previousIndex = vertexRanges[j].End + 1;
                if (j == vertexRanges.Length - 1)
                {
                    for (int i = previousIndex; i <  mesh.vertices.Length; i++)
                    {
                        colors[i] = Color.white;
                    }
                }
            }

            mesh.colors = colors;
        }

        public static void Color32Vertices(this Mesh mesh, IntRange vertexRange, Color color)
        {
            Color32[] colors = new Color32[mesh.vertices.Length];

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                colors[i] = i >= vertexRange.Start && i <= vertexRange.End ? color : Color.white;
            }
            mesh.colors32 = colors;
        }
        public static void Color32Vertices(this Mesh mesh, IntRange[] vertexRanges, Color color)
        {
            Color32[] colors = new Color32[mesh.vertices.Length];

            int previousIndex = 0;
            for (int j = 0; j < vertexRanges.Length; j++)
            {
                for (int i = previousIndex; i <  mesh.vertices.Length; i++)
                {
                    colors[i] = i >= vertexRanges[j].Start && i <=  vertexRanges[j].End ? color : Color.white;
                    if (i ==  vertexRanges[j].End) break;
                }

                previousIndex = vertexRanges[j].End + 1;
                if (j == vertexRanges.Length - 1)
                {
                    for (int i = previousIndex; i <  mesh.vertices.Length; i++)
                    {
                        colors[i] = Color.white;
                    }
                }
            }

            mesh.colors32 = colors;
        }
        
    }
}
