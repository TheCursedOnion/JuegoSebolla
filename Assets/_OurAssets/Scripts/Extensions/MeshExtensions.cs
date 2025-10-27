using System;
using CursedOnion.Game;
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

        public static void Color32Vertices(this Mesh mesh, IntRange vertexRange, Color color)
        {
            
            Color32[] colors = null;
            if (mesh.colors32.Length != mesh.vertices.Length)
            {
                colors = new Color32[mesh.vertices.Length];
            }
            else
            {
                colors = (Color32[])mesh.colors32.Clone();
            }
            
            if(!vertexRange.BoundedInArray(colors)) return;
                
            for (int i = vertexRange.Start; i <= vertexRange.End; i++)
            {
                colors[i] = color;
            }
            mesh.colors32 = colors;
        }
        public static void Color32Vertices(this Mesh mesh, IntRange[] vertexRanges, Color color)
        {
            Color32[] colors = new Color32[mesh.vertices.Length];
            
            Action<int, int> tryPaintInRange = (j, i) => colors[i] = i >= vertexRanges[j].Start && i <= vertexRanges[j].End ? color : Color.white;
            Action<int, int> defaultPaint = (_, i) => colors[i] = Color.white;
            
            Action<int, int> paintAtIndexAction = tryPaintInRange;

            int previousIndex = 0;
            for (int j = 0; j < vertexRanges.Length; j++)
            {
                paintAtIndexAction = vertexRanges[j].BoundedInArray(colors) ? tryPaintInRange : defaultPaint;
                for (int i = previousIndex; i <  mesh.vertices.Length; i++)
                {
                    paintAtIndexAction(j, i);
                    if (i ==  vertexRanges[j].End) break;
                }

                previousIndex = vertexRanges[j].End + 1;
                if (j == vertexRanges.Length - 1)
                {
                    for (int i = previousIndex; i <  mesh.vertices.Length; i++)
                    {
                       defaultPaint(j, i);
                    }
                }
            }

            mesh.colors32 = colors;
        }

        public static void FillColor32(this Mesh mesh, Color color)
        {
            Color32[] colors32 = new Color32[mesh.vertices.Length];
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                colors32[i] = color;
            }
            mesh.colors32 = colors32;
        }

        public static Mesh Clone(this Mesh original)
        {
            Mesh clone = new Mesh();

            clone.vertices = original.vertices;
            clone.normals = original.normals;
            clone.uv = original.uv;
            clone.triangles = original.triangles;
            clone.tangents = original.tangents;
            clone.colors = original.colors;

            return clone;
        }

        public static void SetMeshData(this Mesh mesh, Vector3[] vertices, int[] triangles, Vector2[] uvs, Vector3[] normals)
        {
            mesh.Clear();
            mesh.vertices = vertices;
            if (uvs.Length > 0) mesh.uv = uvs;
            if (normals.Length > 0) mesh.normals = normals;

            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
        
    }
}
