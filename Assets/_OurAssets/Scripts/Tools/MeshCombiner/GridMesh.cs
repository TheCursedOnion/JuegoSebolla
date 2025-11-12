using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
using CursedOnion.ScriptableObjects;
using UnityEngine;

namespace CursedOnion.Tools
{
    public class GridMesh
    {
        private readonly Grid3d grid;
        
        private readonly List<CombineInstance> combineInstances = new();
        
        private readonly Mesh mesh = new();
            public Mesh Mesh => mesh;
            
        private readonly List<Material> materials  = new();
            public Material[] MaterialsArray => materials.ToArray();
            
        
        public GridMesh(MeshFilter[] filters, Grid3d grid)
        {
            this.grid = grid;
            
            var meshMaterialDictionary = BuildMeshMaterialDictionary(filters);
            CombineMeshesWithMaterials(meshMaterialDictionary);
            
        }


            Dictionary<Material, List<CombineInstance>> BuildMeshMaterialDictionary(MeshFilter[] filters)
            {
                Dictionary<Material, List<CombineInstance>> meshMaterialDictionary = new Dictionary<Material, List<CombineInstance>>();
                foreach (MeshFilter filter in filters)
                {
                    MeshRenderer meshRenderer = filter.GetComponent<MeshRenderer>();
                    if (meshRenderer == null || meshRenderer.sharedMaterial == null || filter.sharedMesh == null) continue;

                    Matrix4x4 meshTransformMatrix = filter.transform.localToWorldMatrix;
                    
                    Mesh processedMesh = ProcessMesh(filter);
                    if(processedMesh == null) continue;
                    
                    
                    Material material = meshRenderer.sharedMaterial;
                    if (!meshMaterialDictionary.ContainsKey(material))
                        meshMaterialDictionary.Add(material, new List<CombineInstance>());
                    
                    filter.transform.GetComponent<MeshRenderer>().sharedMaterial = material;
                    
                    CombineInstance combineInstance = new CombineInstance
                    {
                        subMeshIndex = 0,
                        mesh = processedMesh,
                        transform = meshTransformMatrix
                    };
                    
                    meshMaterialDictionary[material].Add(combineInstance);
                }
                return meshMaterialDictionary;
            }
            
            List<int> newTriangles = new List<int>();
            List<int> vertexIndices = new List<int>();

            Mesh ProcessMesh(MeshFilter filter)
            {
                Mesh resultMesh = filter.sharedMesh.Clone();
                Transform transform = filter.transform;

                if (grid.TryWorldToGridPosition(transform.position, out Vector3 gridPosition))
                {
                    newTriangles.Clear();
                    vertexIndices.Clear();
                    
                    RemoveHiddenFaces(resultMesh, gridPosition, transform);
                    RemapVertices(resultMesh);

                    return resultMesh;
                }
                return null;
            }

            void RemoveHiddenFaces(Mesh resultMesh, Vector3 gridPosition, Transform meshTransform)
            {
                int[] triangles = resultMesh.triangles;
                Vector3[] vertices = resultMesh.vertices;

                Tile3d tile = grid.GetTileAtGridPosition(gridPosition);

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3 v0 = vertices[triangles[i]];
                    Vector3 v1 = vertices[triangles[i + 1]];
                    Vector3 v2 = vertices[triangles[i + 2]];

                    Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                    Vector3 worldNormal = meshTransform.TransformDirection(normal);
                    
                    Vector3Int gridOffset = worldNormal.RoundToInt();
                    Vector3 possibleNeighbourPosition = gridPosition + gridOffset;

                    bool isVisible = true;

                    if (grid.IsGridPositionInBounds(possibleNeighbourPosition) && worldNormal.IsCardinalDirection())
                    {
                        Tile3d neighbourTile = grid.GetTileAtGridPosition(possibleNeighbourPosition);
                        isVisible = neighbourTile.IsEmptyTile() ||
                                    (tile.IsFullTile() && !neighbourTile.IsFullTile()) ||
                                    (tile.IsFluidTile() && !neighbourTile.IsFluidTile()) ||
                                    (!tile.IsFullTile() && !tile.IsFluidTile() && !neighbourTile.IsFullTile());
                    }

                    if (isVisible)
                    {
                        AddTriangle(triangles[i]);
                        AddTriangle(triangles[i + 1]);
                        AddTriangle(triangles[i + 2]);
                    }
                }
            }

            void AddTriangle(int triangleIndex)
            {
                newTriangles.Add(triangleIndex);
                if (!vertexIndices.Contains(triangleIndex))
                    vertexIndices.Add(triangleIndex);
            }

            void RemapVertices(Mesh resultMesh)
            {
                var indexMap = new Dictionary<int, int>(vertexIndices.Count);

                Vector3[] oldVerts = resultMesh.vertices;
                Vector2[] oldUV = resultMesh.uv;
                Vector3[] oldNormals = resultMesh.normals;

                var newVerts = new List<Vector3>(vertexIndices.Count);
                var newUVs = new List<Vector2>(vertexIndices.Count);
                var newNormals = new List<Vector3>(vertexIndices.Count);

                int nextIndex = 0;
                foreach (int oldIndex in vertexIndices)
                {
                    indexMap[oldIndex] = nextIndex++;
                    newVerts.Add(oldVerts[oldIndex]);

                    if (oldUV != null && oldUV.Length > oldIndex)
                        newUVs.Add(oldUV[oldIndex]);
                    if (oldNormals != null && oldNormals.Length > oldIndex)
                        newNormals.Add(oldNormals[oldIndex]);
                }

                int[] remapped = new int[newTriangles.Count];
                for (int i = 0; i < newTriangles.Count; i++)
                {
                    int oldIdx = newTriangles[i];
                    if (!indexMap.TryGetValue(oldIdx, out int mappedIdx))
                    {
                        Debug.LogError($"[MeshProcessor] Índice {oldIdx} no encontrado en vertexIndices");
                        mappedIdx = 0;
                    }
                    remapped[i] = mappedIdx;
                }

                resultMesh.Clear();
                resultMesh.vertices = newVerts.ToArray();
                resultMesh.triangles = remapped;
                if (newUVs.Count > 0) resultMesh.uv = newUVs.ToArray();
                if (newNormals.Count > 0) resultMesh.normals = newNormals.ToArray();

                resultMesh.RecalculateBounds();
                resultMesh.RecalculateTangents();
            }
            
            void CombineMeshesWithMaterials(Dictionary<Material, List<CombineInstance>> dictionary)
            {
                foreach (var keyValuePair in dictionary)
                {
                    Material mat = keyValuePair.Key;
                    List<CombineInstance> combineInstances = keyValuePair.Value;

                    Mesh meshPerMat = new Mesh();
                    meshPerMat.CombineMeshes(combineInstances.ToArray(), true, true);

                    CombineInstance ci = new CombineInstance
                    {
                        subMeshIndex = 0,
                        mesh = meshPerMat,
                        transform = Matrix4x4.identity
                    };

                    this.combineInstances.Add(ci);
                    materials.Add(mat);
                }
                
                mesh.CentricCombineMeshes(combineInstances.ToArray());
            }
        
    }
}