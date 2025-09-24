using UnityEngine;

namespace CursedOnion.Extensions
{
    public static class MeshExtensions
    {
        public static Mesh CentricCombineMeshes(this Mesh mesh, CombineInstance[] combiners, bool mergeSubMeshes = false, bool useMatrices = true)
        {
            mesh.CombineMeshes(combiners, mergeSubMeshes, useMatrices);
            mesh.CenterMesh();
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
    }
}
