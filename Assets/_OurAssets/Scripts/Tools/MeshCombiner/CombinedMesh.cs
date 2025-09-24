using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems;
using CursedOnion.ScriptableObjects;
using UnityEngine;

namespace CursedOnion.Tools
{
    public class CombinedMesh : ISaveableFileAsset
    {
        public List<CombineInstance> CombineInstances = new();
        public List<Material> Materials  = new();
        public Mesh Mesh = new();

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

                    CombineInstances.Add(ci);
                    Materials.Add(mat);
                }
                
                Mesh.CentricCombineMeshes(CombineInstances.ToArray());
            }
        
        public void SpawnObject()
        {
            GameObject combined = new GameObject("CombinedMesh");
            MeshFilter mfCombined = combined.AddComponent<MeshFilter>();
            MeshRenderer mrCombined = combined.AddComponent<MeshRenderer>();

            mfCombined.sharedMesh = Mesh;
            mrCombined.sharedMaterials = Materials.ToArray();
        }
        
        public void SaveResults()
        {
            FileAsset saveAsset = FileAsset.Default;
            saveAsset.SetObject(Mesh, "asset");
            FilePanelWindow.SaveFileAsset(ref saveAsset,"Guardar Mesh combinado", "Elige dónde guardar el mesh combinado");
        
            MaterialArrayAsset materialArrayAsset = ScriptableObject.CreateInstance<MaterialArrayAsset>();
            materialArrayAsset.materials = Materials.ToArray();
            saveAsset.SetObject(materialArrayAsset, "asset");
            FilePanelWindow.SaveFileAsset(ref saveAsset,"Guardar Materials Asset");
        }
    }
}