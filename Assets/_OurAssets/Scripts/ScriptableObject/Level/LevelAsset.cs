using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelAsset", menuName = "Game/FileAsset/LevelAsset")]
    public class LevelAsset : ScriptableObject, ISaveableAsset
    {
        public Mesh Mesh;
        public Material[] MeshMaterials;

        public Grid3d LevelGrid;
        
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
            
            LevelGrid.DebugGrid();
        }
    }
}
