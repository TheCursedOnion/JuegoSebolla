using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Files;
using CursedOnion.ScriptableObjects;
using UnityEngine;

namespace CursedOnion.Tools
{
    public class CombinedMesh
    {
        private readonly List<CombineInstance> combineInstances = new();
        
        private readonly Mesh mesh = new();
            public Mesh Mesh => mesh;
            
        private readonly List<Material> materials  = new();
            public Material[] MaterialsArray => materials.ToArray();
            
        
        public CombinedMesh(MeshFilter[] filters)
        {
            var meshMaterialDictionary = BuildMeshMaterialDictionary(filters);
            AssingVariablesFromDictionary(meshMaterialDictionary);
        }
            Dictionary<Material, List<CombineInstance>> BuildMeshMaterialDictionary(MeshFilter[] filters)
            {
                Dictionary<Material, List<CombineInstance>> meshMaterialDictionary = new Dictionary<Material, List<CombineInstance>>();
                foreach (MeshFilter filter in filters)
                {
                    if (filter.sharedMesh == null) continue;

                    MeshRenderer meshRenderer = filter.GetComponent<MeshRenderer>();
                    if (meshRenderer == null || meshRenderer.sharedMaterial == null) continue;

                    Material material = meshRenderer.sharedMaterial;
                    if (!meshMaterialDictionary.ContainsKey(material))
                        meshMaterialDictionary.Add(material, new List<CombineInstance>());
                    
                    filter.transform.GetComponent<MeshRenderer>().sharedMaterial = material;

                    CombineInstance combineInstance = new CombineInstance
                    {
                        subMeshIndex = 0,
                        mesh = filter.sharedMesh,
                        transform = filter.transform.localToWorldMatrix
                    };
                    
                    meshMaterialDictionary[material].Add(combineInstance);
                }
                return meshMaterialDictionary;
            }
            void AssingVariablesFromDictionary(Dictionary<Material, List<CombineInstance>> dictionary)
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