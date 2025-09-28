using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelAsset", menuName = "Game/FileAsset/LevelAsset")]
    public class LevelAsset : ScriptableObject, ISaveableAsset
    {
        public Mesh Mesh;
        public Material[] MeshMaterials;

        public Grid3d Grid;

        public void SetupLevelAsset(CombinedMesh combinedMesh, Grid3d grid3d)
        {
            this.Mesh = combinedMesh.Mesh;
            this.MeshMaterials = combinedMesh.MaterialsArray;
            this.Grid = grid3d;
        }
        public void Save()
        {
            AssetFile file = AssetFile.DefaultFile(Mesh, "asset");
            file.SetSaveTitle("Guardar Mesh");
            file.SetSaveMessage("Elige dónde guardar el Mesh del Nivel");
            file.SaveAsset();
            
            file.SetAssetObject(this, "asset");
            file.SetSaveTitle("Guardar Nivel");
            file.SetSaveMessage("Elige dónde guardar el Nivel");
            file.SaveAsset();
            
            Grid.DebugGrid();
        }
    }
}
