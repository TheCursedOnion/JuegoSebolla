using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Files;
using CursedOnion.ScriptableObjects;
using UnityEngine;

namespace CursedOnion.Game.Systems.Files
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
        
        public void SpawnObject()
        {
            GameObject combined = new GameObject("CombinedMesh");
            MeshFilter mfCombined = combined.AddComponent<MeshFilter>();
            MeshRenderer mrCombined = combined.AddComponent<MeshRenderer>();

            mfCombined.sharedMesh = mesh;
            mrCombined.sharedMaterials = materials.ToArray();
        }

        public void Save()
        {
            /*AssetFile file = AssetFile.DefaultFile;
            
            file.SetObject(mesh, "asset");
            file.SaveTitle = "Guardar Mesh combinado";
            file.SaveMessage = "Elige dónde guardar el mesh combinado";
            
            if(FilePanelWindow.TryGetAssetDatabasePath(ref file, out string meshPath))
            {
                file.SaveAsset(meshPath);
            }
            
            MaterialArrayAsset materialArrayAsset = ScriptableObject.CreateInstance<MaterialArrayAsset>();
            materialArrayAsset.materials = materials.ToArray();
                file.SetObject(materialArrayAsset, "asset");
                file.SaveTitle = "Guardar Materials Asset";
                file.SaveMessage = "Elige dónde guardar el Asset de Materiales";
                
            if(FilePanelWindow.TryGetAssetDatabasePath(ref file, out string materialPath))
            {
                file.SaveAsset(materialPath);
            }*/
        }
    }
}